using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Laughman.LaughmanCode.Mechs;

public class ChampionFormPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();
}
