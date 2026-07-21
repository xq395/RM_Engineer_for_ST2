using BaseLib.Abstracts;
using System.Collections.Generic;
using System.Linq;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Laughman.LaughmanCode.Mechs;

// R16 熬夜调车：每回合开始抽 1 张牌；若当前没有车正在借用，则借用 1，
// 借用成功时使随机一台机器人获得 2/3 点力量和 2/3 层覆甲。
public class AllNighterTuningPower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public int StrengthAmount { get; set; } = 2;
    public int PlatingAmount { get; set; } = 2;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || !Owner.IsPlayer || Owner.Player == null)
        {
            return;
        }
        var context = new ThrowingPlayerChoiceContext();
        await CardPileCmd.Draw(context, 1, Owner.Player);

        // 若当前已有车正在借用，则本回合不再借用（避免持续把更多机甲抽离）。
        if (BorrowUtils.FindBorrowed(Owner.Player) != null)
        {
            return;
        }
        if (!await BorrowUtils.TryBorrow(context, Owner.Player, 1))
        {
            return;
        }
        // 借用成功：随机一台机器人获得力量与覆甲。
        var target = TeamMemberUtils.RandomMech(Owner.Player);
        if (target != null)
        {
            if (StrengthAmount > 0)
            {
                await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.StrengthPower>(context, target, StrengthAmount, Owner, null);
            }
            if (PlatingAmount > 0)
            {
                await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.PlatingPower>(context, target, PlatingAmount, Owner, null);
            }
        }
    }
}
