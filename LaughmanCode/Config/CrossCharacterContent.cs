using MegaCrit.Sts2.Core.Runs;

namespace Laughman.LaughmanCode.Config;

internal static class CrossCharacterContent
{
    public static bool IsLaughmanRun(IRunState runState) =>
        runState.Players.Any(player =>
            player.Character is global::Laughman.LaughmanCode.Character.Laughman);

    public static bool AllowForRun(bool shareWithOtherCharacters, IRunState runState) =>
        shareWithOtherCharacters || IsLaughmanRun(runState);

}
