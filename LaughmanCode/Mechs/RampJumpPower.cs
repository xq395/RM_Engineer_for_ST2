using BaseLib.Abstracts;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Laughman.LaughmanCode.Extensions;

namespace Laughman.LaughmanCode.Mechs;

// 飞坡（U7）施加在机器人身上的临时增益：
//  - 本回合该机器人造成的伤害 ×3（结算三倍，多段各段分别翻倍）。
//  - 本回合该机器人受到的伤害减半。
// 下一次玩家回合开始时移除，覆盖敌方回合伤害。
public class RampJumpPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

    // 造成伤害 ×3。
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer == Owner && props.IsPoweredAttack())
        {
            return 3m;
        }
        // 受到伤害减半。
        if (target == Owner)
        {
            return 0.5m;
        }
        return 1m;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
