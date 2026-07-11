using System.Linq;
using HarmonyLib;
using Laughman.LaughmanCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Laughman.LaughmanCode.Patches;

// 尘封魔典（达弗给一张先古卡）：把「剑指春茧」排除出可给出的先古卡池，
// 使其只能由三层特殊事件用「冲击UL」替换获得。冲击UL、冠军形态仍可正常被给出。
[HarmonyPatch(typeof(DustyTome), "SetupForPlayer")]
public static class DustyTomeExcludeCocoonPatch
{
    [HarmonyPrefix]
    private static bool ExcludeCocoon(DustyTome __instance, Player player)
    {
        var items = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c.Rarity == CardRarity.Ancient
                && !ArchaicTooth.TranscendenceCards.Contains(c)
                && c is not RoadToSpringCocoon)
            .ToList();
        if (items.Count == 0)
        {
            return true; // 没有可给的先古卡，交回原逻辑（极端兜底）。
        }
        __instance.AncientCard = player.PlayerRng.Rewards.NextItem(items).Id;
        return false;
    }
}
