using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 步兵机甲：均衡坦克，拥有屏卫。每回合攻击随机敌人 + 给玩家格挡。
public class InfantryMech : MechModel, IRevivableMech
{
    private const int AttackDamage = 6;
    private const int BlockPerTurn = 4;

    public override bool IsGuard => true;

    public override async Task PerformTurn(Player owner, ICombatState combatState)
    {
        await AttackRandomEnemy(owner, combatState, AttackDamage);
        await GivePlayerBlock(owner, BlockPerTurn);
    }

    public override void RefreshIntent(Player owner, ICombatState combatState)
    {
        ShowIntent(new FixedAttackIntent(AttackDamage), new DefendIntent());
    }
}
