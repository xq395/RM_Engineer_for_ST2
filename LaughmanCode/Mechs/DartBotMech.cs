using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 飞镖机器人：蓄力玻璃炮，无屏卫。它本体是发射器，不具有飞行。
// 行动链：休眠若干回合，开火 ×4（每次 BurstDamage），之后持续休息。
public class DartBotMech : MechModel
{
    private const int BurstCount = 4;

    public int ChargeTurns { get; set; } = 2;
    public int BurstDamage { get; set; } = 25;

    // 信火一体（由「信火一体飞镖」设置）：蓄力（休眠）回合对生命最高的敌人施加 1 回合集火标记。
    public bool MarksWhileCharging { get; set; }

    // 已经历的回合序号（从 0 开始）。
    private int _turn;

    public override async Task PerformTurn(Player owner, ICombatState combatState)
    {
        int stage = _turn;
        _turn++;

        // 前 ChargeTurns 回合蓄力，不行动。
        if (stage < ChargeTurns)
        {
            if (MarksWhileCharging)
            {
                await MarkHighestHpEnemy(combatState);
            }
            return;
        }
        // 接下来 BurstCount 回合各开火一次。
        if (stage < ChargeTurns + BurstCount)
        {
            await AttackRandomEnemy(owner, combatState, BurstDamage);
        }
        // 之后持续休息。
    }

    // 对当前生命值最高的存活敌人施加 1 回合集火标记。
    private async Task MarkHighestHpEnemy(ICombatState combatState)
    {
        var target = combatState.HittableEnemies
            .Where(e => !e.IsDead)
            .OrderByDescending(e => e.CurrentHp)
            .FirstOrDefault();
        if (target != null)
        {
            await PowerCmd.Apply<FocusFirePower>(new ThrowingPlayerChoiceContext(), target, 1m, Creature, null);
        }
    }

    public override void RefreshIntent(Player owner, ICombatState combatState)
    {
        // 用“下一回合”的阶段来显示意图（PerformTurn 已把 _turn 递增）。
        int next = _turn;
        if (next < ChargeTurns)
        {
            ShowIntent(new SleepIntent());
        }
        else if (next < ChargeTurns + BurstCount)
        {
            ShowIntent(new FixedAttackIntent(BurstDamage));
        }
        else
        {
            ShowIntent(new SleepIntent());
        }
    }
}
