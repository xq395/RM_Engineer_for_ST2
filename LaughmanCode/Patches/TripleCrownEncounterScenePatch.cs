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

        // 屏卫集中在左侧前排，其余单位在右侧分层展开。
        AddMarker(__result, "infantry3", new Vector2(1040f, 610f));
        AddMarker(__result, "sentinel", new Vector2(1130f, 430f));
        AddMarker(__result, "infantry4", new Vector2(1220f, 610f));
        AddMarker(__result, "hero", new Vector2(1370f, 390f));
        AddMarker(__result, "boss", new Vector2(1450f, 620f));
        AddMarker(__result, "drone", new Vector2(1540f, 360f));
        AddMarker(__result, "engineer", new Vector2(1640f, 610f));
        AddMarker(__result, "dart", new Vector2(1740f, 420f));
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
