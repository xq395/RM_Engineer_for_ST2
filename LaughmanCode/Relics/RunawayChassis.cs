using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Laughman.LaughmanCode.Relics;

// 失控底盘（商店）：每场战斗第一回合开始时，你获得 1 层小陀螺（本回合受伤减半），
// 但临时获得一张诅咒牌「受伤」加入手牌（本场用完清除，不进牌组）。
[Pool(typeof(LaughmanRelicPool))]
public class RunawayChassis : LaughmanRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    // 本场战斗是否已触发（每场只在第一回合触发一次）。
    private bool _triggeredThisCombat;

    public override Task BeforeCombatStart()
    {
        _triggeredThisCombat = false;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || _triggeredThisCombat)
        {
            return;
        }
        _triggeredThisCombat = true;
        Flash();

        // 玩家获得 1 层小陀螺（模组 buff）。
        await PowerCmd.Apply<GyroSpinPower>(choiceContext, player.Creature, 1m, player.Creature, null);

        // 临时获得一张「受伤」诅咒/状态牌到手牌（生成卡，不加入牌组）。
        var combatState = player.Creature.CombatState;
        if (combatState != null)
        {
            var wound = combatState.CreateCard<Wound>(player);
            await CardPileCmd.AddGeneratedCardToCombat(wound, PileType.Hand, player);
        }
    }
}
