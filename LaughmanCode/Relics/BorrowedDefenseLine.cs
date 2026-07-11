using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Relics;

// 借调防线（普通）：每场战斗你第一次借用机器人时，获得 8 点格挡。
[Pool(typeof(LaughmanRelicPool))]
public class BorrowedDefenseLine : LaughmanRelic
{
    private const int BlockAmount = 8;

    public override RelicRarity Rarity => RelicRarity.Common;

    // 本场是否已触发（每场第一次借用触发）。
    private bool _triggeredThisCombat;

    public override Task BeforeCombatStart()
    {
        _triggeredThisCombat = false;
        return Task.CompletedTask;
    }

    // 由 BorrowUtils 在借用成功后调用。
    public async Task OnBorrow(PlayerChoiceContext context, Player owner)
    {
        if (_triggeredThisCombat)
        {
            return;
        }
        _triggeredThisCombat = true;
        Flash();
        await CreatureCmd.GainBlock(owner.Creature, BlockAmount, ValueProp.Move, null);
    }
}
