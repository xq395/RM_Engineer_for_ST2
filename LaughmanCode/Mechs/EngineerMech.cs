using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 工程机甲：经济 / 增益，无屏卫（被保护对象）。
// 每回合给玩家格挡 + 金币，并给随机一台机器人 +1 力量。
public class EngineerMech : MechModel
{
    private const int StrengthPerTurn = 1;

    public int BlockPerTurn { get; set; } = 4;
    public int GoldPerTurn { get; set; } = 5;
    public int MaxActions { get; set; } = int.MaxValue;
    private int _actionsTaken;

    public override async Task PerformTurn(Player owner, ICombatState combatState)
    {
        if (_actionsTaken >= MaxActions)
        {
            return;
        }
        _actionsTaken++;

        await GivePlayerBlock(owner, BlockPerTurn);
        await PlayerCmd.GainGold(GoldPerTurn, owner);

        // 给随机一台机器人 +1 力量（含自己）。
        var mechs = owner.Creature.Pets.Where(p => p.Monster is MechModel && !p.IsDead).ToList();
        if (mechs.Count > 0)
        {
            var target = mechs[owner.RunState.Rng.MonsterAi.NextInt(mechs.Count)];
            await PowerCmd.Apply<StrengthPower>(
                new ThrowingPlayerChoiceContext(), target, StrengthPerTurn, Creature, null);
        }
    }

    public override void RefreshIntent(Player owner, ICombatState combatState)
    {
        if (_actionsTaken >= MaxActions)
        {
            ShowIntent(new SleepIntent());
            return;
        }
        ShowIntent(new BuffIntent(), new DefendIntent());
    }
}
