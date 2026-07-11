using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 无人机：多段输出，飞行（共享伤害减半），无屏卫。
// 每回合扣玩家 1 金币（运维成本，金币不足由协调器改为晕眩）。
// 多段攻击：每段 DamagePerHit 点，共 HitCount 次（召唤时由卡设定：基础 3 次 / 升级 5 次）。
public class DroneMech : MechModel
{
    private const int DamagePerHit = 2;
    private const int UpkeepGoldCost = 1;

    // 多段次数，召唤时由 DroneDeployment 设定。
    public int HitCount { get; set; } = 3;

    public override bool IsFlying => true;
    public override int UpkeepGold => UpkeepGoldCost;

    public override async Task PerformTurn(Player owner, ICombatState combatState)
    {
        await AttackRandomEnemy(owner, combatState, DamagePerHit, HitCount);
    }

    public override void RefreshIntent(Player owner, ICombatState combatState)
    {
        ShowIntent(new FixedAttackIntent(DamagePerHit, HitCount));
    }
}
