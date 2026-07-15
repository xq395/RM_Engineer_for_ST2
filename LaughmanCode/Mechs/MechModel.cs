using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using Laughman.LaughmanCode.Bosses;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using Laughman.LaughmanCode.Extensions;

namespace Laughman.LaughmanCode.Mechs;

// 所有 RoboMaster 机器人（玩家侧宠物）的抽象基类。
//
// 关键约束（见 docs/mech-summon-system.md）：
//  - 引擎从不为玩家侧宠物自动 perform move，攻击/格挡/意图全部由挂在玩家身上的
//    MechCoordinatorPower 手动驱动。因此这里的 move state 只是惰性占位。
//  - 视觉用占位图（静态 Sprite，无 spine），长宽缩到 1/3，避免占位图过大遮住血条/状态栏。
//  - 每台机器人只负责“描述自己是谁、每回合做什么”（PerformTurn / RefreshIntent），
//    统一调度、多屏卫结算、金币运维等由协调器负责。
public abstract class MechModel : CustomMonsterModel
{
    // 占位图缩放：整节点长宽缩到 1/3（DefaultScale + Scale 同时设，防止受击 ScaleTo 恢复原大小）。
    protected const float VisualScale = 1f / 3f;

    // 占位血量，真实血量由 MechManager 召唤时用 CreatureCmd.SetMaxAndCurrentHp / GainMaxHp 覆写。
    public override int MinInitialHp => 1;
    public override int MaxInitialHp => 1;

    public override float HpBarSizeReduction => 40f;

    // ---- 机器人特性（供协调器读取）----

    // 是否拥有“屏卫”能力：会替玩家吸收未被格挡的强攻击（按召唤顺序结算）。
    public virtual bool IsGuard => false;

    // 是否拥有“飞行”：在屏卫全破后的共享伤害中受到的伤害减半。
    public virtual bool IsFlying => false;

    // 每回合开始扣除玩家多少金币（运维成本）。0 表示无成本。金币不足时该回合被“晕眩”，不行动。
    public virtual int UpkeepGold => 0;

    // 借用状态由挂在机器人身上的 BorrowedPower（debuff）表示，层数 = 剩余借用回合。
    // 被借用期间该机器人不行动、不参与屏卫结算、不参与共享受伤（判断见协调器）。
    public bool IsBorrowed => Creature.HasPower<BorrowedPower>();

    public int BorrowedTurns => Creature.HasPower<BorrowedPower>() ? Creature.GetPower<BorrowedPower>()!.Amount : 0;


    // 占位图文件名（位于 <ModId>/images/card_portraits/ 下）。默认用通用占位图。
    protected virtual string PlaceholderImage => "placeholder.png";

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        var texture = PreloadManager.Cache.GetTexture2D("res://" + PlaceholderImage.CardImagePath());
        if (texture == null)
        {
            MainFile.Logger.Info($"[{GetType().Name}] placeholder texture not found, falling back to default visuals");
            return null;
        }

        var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(texture);
        visuals.DefaultScale = VisualScale;
        visuals.Scale = Vector2.One * VisualScale;
        return visuals;
    }

    // 静态图无 spine，动画状态机返回 null（游戏检测到非 spine 会跳过动画）。
    public override CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller) => null;

    // 惰性 move 状态机：一个自循环空状态（真正行为由协调器驱动）。
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var idle = new MoveState("NOTHING_MOVE", _ => Task.CompletedTask);
        idle.FollowUpState = idle;
        return new MonsterMoveStateMachine(new[] { (MonsterState)idle }, idle);
    }

    // 刷新意图显示：设置一个只显示意图、不执行逻辑的 move（真实行为由协调器手动施加）。
    // intents 为空时显示“无意图/休息”。
    public void ShowIntent(params AbstractIntent[] intents)
    {
        var move = new MoveState("MECH_INTENT", _ => Task.CompletedTask, intents);
        move.FollowUpState = move;
        SetMoveImmediate(move, forceTransition: true);
    }

    // ---- 行为契约（由协调器在玩家回合开始时调用）----

    // 该机器人本回合执行的行为（攻击/格挡等）。由协调器统一调用。
    public abstract Task PerformTurn(Player owner, ICombatState combatState);

    // 刷新该机器人的意图显示（正常意图）。晕眩状态由协调器统一处理，不在此方法内。
    public abstract void RefreshIntent(Player owner, ICombatState combatState);

    // ---- 共享行为工具 ----

    // 仅供“测试打击”这类立即行动效果使用；非攻击行为不会读取该目标。
    public Creature? ForcedAttackTarget { get; set; }

    // 选择一个存活敌人作为机甲攻击目标：优先被「集火指令」标记的敌人，否则随机。
    private Creature? PickAttackTarget(Player owner, ICombatState combatState)
    {
        var enemies = combatState.HittableEnemies.Where(e => !e.IsDead).ToList();
        if (enemies.Count == 0)
        {
            return null;
        }
        if (ForcedAttackTarget != null && enemies.Contains(ForcedAttackTarget))
        {
            return ForcedAttackTarget;
        }
        var marked = enemies.Where(e => e.HasPower<FocusFirePower>()).ToList();
        if (marked.Count > 0)
        {
            return marked[owner.RunState.Rng.MonsterAi.NextInt(marked.Count)];
        }
        return enemies[owner.RunState.Rng.MonsterAi.NextInt(enemies.Count)];
    }

    // 攻击一个存活敌人，造成 damage 点伤害（powered attack，吃力量/易伤）。
    // 优先攻击被「集火指令」标记的敌人。
    protected async Task AttackRandomEnemy(Player owner, ICombatState combatState, decimal damage, int hits = 1)
    {
        var vigor = Creature.GetPower<VigorPower>();
        damage += vigor?.Amount ?? 0m;
        var target = PickAttackTarget(owner, combatState);
        if (target == null)
        {
            return;
        }
        for (int i = 0; i < hits; i++)
        {
            if (target.IsDead)
            {
                target = PickAttackTarget(owner, combatState);
                if (target == null)
                {
                    return;
                }
            }
            var props = Creature.HasPower<PrecisionGuidancePower>() ? ValueProp.Move | ValueProp.Unblockable : ValueProp.Move;
            // PersonalHivePowerPatch maps mech pet dealers back to their player owner, so this can
            // remain a powered attack and still receive Ramp Jump, guidance and focus-fire bonuses.
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, damage, props, Creature);
        }
        if (vigor != null && Creature.GetPower<VigorPower>() == vigor)
        {
            await PowerCmd.Remove(vigor);
        }
    }

    // 攻击所有存活敌人，各造成 damage 点伤害。
    protected async Task AttackAllEnemies(Player owner, ICombatState combatState, decimal damage, int hits = 1)
    {
        var vigor = Creature.GetPower<VigorPower>();
        damage += vigor?.Amount ?? 0m;
        for (int i = 0; i < hits; i++)
        {
            var enemies = combatState.HittableEnemies.Where(e => !e.IsDead).ToList();
            if (enemies.Count == 0)
            {
                return;
            }
            TripleCrownFormationPower.BeginDirectMultiTargetAttack(combatState);
            try
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemies, damage, ValueProp.Move, Creature);
            }
            finally
            {
                TripleCrownFormationPower.EndDirectMultiTargetAttack(combatState);
            }
        }
        if (vigor != null && Creature.GetPower<VigorPower>() == vigor)
        {
            await PowerCmd.Remove(vigor);
        }
    }

    // 给主人获得格挡。
    protected async Task GivePlayerBlock(Player owner, decimal block)
    {
        if (!owner.Creature.IsDead)
        {
            // This move grants block to the player, so the game's Dexterity lookup would otherwise
            // only see the player. Add the mech's Dexterity before normal player-side modifiers apply.
            decimal mechDexterity = Creature.GetPower<DexterityPower>()?.Amount ?? 0m;
            await CreatureCmd.GainBlock(owner.Creature, block + mechDexterity, ValueProp.Move, null);
        }
    }

    // 给自己获得格挡（用于屏卫吸收池）。
    protected async Task GiveSelfBlock(decimal block)
    {
        if (!Creature.IsDead)
        {
            await CreatureCmd.GainBlock(Creature, block, ValueProp.Move, null);
        }
    }
}
