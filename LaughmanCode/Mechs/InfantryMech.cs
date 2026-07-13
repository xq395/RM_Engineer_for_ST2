using Laughman.LaughmanCode.Config;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 步兵机甲：前线屏卫。每回合攻击随机敌人 + 给自己格挡，补充自身承伤池。
public class InfantryMech : MechModel, IRevivableMech
{
    private static int AttackDamage => WeakHelper.V(4, 6);
    private static int BlockPerTurn => WeakHelper.V(3, 4);

    public override bool IsGuard => true;

    public override async Task PerformTurn(Player owner, ICombatState combatState)
    {
        await AttackRandomEnemy(owner, combatState, AttackDamage);
        await GiveSelfBlock(BlockPerTurn);
    }

    public override void RefreshIntent(Player owner, ICombatState combatState)
    {
        ShowIntent(new FixedAttackIntent(AttackDamage), new DefendIntent());
    }
}
