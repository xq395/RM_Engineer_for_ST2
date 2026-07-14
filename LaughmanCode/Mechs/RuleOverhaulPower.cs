using BaseLib.Abstracts;
using System.Collections.Generic;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Mechs;

// R18 时间管理大师：每台机器人每回合第一次被借用时，抽牌、获得格挡并归队 1。
public class RuleOverhaulPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

    public int BlockAmount { get; set; } = 1;
    private readonly HashSet<Creature> _triggeredMechs = new();

    public async Task OnBorrow(PlayerChoiceContext context, Player owner, MechModel mech)
    {
        if (!_triggeredMechs.Add(mech.Creature))
        {
            return;
        }
        await CardPileCmd.Draw(context, 1, owner);
        await CreatureCmd.GainBlock(owner.Creature, BlockAmount, ValueProp.Move, null);
        await BorrowUtils.ReduceOneBorrow(context, owner);
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side)
        {
            _triggeredMechs.Clear();
        }
        return Task.CompletedTask;
    }
}
