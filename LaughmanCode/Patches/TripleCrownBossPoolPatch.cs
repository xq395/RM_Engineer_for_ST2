using HarmonyLib;
using Laughman.LaughmanCode.Bosses;
using Laughman.LaughmanCode.Config;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Laughman.LaughmanCode.Patches;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.GenerateRooms))]
internal static class TripleCrownBossPoolPatch
{
    [HarmonyPostfix]
    private static void FilterForOtherCharacters(RunManager __instance)
    {
        if (LaughmanConfig.ShareBossesWithOtherCharacters)
        {
            return;
        }

        var runState = Traverse.Create(__instance).Property<RunState>("State").Value;
        if (runState is null || CrossCharacterContent.IsLaughmanRun(runState))
        {
            return;
        }

        foreach (var act in runState.Acts)
        {
            var rooms = Traverse.Create(act).Field<RoomSet>("_rooms").Value;
            if (rooms.Boss is TripleCrownEncounter)
            {
                rooms.Boss = PickReplacement(act, runState, null);
            }

            if (rooms.SecondBoss is TripleCrownEncounter)
            {
                rooms.SecondBoss = PickReplacement(act, runState, rooms.Boss.Id);
            }
        }
    }

    private static EncounterModel PickReplacement(
        ActModel act,
        RunState runState,
        ModelId? excludedId)
    {
        var candidates = act.AllBossEncounters.Where(encounter =>
            encounter is not TripleCrownEncounter
            && (excludedId is null || encounter.Id != excludedId));
        return runState.Rng.UpFront.NextItem(candidates)
               ?? throw new InvalidOperationException(
                   $"No replacement boss is available for act {act.Id.Entry}.");
    }
}
