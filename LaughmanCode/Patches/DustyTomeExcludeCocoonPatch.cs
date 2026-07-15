using System.Linq;
using HarmonyLib;
using Laughman.LaughmanCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Laughman.LaughmanCode.Patches;

// 尘封魔典固定给出本角色的专属先古卡「最是人间留不住」。
// 冲击UL、冠军形态和剑指春茧均保留各自的专属获取途径。
[HarmonyPatch(typeof(DustyTome), "SetupForPlayer")]
public static class DustyTomeExcludeCocoonPatch
{
    [HarmonyPrefix]
    private static bool ExcludeCocoon(DustyTome __instance, Player player)
    {
        var items = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c is TimeSparethNone)
            .ToList();
        if (items.Count == 0)
        {
            return true; // 没有可给的先古卡，交回原逻辑（极端兜底）。
        }
        __instance.AncientCard = player.PlayerRng.Rewards.NextItem(items).Id;
        return false;
    }
}
