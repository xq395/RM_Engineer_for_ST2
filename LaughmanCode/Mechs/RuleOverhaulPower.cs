using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Mechs;

// R18 赛制改革：每当你借用一台机器人时，抽 1 张牌、获得 3 点格挡，并使一台被借用机器人提前归队（借用回合 -1）。
public class RuleOverhaulPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

    public int BlockAmount { get; set; } = 1;

    public async Task OnBorrow(PlayerChoiceContext context, Player owner)
    {
        await CardPileCmd.Draw(context, 1, owner);
        await CreatureCmd.GainBlock(owner.Creature, BlockAmount, ValueProp.Move, null);
        await BorrowUtils.ReduceOneBorrow(context, owner);
    }
}
