using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;

namespace Laughman.LaughmanCode.Patches;

internal static class LaughmanTimelineArt
{
    internal static string Small(string id) =>
        $"res://Laughman/images/timeline/epochs/{id.ToLowerInvariant()}.png";

    internal static string Big(string id) =>
        $"res://Laughman/images/timeline/epochs/big/{id.ToLowerInvariant()}.png";
}

[HarmonyPatch(typeof(NTimelineScreen), nameof(NTimelineScreen.GetEraIcon))]
internal static class LaughmanEraIconPatch
{
    private const int FirstEra = 2727;
    private const int LastEra = 2732;

    private static void Postfix(EpochEra era, ref (Texture2D Texture, string Name) __result)
    {
        int eraValue = (int)era;
        if (eraValue < FirstEra || eraValue > LastEra)
        {
            return;
        }

        string path = $"res://Laughman/images/timeline/eras/era_{eraValue}.png";
        var texture = PreloadManager.Cache.GetTexture2D(path);
        if (texture != null)
        {
            __result = (texture, __result.Name);
        }
    }
}
