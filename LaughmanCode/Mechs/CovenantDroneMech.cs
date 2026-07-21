using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Mechs;

// 新约无人机（R）：继承无人机以兼容原有交互（曼巴出去、飞行、共享伤害减半）。
// 行为与原版无人机完全不同：
//  - 行动时：扣 3 金币 + 扣自身 1 血（不可格挡），给一台其他机甲（屏卫优先）+护盾，
//    并让它本回合临时 -X 力量（DarkShackles），自身 +护盾，并本回合临时 +X 力量（FlexPotion）。
//    若没有可偷取的其他机甲，自身改为本回合临时 +2 力量。然后攻击（基础 0，吃自身力量）×5。
//  - 双方力量都只在本回合临时生效，回合结束自动恢复。
//  - 金币不足 3 时本回合不行动（休息），避免白扣血。
//  - 升级：双方护盾 5->7，临时力量 3->4。
public class CovenantDroneMech : DroneMech
{
    private const int GoldCost = 3;
    // 偷力量不再跨回合永久累积后（bug 修复），下调自伤作为补偿：3->1。
    // 新约仍保有“偷队友力量 + 双方护盾 + 多段攻击”的异格质变，只是不再无限滚雪球。
    private const int SelfDamage = 1;
    private const int AttackHits = 5;
    private const int NoTargetStrength = 2;

    // 由卡设置：护盾量（5/7）与临时力量量（3/4）。
    public int ShieldAmount { get; set; } = 5;
    public int StrengthSteal { get; set; } = 3;

    // 覆盖原版无人机的运维金币：自身在 PerformTurn 里处理 3 金币开销。
    public override int UpkeepGold => 0;

    private bool CanAfford(Player owner) => owner.Gold >= GoldCost;

    public override async Task PerformTurn(Player owner, ICombatState combatState)
    {
        if (!CanAfford(owner))
        {
            return; // 金币不足，本回合不行动。
        }
        await PlayerCmd.LoseGold(GoldCost, owner, GoldLossType.Spent);
        // 自残 3 血（不可格挡、无力量修正）。
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Creature, SelfDamage, ValueProp.Unblockable | ValueProp.Unpowered, Creature);
        if (Creature.IsDead)
        {
            return;
        }

        // 选一台“其他机甲”作为受益者，屏卫优先。
        var others = owner.Creature.Pets
            .Select(p => p.Monster as MechModel)
            .Where(m => m != null && m != this && !m!.Creature.IsDead)
            .ToList();
        var ally = others.FirstOrDefault(m => m!.IsGuard) ?? others.FirstOrDefault();
        var ctx = new ThrowingPlayerChoiceContext();
        if (ally != null)
        {
            // 有目标：给它护盾，本回合临时 -X 力量（DarkShackles，回合末恢复）。
            await CreatureCmd.GainBlock(ally.Creature, ShieldAmount, ValueProp.Move, null);
            await PowerCmd.Apply<DarkShacklesPower>(ctx, ally.Creature, StrengthSteal, Creature, null);
            // 自身 +护盾，本回合临时 +X 力量（FlexPotion，回合末移除）。
            await CreatureCmd.GainBlock(Creature, ShieldAmount, ValueProp.Move, null);
            await PowerCmd.Apply<FlexPotionPower>(ctx, Creature, StrengthSteal, Creature, null);
        }
        else
        {
            // 没有可偷取目标：自身 +护盾，本回合临时 +2 力量。
            await CreatureCmd.GainBlock(Creature, ShieldAmount, ValueProp.Move, null);
            await PowerCmd.Apply<FlexPotionPower>(ctx, Creature, NoTargetStrength, Creature, null);
        }

        // 攻击 0×5（吃自身力量）。
        await AttackRandomEnemy(owner, combatState, 0m, AttackHits);
    }

    public override void RefreshIntent(Player owner, ICombatState combatState)
    {
        if (!CanAfford(owner))
        {
            ShowIntent(new SleepIntent());
            return;
        }
        // 基础攻击 0，显示时按自身力量动态修正（FixedAttackIntent 已处理），5 段。
        ShowIntent(new FixedAttackIntent(0, AttackHits), new BuffIntent());
    }
}
