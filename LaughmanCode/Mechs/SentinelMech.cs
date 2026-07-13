using Laughman.LaughmanCode.Config;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 哨兵机甲：守家支援，拥有屏卫。普通模式每回合给玩家格挡 + 小攻击。
// AI 哨兵能够单走，仍使用原有的自格挡/小陀螺或多段攻击逻辑。
public class SentinelMech : MechModel, IRevivableMech
{
    private static int SelfBlock => WeakHelper.V(5, 7);
    private const int AttackDamage = 1;

    public override bool IsGuard => true;

    public override async Task PerformTurn(Player owner, ICombatState combatState)
    {
        if (Creature.HasPower<AiSentinelPower>())
        {
            var ai = Creature.GetPower<AiSentinelPower>();
            if (ai == null)
            {
                return;
            }
            bool enemyAttacks = combatState.HittableEnemies
                .Any(e => !e.IsDead && e.Monster?.NextMove.Intents.Any(i => i is AttackIntent) == true);
            if (enemyAttacks)
            {
                await GiveSelfBlock(ai.BlockAmount);
                await PowerCmd.Apply<GyroSpinPower>(new ThrowingPlayerChoiceContext(), Creature, 1m, Creature, null);
            }
            else
            {
                await AttackRandomEnemy(owner, combatState, 5, ai.HitCount);
            }
            return;
        }

        await GivePlayerBlock(owner, SelfBlock);
        await AttackRandomEnemy(owner, combatState, AttackDamage);
    }

    public override void RefreshIntent(Player owner, ICombatState combatState)
    {
        if (Creature.HasPower<AiSentinelPower>())
        {
            var ai = Creature.GetPower<AiSentinelPower>();
            if (ai == null)
            {
                ShowIntent(new DefendIntent(), new FixedAttackIntent(AttackDamage));
                return;
            }
            bool enemyAttacks = combatState.HittableEnemies
                .Any(e => !e.IsDead && e.Monster?.NextMove.Intents.Any(i => i is AttackIntent) == true);
            if (enemyAttacks)
            {
                ShowIntent(new DefendIntent(), new BuffIntent());
            }
            else
            {
                ShowIntent(new FixedAttackIntent(5, ai.HitCount));
            }
            return;
        }
        ShowIntent(new DefendIntent(), new FixedAttackIntent(AttackDamage));
    }
}
