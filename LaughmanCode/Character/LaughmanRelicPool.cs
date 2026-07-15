using BaseLib.Abstracts;
using Godot;
using Laughman.LaughmanCode.Config;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Unlocks;

namespace Laughman.LaughmanCode.Character;

public class LaughmanRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Laughman.Color;

    public override IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
        => LaughmanConfig.ShareRelicsWithOtherCharacters
            ? Array.Empty<RelicModel>()
            : GetUnlockedRelicsForSharedPool(unlockState);

    internal IEnumerable<RelicModel> GetUnlockedRelicsForSharedPool(UnlockState unlockState) =>
        base.GetUnlockedRelics(unlockState);
}
