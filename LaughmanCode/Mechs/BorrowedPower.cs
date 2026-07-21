using BaseLib.Abstracts;
using System.Collections.Generic;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Laughman.LaughmanCode.Mechs;

// 借用（借用关键词）：挂在被借走机器人身上的 debuff。
// 层数 = 剩余借用回合数。被借用期间该机器人不行动、不参与屏卫结算/共享受伤（判断见协调器）。
// 每个玩家回合开始时自动 -1，归零移除。
public class BorrowedPower : LaughmanPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // 在玩家回合开始时递减借用回合。归零后移除，机器人当回合即可恢复正常行动。
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
