using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
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

public sealed class TripleCrownGyroPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

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

public sealed class TripleCrownRampPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

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

public sealed class TripleCrownFormationPower : CustomPowerModel
{
    private static readonly AsyncLocal<int> DirectMultiTargetDepth = new();
    private readonly List<Creature> _pendingKills = new();
    private IReadOnlyList<Creature>? _guardSnapshot;
    private bool _isMultiTargetAttack;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();
    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (amount <= 0m || !props.IsPoweredAttack() || Owner.CombatState == null)
        {
            return amount;
        }

        if (target == Owner)
        {
            return RouteBossDamage(amount, props, AliveGuards());
        }

        if (target.Monster is not TripleCrownMinion minion)
        {
            return amount;
        }

        decimal multiplier = 1m;
        if (minion.IsBackline && (_guardSnapshot ?? AliveGuards()).Count > 0)
        {
            multiplier *= 0.5m;
        }
        if (IsMultiTargetAttack)
        {
            multiplier *= 0.75m;
            if (target.Monster is TripleCrownDrone)
            {
                multiplier *= 0.5m;
            }
        }
        return Math.Floor(amount * multiplier);
    }

    private decimal RouteBossDamage(decimal amount, ValueProp props, IReadOnlyList<Creature> guards)
    {
        decimal remaining = amount;
        foreach (var guard in guards)
        {
            if (remaining <= 0m)
            {
                break;
            }

            decimal multiplier = IncomingMultiplier(guard);
            if (IsMultiTargetAttack)
            {
                multiplier *= 0.75m;
            }
            int effectiveDamage = (int)Math.Floor(remaining * multiplier);
            if (effectiveDamage <= 0)
            {
                return 0m;
            }

            int effectiveAbsorbed = 0;
            if (guard.Block > 0)
            {
                int absorbed = Math.Min(guard.Block, effectiveDamage);
                guard.LoseBlockInternal(absorbed);
                effectiveDamage -= absorbed;
                effectiveAbsorbed += absorbed;
                if (effectiveDamage <= 0)
                {
                    return 0m;
                }
            }

            var result = guard.LoseHpInternal(effectiveDamage, props);
            effectiveAbsorbed += result.UnblockedDamage;
            if (!result.WasTargetKilled)
            {
                return 0m;
            }

            _pendingKills.Add(guard);
            remaining = Math.Max(0m, remaining - effectiveAbsorbed / multiplier);
        }
        return remaining;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await FlushPendingKills();
    }

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.TargetSide == Owner.Side && command.DamageProps.IsPoweredAttack())
        {
            _isMultiTargetAttack = command.IsMultiTargeted;
            _guardSnapshot = AliveGuards();
        }
        return Task.CompletedTask;
    }

    public override Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        _isMultiTargetAttack = false;
        _guardSnapshot = null;
        return Task.CompletedTask;
    }

    internal static void BeginDirectMultiTargetAttack(ICombatState combatState)
    {
        DirectMultiTargetDepth.Value++;
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
        var formation = combatState.Enemies
            .FirstOrDefault(enemy => enemy.Monster is TripleCrownChampion)
            ?.GetPower<TripleCrownFormationPower>();
        if (formation != null)
        {
            formation._guardSnapshot = null;
        }
    }

    private bool IsMultiTargetAttack => _isMultiTargetAttack || DirectMultiTargetDepth.Value > 0;

    public override async Task BeforeDeath(Creature creature)
    {
        if (creature == Owner)
        {
            await FlushPendingKills();
        }
    }

    private async Task FlushPendingKills()
    {
        if (_pendingKills.Count == 0)
        {
            return;
        }
        var dead = _pendingKills.Where(creature => creature.IsDead).Distinct().ToList();
        _pendingKills.Clear();
        if (dead.Count > 0)
        {
            await CreatureCmd.Kill(dead);
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

    private static decimal IncomingMultiplier(Creature creature)
    {
        if (creature.HasPower<TripleCrownRampPower>() || creature.HasPower<TripleCrownGyroPower>())
        {
            return 0.5m;
        }
        return 1m;
    }
}
