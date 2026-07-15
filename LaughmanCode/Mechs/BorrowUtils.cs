using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Laughman.LaughmanCode.Mechs;

// 借用关键词工具：借用一台“当前未被借用”的机器人。
// 默认自动选择顺序（尽量不借核心输出）：飞镖 > 无人机 > 工程 > 哨兵 > 步兵 > 英雄。
public static class BorrowUtils
{
    private static int Priority(MechModel mech) => mech switch
    {
        DartBotMech => 0,
        DroneMech => 1,
        EngineerMech => 2,
        SentinelMech => 3,
        InfantryNo4Mech => 4,
        InfantryMech => 5,
        HeroMech => 6,
        _ => 7
    };

    // 找到一台可借（存活且未被借用）的机器人。
    public static MechModel? FindBorrowable(Player owner)
        => FindBorrowable(owner, _ => true);

    public static MechModel? FindBorrowable(Player owner, Func<MechModel, bool> filter)
    {
        return owner.Creature.Pets
            .Select(p => p.Monster as MechModel)
            .Where(m => m != null && !m.Creature.IsDead && !m.IsBorrowed && filter(m))
            .OrderBy(m => Priority(m!))
            .FirstOrDefault();
    }

    // 尝试借用一台机器人 turns 回合。成功返回 true（后半段应触发）。
    public static async Task<bool> TryBorrow(PlayerChoiceContext context, Player owner, int turns)
        => await TryBorrow(context, owner, turns, _ => true) != null;

    public static async Task<MechModel?> TryBorrow(
        PlayerChoiceContext context, Player owner, int turns, Func<MechModel, bool> filter)
    {
        var mech = FindBorrowable(owner, filter);
        if (mech == null)
        {
            return null;
        }
        await PowerCmd.Apply<BorrowedPower>(context, mech.Creature, turns, owner.Creature, null);
        RefreshBorrowVisual(owner, mech);

        // 借用成功后触发「赛制改革」payoff。
        if (owner.Creature.HasPower<RuleOverhaulPower>())
        {
            await owner.Creature.GetPower<RuleOverhaulPower>()!.OnBorrow(context, owner, mech);
        }

        // 借用成功后触发「借调防线」遗物（每场首次借用得格挡）。
        var defenseLine = owner.GetRelic<Laughman.LaughmanCode.Relics.BorrowedDefenseLine>();
        if (defenseLine != null)
        {
            await defenseLine.OnBorrow(context, owner);
        }
        return mech;
    }

    public static MechModel? FindBorrowed(Player owner) =>
        owner.Creature.Pets
            .Select(p => p.Monster as MechModel)
            .Where(m => m != null && !m.Creature.IsDead && m.IsBorrowed)
            .OrderByDescending(m => m!.BorrowedTurns)
            .ThenBy(m => Priority(m!))
            .FirstOrDefault();

    public static async Task<(MechModel? Mech, bool FullyReturned)> TryReturn(
        PlayerChoiceContext context, Player owner, int amount)
    {
        var borrowed = FindBorrowed(owner);
        if (borrowed?.Creature.GetPower<BorrowedPower>() is { } power)
        {
            bool fullyReturned = power.Amount <= amount;
            if (fullyReturned)
            {
                await PowerCmd.Remove(power);
            }
            else
            {
                await PowerCmd.ModifyAmount(context, power, -amount, null, null);
            }
            RefreshBorrowVisual(owner, borrowed);
            return (borrowed, fullyReturned);
        }
        return (null, false);
    }

    public static async Task<bool> TryReturn(
        PlayerChoiceContext context, Player owner, MechModel mech, int amount)
    {
        if (mech.Creature.GetPower<BorrowedPower>() is not { } power)
        {
            return false;
        }
        if (power.Amount <= amount)
        {
            await PowerCmd.Remove(power);
        }
        else
        {
            await PowerCmd.ModifyAmount(context, power, -amount, null, null);
        }
        RefreshBorrowVisual(owner, mech);
        return true;
    }

    public static async Task<int> ReturnRepeated(PlayerChoiceContext context, Player owner, int repeats)
    {
        int fullyReturned = 0;
        for (int i = 0; i < repeats; i++)
        {
            var result = await TryReturn(context, owner, 1);
            if (result.Mech == null)
            {
                break;
            }
            if (result.FullyReturned)
            {
                fullyReturned++;
            }
        }
        return fullyReturned;
    }

    // 兼容现有时间管理大师：减少当前最高的一层借用。
    public static async Task ReduceOneBorrow(PlayerChoiceContext context, Player owner) =>
        _ = await TryReturn(context, owner, 1);

    private static void RefreshBorrowVisual(Player owner, MechModel mech)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState != null && !mech.Creature.IsDead)
        {
            mech.RefreshIntent(owner, combatState);
        }
    }
}
