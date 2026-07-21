using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Bosses;

public sealed class TripleCrownGyroPower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "gyro_spin_power.png".PowerImagePath();

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => target == Owner ? 0.5m : 1m;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

public sealed class TripleCrownRampPower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "ramp_jump_power.png".PowerImagePath();

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer == Owner && props.IsPoweredAttack())
        {
            return 3m;
        }
        return target == Owner ? 0.5m : 1m;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

public sealed class TripleCrownFormationPower : LaughmanPower
{
    private static readonly AsyncLocal<int> DirectMultiTargetDepth = new();
    private IReadOnlyList<Creature>? _guardSnapshot;

    // 每台存活屏卫为本体和后排提供的减伤，最多叠加到 GuardReductionCap。
    private const decimal GuardReductionPerGuard = 0.2m;
    private const decimal GuardReductionCap = 0.6m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    // 屏卫光环：敌方每有一台存活屏卫（3号步兵/哨兵/4号步兵），本体和后排受到的强攻击伤害各减少
    // 20%，最多 60%。屏卫本体正常承伤，可被击破以逐级削弱光环。
    //  - 集火拆屏卫（对单流）：每杀一台屏卫，本体和后排减伤下降 20%，进展稳定。
    //  - 分散铺场群攻（不早减员）：屏卫存活越久，整队减伤越持久，群攻收益被压低。
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (amount <= 0m || !props.IsPoweredAttack() || Owner.CombatState == null)
        {
            return amount;
        }

        // 屏卫自身不吃光环减伤，正常承伤，方便被集火击破。
        if (target.Monster is TripleCrownMinion guardMinion && guardMinion.IsGuard)
        {
            return amount;
        }

        bool isBoss = target == Owner;
        bool isBackline = target.Monster is TripleCrownMinion backlineMinion && backlineMinion.IsBackline;
        if (!isBoss && !isBackline)
        {
            return amount;
        }

        int guardCount = (_guardSnapshot ?? AliveGuards()).Count;
        if (guardCount <= 0)
        {
            return amount;
        }

        decimal reduction = Math.Min(GuardReductionCap, GuardReductionPerGuard * guardCount);
        return Math.Floor(amount * (1m - reduction));
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        // 单次攻击开始时锁定屏卫数量，保证一次多段/群攻内所有目标看到一致的光环层数。
        if (command.TargetSide == Owner.Side && command.DamageProps.IsPoweredAttack())
        {
            _guardSnapshot = AliveGuards();
        }
        return Task.CompletedTask;
    }

    public override Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        _guardSnapshot = null;
        return Task.CompletedTask;
    }

    internal static void BeginDirectMultiTargetAttack(ICombatState combatState)
    {
        DirectMultiTargetDepth.Value++;
        if (DirectMultiTargetDepth.Value > 1)
        {
            return;
        }
        var formation = combatState.Enemies
            .FirstOrDefault(enemy => enemy.Monster is TripleCrownChampion)
            ?.GetPower<TripleCrownFormationPower>();
        if (formation != null)
        {
            formation._guardSnapshot = formation.AliveGuards();
        }
    }

    internal static void EndDirectMultiTargetAttack(ICombatState combatState)
    {
        DirectMultiTargetDepth.Value = Math.Max(0, DirectMultiTargetDepth.Value - 1);
        if (DirectMultiTargetDepth.Value > 0)
        {
            return;
        }
        var formation = combatState.Enemies
            .FirstOrDefault(enemy => enemy.Monster is TripleCrownChampion)
            ?.GetPower<TripleCrownFormationPower>();
        if (formation != null)
        {
            formation._guardSnapshot = null;
        }
    }

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return creature.Monster is not ITripleCrownMinion;
    }

    private List<Creature> AliveGuards()
    {
        if (Owner.CombatState == null)
        {
            return new List<Creature>();
        }

        var order = new[]
        {
            typeof(TripleCrownInfantry3),
            typeof(TripleCrownSentinel),
            typeof(TripleCrownInfantry4)
        };
        return order
            .Select(type => Owner.CombatState.Enemies.FirstOrDefault(enemy =>
                enemy.IsAlive && enemy.Monster?.GetType() == type))
            .Where(creature => creature != null)
            .Cast<Creature>()
            .ToList();
    }
}
