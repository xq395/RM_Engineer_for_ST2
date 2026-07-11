using BaseLib.Abstracts;
using System.Collections.Generic;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Mechs;

// 夹击（自定义持续版）：由天眼雷达在玩家回合结束时施加。
//  - 持续期间该敌人受到的所有强攻击伤害 +100%（×2）。
//  - 层数 = 剩余回合数；在玩家回合开始时 -1，归零移除。
//    这样它能存活到玩家的下一个回合，让机甲攻击时吃到加成（原版夹击在敌方回合结束就移除，时机不符）。
public class SustainedFlankingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == Owner && props.IsPoweredAttack())
        {
            return 2m;
        }
        return 1m;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // 与集火标记一致：debuff 挂在敌人身上（Owner.Side = 敌方），在敌方回合开始时倒计时。
        // 天眼雷达在玩家回合结束时施加，1 层可存活到玩家下个回合、覆盖机甲行动（玩家回合结束前）。
        if (side != Owner.Side)
        {
            return;
        }
        if (Amount <= 1)
        {
            await PowerCmd.Remove(this);
        }
        else
        {
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -1m, null, null);
        }
    }
}
