using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Laughman.LaughmanCode.Mechs;

public class AiSentinelPower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public int BlockAmount { get; set; } = 10;
    public int HitCount { get; set; } = 5;
}
