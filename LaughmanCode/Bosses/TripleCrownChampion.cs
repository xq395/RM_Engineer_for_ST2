using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Laughman.LaughmanCode.Bosses;

public sealed class TripleCrownChampion : CustomMonsterModel
{
    private const int StartingMinerals = 30;
    private int _minerals = StartingMinerals;
    private bool _repairMainNext = true;

    public override int MinInitialHp => 360;
    public override int MaxInitialHp => 360;
    public override float HpBarSizeReduction => 30f;

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        var texture = PreloadManager.Cache.GetTexture2D("res://Laughman/images/monsters/triple_crown_base.png");
        if (texture == null)
        {
            return null;
        }
        var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(texture);
        visuals.DefaultScale = 0.72f;
        visuals.Scale = Vector2.One * 0.72f;
        return visuals;
    }

    public override CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller) => null;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<TripleCrownFormationPower>(new ThrowingPlayerChoiceContext(), Creature, 1m, Creature, null);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var mainDeploy = new MoveState("MAIN_DEPLOY", MainDeploy, new SummonIntent());
        var member1 = new MoveState("MEMBER_RADAR_1", MemberAndRadar, new BuffIntent(), new DebuffIntent(strong: true));
        var specialDeploy = new MoveState("SPECIAL_DEPLOY", SpecialDeploy, new SummonIntent());
        var gyro1 = new MoveState("GYRO_1", Gyro, new BuffIntent());
        var ramp1 = new MoveState("RAMP_1", Ramp, new BuffIntent());
        var repair1 = new MoveState("REPAIR_1", Repair, new HealIntent(), new SummonIntent());
        var member2 = new MoveState("MEMBER_RADAR_2", MemberAndRadar, new BuffIntent(), new DebuffIntent(strong: true));
        var gyro2 = new MoveState("GYRO_2", Gyro, new BuffIntent());
        var ramp2 = new MoveState("RAMP_2", Ramp, new BuffIntent());
        var repair2 = new MoveState("REPAIR_2", Repair, new HealIntent(), new SummonIntent());
        var member3 = new MoveState("MEMBER_RADAR_3", MemberAndRadar, new BuffIntent(), new DebuffIntent(strong: true));

        mainDeploy.FollowUpState = member1;
        member1.FollowUpState = specialDeploy;
        specialDeploy.FollowUpState = gyro1;
        gyro1.FollowUpState = ramp1;
        ramp1.FollowUpState = repair1;
        repair1.FollowUpState = member2;
        member2.FollowUpState = gyro2;
        gyro2.FollowUpState = ramp2;
        ramp2.FollowUpState = repair2;
        repair2.FollowUpState = member3;
        member3.FollowUpState = gyro1;

        return new MonsterMoveStateMachine(
            new MonsterState[] { mainDeploy, member1, specialDeploy, gyro1, ramp1, repair1, member2, gyro2, ramp2, repair2, member3 },
            mainDeploy);
    }

    private async Task MainDeploy(IReadOnlyList<Creature> targets)
    {
        await Summon<TripleCrownSentinel>("sentinel");
        await Summon<TripleCrownHero>("hero");
    }

    private async Task SpecialDeploy(IReadOnlyList<Creature> targets)
    {
        await Summon<TripleCrownInfantry4>("infantry4");
        await Summon<TripleCrownDrone>("drone");
        await Summon<TripleCrownEngineer>("engineer");
        await Summon<TripleCrownDart>("dart");
    }

    private async Task MemberAndRadar(IReadOnlyList<Creature> targets)
    {
        if (await TryBuyback()) return;
        var minions = CombatState.Enemies.Where(enemy => enemy.IsAlive && enemy.Monster is ITripleCrownMinion).ToList();
        var context = new ThrowingPlayerChoiceContext();
        // 联调改为“队员主体强化”：力量增幅大幅降低（3->1），改以覆甲和血上限强化防御，
        // 更贴合冠军战队队员培养的主题，同时压低此前失控的力量滚雪球。
        //  - 视觉：+1 力量
        //  - 硬件：+1 敏捷
        //  - 电控：+2 覆甲
        //  - 机械：+3 最大生命
        await PowerCmd.Apply<StrengthPower>(context, minions, 1m, Creature, null);
        await PowerCmd.Apply<DexterityPower>(context, minions, 1m, Creature, null);
        await PowerCmd.Apply<PlatingPower>(context, minions, 2m, Creature, null);
        foreach (var minion in minions)
        {
            await CreatureCmd.GainMaxHp(minion, 3m);
        }
        await PowerCmd.Apply<FrailPower>(context, targets, 3m, Creature, null);
        await PowerCmd.Apply<VulnerablePower>(context, targets, 3m, Creature, null);
    }

    private async Task Gyro(IReadOnlyList<Creature> targets)
    {
        if (await TryBuyback()) return;
        var eligible = CombatState.Enemies.Where(enemy => enemy.IsAlive && enemy.Monster is
            TripleCrownInfantry3 or TripleCrownSentinel or TripleCrownHero or TripleCrownInfantry4).ToList();
        await PowerCmd.Apply<TripleCrownGyroPower>(new ThrowingPlayerChoiceContext(), eligible, 1m, Creature, null);
    }

    private async Task Ramp(IReadOnlyList<Creature> targets)
    {
        if (await TryBuyback()) return;
        var target = CombatState.Enemies.FirstOrDefault(enemy => enemy.IsAlive && enemy.Monster is TripleCrownHero)
            ?? CombatState.Enemies.FirstOrDefault(enemy => enemy.IsAlive && enemy.Monster is TripleCrownInfantry3)
            ?? CombatState.Enemies.FirstOrDefault(enemy => enemy.IsAlive && enemy.Monster is TripleCrownInfantry4)
            ?? CombatState.Enemies.FirstOrDefault(enemy => enemy.IsAlive && enemy.Monster is TripleCrownSentinel);
        if (target != null)
        {
            if (target.GetPower<TripleCrownGyroPower>() is { } gyro)
            {
                await PowerCmd.Remove(gyro);
            }
            await PowerCmd.Apply<TripleCrownRampPower>(new ThrowingPlayerChoiceContext(), target, 1m, Creature, null);
        }
    }

    private async Task Repair(IReadOnlyList<Creature> targets)
    {
        if (await TryBuyback()) return;
        Type[] group = _repairMainNext
            ? new[] { typeof(TripleCrownInfantry3), typeof(TripleCrownSentinel), typeof(TripleCrownHero) }
            : new[] { typeof(TripleCrownInfantry4), typeof(TripleCrownDrone), typeof(TripleCrownEngineer), typeof(TripleCrownDart) };
        _repairMainNext = !_repairMainNext;

        foreach (var type in group)
        {
            var unit = CombatState.Enemies.FirstOrDefault(enemy => enemy.Monster?.GetType() == type);
            if (unit == null)
            {
                continue;
            }
            if (unit.IsDead)
            {
                await ResetAndRevive(unit, (int)Math.Ceiling(unit.MaxHp * 0.3m), actThisTurn: false);
            }
            else
            {
                await CreatureCmd.Heal(unit, Math.Ceiling(unit.MaxHp * 0.25m));
            }
        }
    }

    private async Task<bool> TryBuyback()
    {
        if (_minerals < 6)
        {
            return false;
        }

        var dead = CombatState.Enemies
            .Where(enemy => enemy.IsDead && enemy.Monster is ITripleCrownMinion { CanBuyback: true })
            .OrderBy(enemy => enemy.Monster switch
            {
                TripleCrownHero => 0,
                TripleCrownSentinel => 1,
                TripleCrownInfantry4 => 2,
                TripleCrownInfantry3 => 3,
                _ => 4
            })
            .FirstOrDefault();
        if (dead == null)
        {
            return false;
        }

        _minerals -= 6;
        await ResetAndRevive(dead, 20, actThisTurn: true);
        await PowerCmd.Apply<TripleCrownGyroPower>(new ThrowingPlayerChoiceContext(), dead, 1m, Creature, null);
        return true;
    }

    private async Task ResetAndRevive(Creature unit, int hp, bool actThisTurn)
    {
        if (unit.GetPower<StrengthPower>() is { } strength) await PowerCmd.Remove(strength);
        if (unit.GetPower<DexterityPower>() is { } dexterity) await PowerCmd.Remove(dexterity);
        if (unit.GetPower<TripleCrownRampPower>() is { } ramp) await PowerCmd.Remove(ramp);
        if (unit.GetPower<TripleCrownGyroPower>() is { } gyro) await PowerCmd.Remove(gyro);
        if (unit.Block > 0) unit.LoseBlockInternal(unit.Block);
        if (unit.Monster is TripleCrownMinion minion) minion.ResetAfterRevive();
        await CreatureCmd.Heal(unit, hp);
        if (!actThisTurn)
        {
            await CreatureCmd.Stun(unit, (string?)null);
        }
    }

    private async Task Summon<T>(string slot) where T : TripleCrownMinion
    {
        if (CombatState.Enemies.Any(enemy => enemy.Monster is T))
        {
            return;
        }
        await CreatureCmd.Add<T>(CombatState, slot);
    }
}
