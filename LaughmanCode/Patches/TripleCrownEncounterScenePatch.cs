using Godot;
using HarmonyLib;
using Laughman.LaughmanCode.Bosses;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Patches;

[HarmonyPatch(typeof(EncounterModel), nameof(EncounterModel.CreateScene))]
internal static class TripleCrownEncounterScenePatch
{
    private static void Postfix(EncounterModel __instance, Control __result)
    {
        if (__instance is not TripleCrownEncounter)
        {
            return;
        }

        AddMarker(__result, "dart", new Vector2(1080f, 410f));
        AddMarker(__result, "drone", new Vector2(1260f, 390f));
        AddMarker(__result, "engineer", new Vector2(1430f, 420f));
        AddMarker(__result, "hero", new Vector2(1600f, 430f));
        AddMarker(__result, "infantry3", new Vector2(1070f, 650f));
        AddMarker(__result, "sentinel", new Vector2(1250f, 670f));
        AddMarker(__result, "boss", new Vector2(1430f, 650f));
        AddMarker(__result, "infantry4", new Vector2(1610f, 670f));
    }

    private static void AddMarker(Control root, string name, Vector2 position)
    {
        if (root.HasNode(name))
        {
            return;
        }

        root.AddChild(new Marker2D
        {
            Name = name,
            Position = position
        });
    }
}
