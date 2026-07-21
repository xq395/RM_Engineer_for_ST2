using BaseLib.Abstracts;
using System.Collections.Generic;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Laughman.LaughmanCode.Config;

namespace Laughman.LaughmanCode.Mechs;

public abstract class TeamMemberPower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected abstract TeamMemberType MemberType { get; }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side && Owner.IsPlayer && Owner.Player != null)
        {
            int triggerAmount = Amount;
            await TeamMemberUtils.Trigger(Owner.Player, MemberType, triggerAmount);
            int stableAmount = WeakHelper.V(2, 3);
            if (triggerAmount > stableAmount)
            {
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -1m, Owner, null);
            }
        }
    }
}

public class MechanicalMemberPower : TeamMemberPower
{
    protected override TeamMemberType MemberType => TeamMemberType.Mechanical;
}

public class ElectricalMemberPower : TeamMemberPower
{
    protected override TeamMemberType MemberType => TeamMemberType.Electrical;
}

public class VisionMemberPower : TeamMemberPower
{
    protected override TeamMemberType MemberType => TeamMemberType.Vision;
}

public class HardwareMemberPower : TeamMemberPower
{
    protected override TeamMemberType MemberType => TeamMemberType.Hardware;
}
