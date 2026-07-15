using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 英雄机甲：输出核心，无屏卫（被保护对象）。
//  - 平射（默认）：每回合单体高伤。
//  - 吊射（由 U5 吊射指令切换）：立即放大招 → 下回合休息，循环。
public class HeroMech : MechModel, IRevivableMech
{
    private static int PlainDamage => Laughman.LaughmanCode.Config.WeakHelper.V(9, 11);

    // 英雄自身攻击时的力量倍率：平射/浪潮双倍，吊射四倍。
    // powered attack 已自动含 1 倍力量，这里把额外的 (倍率-1) 倍力量加进伤害里。
    private decimal BonusStrengthDamage(decimal multiplier)
    {
        decimal strength = Creature.GetPower<StrengthPower>()?.Amount ?? 0m;
        if (strength <= 0m) return 0m;
        return strength * (multiplier - 1m);
    }

    // 吊射模式与其伤害（0 表示未启用吊射，仍用平射）。
    public bool LobFireMode { get; private set; }
    public int LobFireDamage { get; private set; }

    // 浪潮模式（由「我即浪潮」切换）：攻击改为全体，且每回合行动两次。
    public bool TidalMode { get; private set; }

    // 吊射循环状态：true = 本回合休息，下回合开火。
    private bool _resting;



    // 由 U5 卡调用：切换为吊射模式并设定大招伤害。
    public void EnableLobFire(int damage)
    {
        LobFireMode = true;
        LobFireDamage = damage;
        _resting = false; // 切换后的首次行动立即开火。
    }

    // 由「我即浪潮」调用：切换为浪潮模式（全体攻击 + 每回合行动两次，行动后自我借用隐身一回合）。
    public void EnableTidal()
    {
        TidalMode = true;
    }

    public override async Task PerformTurn(Player owner, ICombatState combatState)
    {
        if (LobFireMode)
        {
            // 吊射：开火 → 休息 交替。
            if (_resting)
            {
                _resting = false;
            }
            else
            {
                // 吊射：力量四倍。
                await AttackRandomEnemy(
                    owner,
                    combatState,
                    LobFireDamage + BonusStrengthDamage(Laughman.LaughmanCode.Config.WeakHelper.V(3m, 4m)),
                    unblockable: true);
                _resting = true;
            }
            return;
        }

        if (TidalMode)
        {
            // 浪潮：全体攻击两次，然后自我借用隐身。
            // 借用 2 层：本回合末施加，下个玩家回合开始 -1（仍隐身、不行动），
            // 再下个回合开始归零恢复，从而形成攻击一回合、隐身一回合的循环。
            // 浪潮：力量双倍。
            decimal tidalDamage = PlainDamage + BonusStrengthDamage(2m);
            await AttackAllEnemies(owner, combatState, tidalDamage);
            await AttackAllEnemies(owner, combatState, tidalDamage);
            if (!Creature.IsDead && !IsBorrowed)
            {
                await PowerCmd.Apply<BorrowedPower>(new ThrowingPlayerChoiceContext(), Creature, 2m, owner.Creature, null);
            }
            return;
        }

        // 平射：力量双倍。
        await AttackRandomEnemy(owner, combatState, PlainDamage + BonusStrengthDamage(2m));
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
                ShowIntent(new FixedAttackIntent((int)(LobFireDamage + BonusStrengthDamage(Laughman.LaughmanCode.Config.WeakHelper.V(3m, 4m)))));
            }
            return;
        }

        if (TidalMode)
        {
            // 全体、两段。隐身回合由协调器统一显示 Sleep，不在此处处理。
            ShowIntent(new FixedAttackIntent((int)(PlainDamage + BonusStrengthDamage(2m)), 2));
            return;
        }

        ShowIntent(new FixedAttackIntent((int)(PlainDamage + BonusStrengthDamage(2m))));
    }
}
