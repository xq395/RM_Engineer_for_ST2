using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Relics;

// 美美撤离（事件遗物，事件2「跑路了兄弟，跑路了」选项奖励）：
// 获得时，+300 金币，获得 2 瓶随机药水，并从牌组选择移除 5 张牌。
[Pool(typeof(LaughmanRelicPool))]
public class GracefulRetreat : LaughmanRelic
{
    private const int Gold = 300;
    private const int Potions = 2;
    private const int RemoveCount = 5;

    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task AfterObtained()
    {
        await PlayerCmd.GainGold(Gold, Owner);

        // 获得 2 瓶随机药水（从角色药水池随机抽）。
        var pool = ModelDb.PotionPool<LaughmanPotionPool>()
            .GetUnlockedPotionsForSharedPool(Owner.UnlockState).ToList();
        if (pool.Count > 0)
        {
            for (int i = 0; i < Potions; i++)
            {
                var canonical = Owner.PlayerRng.Rewards.NextItem(pool);
                if (canonical != null)
                {
                    await PotionCmd.TryToProcure(canonical.ToMutable(), Owner);
                }
            }
        }

        // 从牌组选择并移除 5 张牌。
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, RemoveCount);
        var chosen = (await CardSelectCmd.FromDeckForRemoval(Owner, prefs)).ToList();
        if (chosen.Count > 0)
        {
            await CardPileCmd.RemoveFromDeck(chosen);
        }
    }
}
