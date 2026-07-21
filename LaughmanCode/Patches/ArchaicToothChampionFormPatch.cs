using System.Linq;
using HarmonyLib;
using Laughman.LaughmanCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Laughman.LaughmanCode.Patches;

// 扩展真正的古老牙齿选择逻辑：
//  - 基地补给(BaseSupply) → 冠军形态(ChampionForm)
//  - 3号步兵部署(InfantrySummon) → 冲击UL(ImpactUL)
// 两者都是初始卡，古老牙齿在牌组里找到其一即变身。沿用原版“牌组中第一张”规则：
// 初始牌组默认先找到 3号步兵部署，确保冲击UL及后续剑指春茧任务链可达；
// 若它已被移除或转化，再由基地补给变为冠军形态。
[HarmonyPatch(typeof(ArchaicTooth))]
public static class ArchaicToothChampionFormPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("GetTranscendenceStarterCard")]
    private static void FindLaughmanStarter(Player player, ref CardModel? __result)
    {
        // 官方卡优先；官方没有时，按当前牌组顺序选择本角色的可超越初始牌。
        __result ??= player.Deck.Cards.FirstOrDefault(card => card is InfantrySummon or BaseSupply);
    }

    [HarmonyPrefix]
    [HarmonyPatch("GetTranscendenceTransformedCard")]
    private static bool TransformLaughmanStarter(CardModel starterCard, ref CardModel __result)
    {
        CardModel? replacement = starterCard switch
        {
            BaseSupply => starterCard.Owner.RunState.CreateCard<ChampionForm>(starterCard.Owner),
            InfantrySummon => starterCard.Owner.RunState.CreateCard<ImpactUL>(starterCard.Owner),
            _ => null
        };
        if (replacement == null)
        {
            return true; // 非本模组初始卡，走原逻辑。
        }
        if (starterCard.IsUpgraded)
        {
            CardCmd.Upgrade(replacement);
        }
        __result = replacement;
        return false;
    }
}
