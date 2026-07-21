using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Laughman.LaughmanCode.Mechs;

public class ChampionFormPower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
