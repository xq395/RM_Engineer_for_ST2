using BaseLib.Abstracts;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Laughman.LaughmanCode.Extensions;

namespace Laughman.LaughmanCode.Mechs;

// 视觉开源（能力）：每回合开始时，你的所有机器人各获得 1 点力量。
public class MechaProtocolPower : LaughmanPower
{
    private const int StrengthPerTurn = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;


    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.IsDead)
        {
            return;
        }
        if (CombatManager.Instance == null || CombatManager.Instance.IsEnding)
        {
            return;
        }

        var owner = Owner.PetOwner ?? (Owner.IsPlayer ? Owner.Player : null);
        if (owner == null)
        {
            return;
        }

        var mechs = owner.Creature.Pets.Where(p => p.Monster is MechModel && !p.IsDead).ToList();
        foreach (var mech in mechs)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), mech, StrengthPerTurn, Owner, null);
        }
    }
}
