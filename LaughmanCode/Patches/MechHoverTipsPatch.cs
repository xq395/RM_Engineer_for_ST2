using HarmonyLib;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;

namespace Laughman.LaughmanCode.Patches;

[HarmonyPatch(typeof(Creature), "get_HoverTips")]
public static class MechHoverTipsPatch
{
    [HarmonyPostfix]
    private static void Postfix(Creature __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance.Monster is not MechModel mech)
        {
            return;
        }
        __result = __result.Concat(new[] { MechHoverTipProvider.GetActionTip(mech) });
    }
}
