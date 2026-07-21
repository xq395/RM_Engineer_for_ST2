using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Mechs;

public class InsomniaModePower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || !Owner.IsPlayer || Owner.Player == null)
        {
            return;
        }
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, 1m, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        await PlayerCmd.GainEnergy(1m, Owner.Player);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), 1, Owner.Player);
    }
}
