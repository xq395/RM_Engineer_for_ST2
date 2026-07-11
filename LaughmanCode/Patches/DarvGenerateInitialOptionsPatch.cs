using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Laughman.LaughmanCode.Patches;

// Some modded Ancient dialogue/option patches can make Darv generate options before Owner is ready.
// Darv's own filters dereference Owner, so provide a narrow fallback instead of letting the event room hard-lock.
[HarmonyPatch(typeof(Darv), "GenerateInitialOptions")]
public static class DarvGenerateInitialOptionsPatch
{
    [HarmonyFinalizer]
    private static Exception? Finalizer(Exception? __exception, Darv __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (__exception is not NullReferenceException)
        {
            return __exception;
        }

        MainFile.Logger.Error("Darv.GenerateInitialOptions hit a null reference; falling back to a safe Dusty Tome option.");
        var relic = ModelDb.Relic<DustyTome>().ToMutable();
        var method = AccessTools.Method(typeof(AncientEventModel), "RelicOption", new[] { typeof(RelicModel), typeof(string), typeof(string) });
        __result = new[] { (EventOption)method.Invoke(__instance, new object?[] { relic, "INITIAL", null })! };
        return null;
    }
}
