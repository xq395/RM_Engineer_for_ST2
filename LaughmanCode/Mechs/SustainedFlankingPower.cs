using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Mechs;

// 天眼雷达的本轮夹击：只放大机器人强攻击，由协调器在机甲齐射后立即移除。
public class SustainedFlankingPower : LaughmanPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath =>
        "res://images/atlases/power_atlas.sprites/flanking_power.tres";

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == Owner && dealer?.Monster is MechModel && props.IsPoweredAttack())
        {
            return 2m;
        }
        return 1m;
    }
}
