using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 英雄机甲：输出核心，无屏卫（被保护对象）。
//  - 平射（默认）：每回合单体高伤。
//  - 吊射（由 U5 吊射指令切换）：休息一回合 → 下回合放大招，循环。
public class HeroMech : MechModel, IRevivableMech
{
    private const int PlainDamage = 12;

    // 吊射模式与其伤害（0 表示未启用吊射，仍用平射）。
    public bool LobFireMode { get; private set; }
    public int LobFireDamage { get; private set; }

    // 浪潮模式（由「我即浪潮」切换）：攻击改为全体，且每回合行动两次。
    public bool TidalMode { get; private set; }

    // 吊射循环状态：true = 本回合休息（蓄力），下回合开火。
    private bool _resting;

    // 浪潮模式下由「我即浪潮」借用 payoff 设置：本回合休息一次（这台英雄很累了）。
    private bool _tidalRestNextTurn;

    // 由 U5 卡调用：切换为吊射模式并设定大招伤害。
    public void EnableLobFire(int damage)
    {
        LobFireMode = true;
        LobFireDamage = damage;
        _resting = true; // 切换后先蓄力一回合。
    }

    // 由「我即浪潮」调用：切换为浪潮模式（全体攻击 + 每回合行动两次）。
    public void EnableTidal()
    {
        TidalMode = true;
    }

    // 由「我即浪潮」借用 payoff 调用：让这台英雄下个自己的行动回合休息一次。
    public void RequestTidalRest()
    {
        _tidalRestNextTurn = true;
    }

    public override async Task PerformTurn(Player owner, ICombatState combatState)
    {
        if (LobFireMode)
        {
            // 吊射：休息 → 开火 交替。
            if (_resting)
            {
                _resting = false;
            }
            else
            {
                await AttackRandomEnemy(owner, combatState, LobFireDamage);
                _resting = true;
            }
            return;
        }

        if (TidalMode)
        {
            // 浪潮：太累时休息一回合。
            if (_tidalRestNextTurn)
            {
                _tidalRestNextTurn = false;
                return;
            }
            // 全体攻击，行动两次。
            await AttackAllEnemies(owner, combatState, PlainDamage);
            await AttackAllEnemies(owner, combatState, PlainDamage);
            return;
        }

        await AttackRandomEnemy(owner, combatState, PlainDamage);
    }

    public override void RefreshIntent(Player owner, ICombatState combatState)
    {
        if (LobFireMode)
        {
            // 吊射：休息回合显示 sleep，开火回合显示大招意图。
            if (_resting)
            {
                ShowIntent(new SleepIntent());
            }
            else
            {
                ShowIntent(new FixedAttackIntent(LobFireDamage));
            }
            return;
        }

        if (TidalMode)
        {
            if (_tidalRestNextTurn)
            {
                ShowIntent(new SleepIntent());
            }
            else
            {
                // 全体、两段。
                ShowIntent(new FixedAttackIntent(PlainDamage, 2));
            }
            return;
        }

        ShowIntent(new FixedAttackIntent(PlainDamage));
    }
}
