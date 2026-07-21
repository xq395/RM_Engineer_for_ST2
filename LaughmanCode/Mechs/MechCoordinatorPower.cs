using BaseLib.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using Laughman.LaughmanCode.Extensions;

namespace Laughman.LaughmanCode.Mechs;

// 挂在玩家身上的机甲协调器（每个玩家只应有一个）。集中负责：
//  1) 玩家回合结束：为每台机器人扣运维金币；金币不足则该机器人本回合“晕眩”不行动，
//     否则调用它的 PerformTurn 执行行为，并刷新意图。
//  2) 多屏卫伤害结算：玩家格挡由引擎先扣（本 hook 之前）；随后按召唤顺序逐个结算带屏卫的
//     机器人（先扣其格挡、再扣其血量），全部击破后剩余伤害由玩家 + 无屏卫机器人共同承受
//     （飞行单位减半）。
//
// 之所以集中到一个 power：引擎的 ModifyUnblockedDamageTarget 会遍历所有 hook 监听者，
// 多台机器人各自挂 guard power 会互相抢重定向；且“逐个结算 + 溢出分摊”只能由知道完整
// 屏卫链的单点完成。
public class MechCoordinatorPower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldPlayVfx => false;


    // 玩家回合结束前：驱动所有机器人。
    // 放在回合结束而不是开始，能让当回合部署的机器人立刻产生收益，也让“飞坡”等本回合增益生效。
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        var owner = Owner.PetOwner ?? (Owner.IsPlayer ? Owner.Player : null);
        if (owner == null || side != Owner.Side)
        {
            return;
        }
        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }
        if (CombatManager.Instance == null || CombatManager.Instance.IsEnding)
        {
            return;
        }

        // 雷达必须先读取敌方意图并施加本轮镣铐/夹击，随后所有机甲才能吃到夹击收益。
        if (owner.Creature.GetPower<TianyanRadarPower>() is { } radar)
        {
            await radar.PrepareMechVolley(choiceContext, combatState);
        }

        // 信火一体飞镖在蓄力回合先施加集火，后续机甲才能吃到优先选敌与每段 +1 伤害。
        // 普通飞镖不改变召唤顺序；OrderBy 是稳定排序，其余机甲仍按原部署顺序行动。
        var mechs = GetMechs(owner)
            .OrderBy(mech => mech is DartBotMech { MarksWhileCharging: true } ? 0 : 1)
            .ToList();
        foreach (var mech in mechs)
        {
            if (mech.Creature.IsDead)
            {
                continue;
            }

            await TryPerformAction(choiceContext, owner, combatState, mech);

            // 覆甲结算：机甲是 pet，其身上的原版 PlatingPower 收不到回合 hook
            // （IterateHookListeners 只含正式参战单位的 powers，不含 pet 的），所以覆甲既不会
            // 自己转成格挡、也不会自己递减。这里由协调器在机甲行动后手动补上，还原原版覆甲语义：
            // 每回合给等于当前层数的格挡，然后层数 -1（衰减）。
            if (!mech.Creature.IsDead && !mech.IsBorrowed)
            {
                var platingPower = mech.Creature.GetPower<PlatingPower>();
                int plating = platingPower?.Amount ?? 0;
                if (plating > 0 && platingPower != null)
                {
                    await CreatureCmd.GainBlock(mech.Creature, plating, ValueProp.Unpowered, null);
                    await PowerCmd.Decrement(platingPower);
                }
            }

        }

        // Pet 不会自动收到正式参战单位的临时 Power 清理 hook。
        // 新约无人机的 DarkShackles(临时 -力量) / FlexPotion(临时 +力量) 因此由协调器在整轮
        // 机甲行动后统一结算，不能放在单台机甲行动后，否则后续机甲仍可能读到错误的力量状态。
        //
        // 关键：原版 TemporaryStrengthPower.AfterTurnEnd 会同时做两件事——移除自身，并回滚它当初
        // 施加的 StrengthPower。但 pet 收不到该 hook，之前只 Remove 了临时 Power，却没回滚底层
        // StrengthPower，导致偷取/自增的力量跨回合永久残留。这里补上力量回滚：
        //  - FlexPotion 是 +力量，结算时 -Amount。
        //  - DarkShackles 是 -力量，结算时 +Amount。
        foreach (var mech in GetMechs(owner))
        {
            var ctx = new ThrowingPlayerChoiceContext();
            if (mech.Creature.GetPower<FlexPotionPower>() is { } flex)
            {
                int flexAmount = flex.Amount;
                await PowerCmd.Remove(flex);
                if (flexAmount != 0)
                {
                    await PowerCmd.Apply<StrengthPower>(ctx, mech.Creature, -flexAmount, mech.Creature, null);
                }
            }
            if (mech.Creature.GetPower<DarkShacklesPower>() is { } shackles)
            {
                int shacklesAmount = shackles.Amount;
                await PowerCmd.Remove(shackles);
                if (shacklesAmount != 0)
                {
                    await PowerCmd.Apply<StrengthPower>(ctx, mech.Creature, shacklesAmount, mech.Creature, null);
                }
            }
        }

        // 夹击只服务本轮机甲齐射，避免残留到玩家攻击或错误地在敌方回合计时。
        foreach (var enemy in combatState.HittableEnemies.Where(e => !e.IsDead).ToList())
        {
            if (enemy.GetPower<SustainedFlankingPower>() is { } flanking)
            {
                await PowerCmd.Remove(flanking);
            }
        }
    }

    // 挂上时立刻刷新所有机器人意图。
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        RefreshAllIntents();
        return Task.CompletedTask;
    }

    // 打出任意卡后刷新意图（力量/飞坡/借用等会改变头顶数字）。
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        RefreshAllIntents();
        return Task.CompletedTask;
    }

    // 任意 power 数值变化后刷新意图（力量、飞坡、借用施加/移除等）。
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        RefreshAllIntents();
        return Task.CompletedTask;
    }

    private void RefreshAllIntents()
    {
        var owner = Owner.PetOwner ?? (Owner.IsPlayer ? Owner.Player : null);
        if (owner == null)
        {
            return;
        }
        var combatState = Owner.CombatState;
        if (combatState == null || CombatManager.Instance == null || CombatManager.Instance.IsEnding)
        {
            return;
        }
        foreach (var mech in GetMechs(owner))
        {
            if (mech.Creature.IsDead)
            {
                continue;
            }
            if (mech.IsBorrowed)
            {
                mech.ShowIntent(new SleepIntent());
            }
            else
            {
                mech.RefreshIntent(owner, combatState);
            }
        }
    }

    // ---- 多屏卫伤害结算 ----
    //
    // 此 hook 在玩家格挡已被扣除之后触发（target == 玩家，amount == 破盾后的伤害）。
    // 我们在这里手动逐个结算屏卫机器人，返回值 = 最终仍由玩家承受的伤害。
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 仅处理打向本协调器主人（玩家）的强攻击。
        var owner = Owner.PetOwner ?? (Owner.IsPlayer ? Owner.Player : null);
        if (owner == null || target != owner.Creature)
        {
            return amount;
        }
        if (!props.IsPoweredAttack() || amount <= 0m)
        {
            return amount;
        }

        decimal remaining = amount;

        // 按召唤顺序逐个结算带屏卫的机器人：先扣格挡、再扣血量。
        foreach (var mech in GetMechs(owner))
        {
            if (remaining <= 0m)
            {
                break;
            }
            if (!mech.IsGuard || mech.Creature.IsDead || mech.IsBorrowed)
            {
                continue;
            }

            var guard = mech.Creature;

            // Keep remaining in the attack's original damage units. Mitigation increases how much
            // original damage this guard can absorb, but must not protect later guards or the player.
            decimal multiplier = IncomingDamageMultiplier(owner, guard);
            int effectiveDamage = (int)Math.Floor(remaining * multiplier);
            if (effectiveDamage <= 0)
            {
                remaining = 0m;
                break;
            }
            int effectiveAbsorbed = 0;

            // 先扣屏卫格挡。
            if (guard.Block > 0)
            {
                int absorbed = Math.Min(guard.Block, effectiveDamage);
                guard.LoseBlockInternal(absorbed);
                effectiveDamage -= absorbed;
                effectiveAbsorbed += absorbed;
                if (effectiveDamage <= 0)
                {
                    remaining = 0m;
                    break;
                }
            }

            // 再扣屏卫血量。屏卫存活即拦下整次攻击；死亡时将它实际吸收的有效伤害
            // 除以自身减伤倍率，换算成从原始伤害池中消耗的数值。
            if (guard.CurrentHp > 0)
            {
                var result = guard.LoseHpInternal(effectiveDamage, props);
                effectiveAbsorbed += result.UnblockedDamage;
                if (result.WasTargetKilled)
                {
                    _pendingKills.Add(guard);
                    remaining = Math.Max(0m, remaining - effectiveAbsorbed / multiplier);
                }
                else
                {
                    remaining = 0m;
                    break;
                }
            }
        }

        if (remaining <= 0m)
        {
            return 0m;
        }

        // 所有屏卫击破后：剩余伤害由玩家 + 所有无屏卫机器人共同承受同等伤害（原版多人 AoE 逻辑）。
        // 飞行单位减半。这些机器人在此直接结算，玩家的份额作为返回值交回引擎。
        foreach (var mech in GetMechs(owner))
        {
            if (mech.IsGuard || mech.Creature.IsDead || mech.IsBorrowed)
            {
                continue;
            }
            decimal share = mech.IsFlying ? Math.Floor(remaining / 2m) : remaining;
            share *= IncomingDamageMultiplier(owner, mech.Creature);
            if (share <= 0m)
            {
                continue;
            }
            var result = mech.Creature.LoseHpInternal(share, props);
            if (result.WasTargetKilled)
            {
                _pendingKills.Add(mech.Creature);
            }
        }

        // 返回玩家自己承受的份额。
        return remaining;
    }

    // 在伤害结算收尾阶段统一击杀被屏卫链/共享伤害打死的机器人。
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (_pendingKills.Count == 0)
        {
            return;
        }
        var toKill = _pendingKills.Where(c => c.IsDead).ToList();
        _pendingKills.Clear();
        if (toKill.Count > 0)
        {
            await CreatureCmd.Kill(toKill);
        }
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;

    // 可买活机甲会保留死亡节点；同时保留其全部正负 Power，让买活/基地补给延续死前状态。
    // 再部署走 MechManager 的重置路径，会显式清空这些 Power，视为一台全新机体。
    public override bool ShouldPowerBeRemovedOnDeath(PowerModel power) =>
        power.Owner.Monster is not IRevivableMech;

    // 让可买活的机器人（步兵/英雄/哨兵）死亡后仍留在战斗中，以便 U8 买活复活它们。
    // 其他机器人（工程/无人机/飞镖）死亡即移出。
    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        if (MechManager.IsReplacementDeath(creature))
        {
            return true;
        }
        if (creature.Monster is IRevivableMech)
        {
            return false;
        }
        return true;
    }

    private readonly List<Creature> _pendingKills = new();

    public static async Task<bool> TryPerformAction(
        PlayerChoiceContext choiceContext,
        Player owner,
        ICombatState combatState,
        MechModel mech)
    {
        if (mech.Creature.IsDead)
        {
            return false;
        }
        if (mech.IsBorrowed)
        {
            mech.ShowIntent(new SleepIntent());
            return false;
        }
        if (mech.Creature.IsStunned)
        {
            // Pets are manually driven, so consume the stunned move by restoring their normal intent.
            mech.RefreshIntent(owner, combatState);
            return false;
        }
        if (mech.UpkeepGold > 0)
        {
            if (owner.Gold < mech.UpkeepGold)
            {
                // Upkeep failure skips this action; it must not also consume the next action.
                mech.ShowIntent(new StunIntent());
                return false;
            }
            await PlayerCmd.LoseGold(mech.UpkeepGold, owner, GoldLossType.Spent);
        }

        await mech.PerformTurn(owner, combatState);
        if (!mech.Creature.IsDead && mech.Creature.HasPower<ChampionFormPower>())
        {
            await TeamMemberUtils.TriggerRandom(owner, choiceContext);
        }
        if (!mech.Creature.IsDead)
        {
            mech.RefreshIntent(owner, combatState);
        }
        return true;
    }

    private static decimal IncomingDamageMultiplier(Player owner, Creature mech)
    {
        decimal multiplier = 1m;
        if (mech.HasPower<GyroSpinPower>())
        {
            multiplier *= 0.5m;
        }
        if (mech.HasPower<RampJumpPower>())
        {
            multiplier *= 0.5m;
        }
        if (owner.Creature.GetPower<ExchangeMiningPower>() is { } exchange)
        {
            multiplier *= ExchangeMiningPower.DamageMultiplierForAmount(exchange.Amount);
        }
        return multiplier;
    }

    private static List<MechModel> GetMechs(Player owner)
    {
        var list = new List<MechModel>();
        foreach (var pet in owner.Creature.Pets)
        {
            if (pet.Monster is MechModel mech)
            {
                list.Add(mech);
            }
        }
        return list;
    }
}
