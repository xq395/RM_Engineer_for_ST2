using HarmonyLib;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Config;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Unlocks;

namespace Laughman.LaughmanCode.Patches;

internal static class CrossCharacterRewardPoolPatch
{
    [HarmonyPatch(typeof(SharedPotionPool), nameof(SharedPotionPool.GetUnlockedPotions))]
    [HarmonyPostfix]
    private static IEnumerable<PotionModel> AddLaughmanPotions(
        IEnumerable<PotionModel> __result,
        UnlockState unlockState)
    {
        return LaughmanConfig.SharePotionsWithOtherCharacters
            ? __result.Concat(ModelDb.PotionPool<LaughmanPotionPool>()
                .GetUnlockedPotionsForSharedPool(unlockState))
            : __result;
    }

    [HarmonyPatch(typeof(SharedRelicPool), nameof(SharedRelicPool.GetUnlockedRelics))]
    [HarmonyPostfix]
    private static IEnumerable<RelicModel> AddLaughmanRelics(
        IEnumerable<RelicModel> __result,
        UnlockState unlockState)
    {
        return LaughmanConfig.ShareRelicsWithOtherCharacters
            ? __result.Concat(ModelDb.RelicPool<LaughmanRelicPool>()
                .GetUnlockedRelicsForSharedPool(unlockState))
            : __result;
    }
}
