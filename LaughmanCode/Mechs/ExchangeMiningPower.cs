using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Laughman.LaughmanCode.Config;

namespace Laughman.LaughmanCode.Mechs;

public class ExchangeMiningPower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _earned;
    private int _strengthGranted;

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var owner = Owner.IsPlayer ? Owner.Player : null;
        if (owner == null)
        {
            return;
        }
        await BuffMechs(owner);
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || !Owner.IsPlayer || Owner.Player == null)
        {
            return;
        }
        int perTurn = Amount switch { <= 1 => 5, 2 => 7, 3 => 9, _ => 10 };
        int cap = Amount switch { <= 1 => 25, 2 => 35, 3 => 45, _ => 50 };
        int gain = Math.Min(perTurn, cap - _earned);
        if (gain > 0)
        {
            _earned += gain;
            await PlayerCmd.GainGold(gain, Owner.Player);
        }
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target?.Monster is not MechModel)
        {
            return 1m;
        }
        return DamageMultiplierForAmount(Amount);
    }

    public static decimal DamageMultiplierForAmount(int amount) =>
        amount >= 4 ? WeakHelper.V(0.75m, 0.5m) : amount >= 3 ? 0.75m : 1m;

    private async Task BuffMechs(Player owner)
    {
        int targetStrength = Amount switch { 2 => 2, >= 3 => 3, _ => 0 };
        int strength = Math.Max(0, targetStrength - _strengthGranted);
        if (strength <= 0)
        {
            return;
        }
        foreach (var mech in TeamMemberUtils.AliveMechs(owner))
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), mech, strength, Owner, null);
        }
        _strengthGranted = targetStrength;
    }
}
