using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;

namespace Laughman.LaughmanCode.Bosses;

public sealed class TripleCrownEncounter : CustomEncounterModel
{
    private static readonly string[] EncounterSlots =
    {
        "boss", "infantry3", "sentinel", "hero",
        "infantry4", "drone", "engineer", "dart"
    };

    public TripleCrownEncounter() : base(RoomType.Boss)
    {
    }

    public override string? CustomScenePath =>
        "res://scenes/encounters/living_fog_normal.tscn";

    public override bool HasScene => true;
    public override IReadOnlyList<string> Slots => EncounterSlots;
    public override string? CustomRunHistoryIconPath =>
        "res://Laughman/images/boss/triple_crown_logo.png";
    public override string? CustomRunHistoryIconOutlinePath =>
        "res://Laughman/images/boss/triple_crown_logo.png";

    public override IEnumerable<MonsterModel> AllPossibleMonsters => new MonsterModel[]
    {
        ModelDb.Monster<TripleCrownChampion>(),
        ModelDb.Monster<TripleCrownInfantry3>(),
        ModelDb.Monster<TripleCrownSentinel>(),
        ModelDb.Monster<TripleCrownHero>(),
        ModelDb.Monster<TripleCrownInfantry4>(),
        ModelDb.Monster<TripleCrownDrone>(),
        ModelDb.Monster<TripleCrownEngineer>(),
        ModelDb.Monster<TripleCrownDart>()
    };

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        new (MonsterModel, string?)[]
        {
            (ModelDb.Monster<TripleCrownChampion>().ToMutable(), "boss"),
            (ModelDb.Monster<TripleCrownInfantry3>().ToMutable(), "infantry3")
        };

    public override bool IsValidForAct(ActModel act) => act is Glory;
}
