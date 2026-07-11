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

// 集火标记（debuff）：由「集火指令」施加在一名敌人身上。
//  - 层数 = 剩余持续回合数，玩家回合开始时 -1，归零移除。
//  - 标记期间该敌人被机甲优先攻击（判断见 MechModel.PickAttackTarget）。
//  - 该敌人受到来自机甲的每段伤害 +1（多段/全体各段分别 +1）。
public class FocusFirePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

    // 机甲对本目标的每段伤害 +1（仅限机甲来源的强攻击）。
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == Owner && dealer?.Monster is MechModel && props.IsPoweredAttack())
        {
            return 1m;
        }
        return 0m;
    }

    // 玩家回合开始时递减，归零移除。
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // 标记由玩家侧施加，随玩家回合推进倒计时。
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
