using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Laughman.LaughmanCode.Relics;

// 量产流水线（罕见）：每场战斗你第二次召唤机器人时，抽 3 张牌。
[Pool(typeof(LaughmanRelicPool))]
public class MassProductionLine : LaughmanRelic
{
    private const int DrawAmount = 3;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    // 本场战斗的召唤次数（每场重置）。
    private int _summonsThisCombat;

    public override Task BeforeCombatStart()
    {
        _summonsThisCombat = 0;
        return Task.CompletedTask;
    }

    // 由 MechManager 在每次召唤（含同名叠加）后调用。
    public async Task OnSummon(Player owner)
    {
        _summonsThisCombat++;
        if (_summonsThisCombat != 2)
        {
            return;
        }
        Flash();
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), DrawAmount, owner);
    }
}
