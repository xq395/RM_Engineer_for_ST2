using BaseLib.Abstracts;
using System.Collections.Generic;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Laughman.LaughmanCode.Mechs;

public class AnnualRecruitmentNextPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || !Owner.IsPlayer)
        {
            return;
        }
        var context = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<MechanicalMemberPower>(context, Owner, 1m, Owner, null);
        await PowerCmd.Apply<ElectricalMemberPower>(context, Owner, 1m, Owner, null);
        await PowerCmd.Apply<VisionMemberPower>(context, Owner, 1m, Owner, null);
        await PowerCmd.Apply<HardwareMemberPower>(context, Owner, 1m, Owner, null);
        await PowerCmd.Remove(this);
    }
}
