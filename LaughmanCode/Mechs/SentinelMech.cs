using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 哨兵机甲：专职肉盾，拥有屏卫。每回合给自己高格挡（可再生的吸收池）+ 小攻击。
public class SentinelMech : MechModel, IRevivableMech
{
    private const int SelfBlock = 7;
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

        await GiveSelfBlock(SelfBlock);
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
