using BaseLib.Abstracts;
using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Cards;

// ============ 普通 Common ============

// C17 例行训练：造成伤害，若有机器人则抽 1 张牌。
[Pool(typeof(LaughmanCardPool))]
public class RoutinePractice : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DamageVar(8m, ValueProp.Move) };
    public RoutinePractice() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        await CardPileCmd.Draw(c, 1, Owner);
        if (TeamMemberUtils.AliveMechs(Owner).Count > 0)
        {
            await CardPileCmd.Draw(c, 1, Owner);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

// C18 调试补刀：低血目标造成更高伤害。
[Pool(typeof(LaughmanCardPool))]
public class DebugFinish : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(8m, ValueProp.Move), new DynamicVar("BigDamage", 12m) };
    public DebugFinish() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        decimal damage = p.Target.CurrentHp * 2 < p.Target.MaxHp ? DynamicVars["BigDamage"].BaseValue : DynamicVars.Damage.BaseValue;
        await DamageCmd.Attack(damage).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["BigDamage"].UpgradeValueBy(4m); }
}

// C19 现场答辩：攻防一体。
[Pool(typeof(LaughmanCardPool))]
public class OnSiteDefense : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(6m, ValueProp.Move), new BlockVar(3m, ValueProp.Move) };
    public OnSiteDefense() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    public override bool GainsBlock => true;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await ApplyEffect(c, p);
        if (await BorrowUtils.TryBorrow(c, Owner, 1))
        {
            await ApplyEffect(c, p);
        }
    }
    private async Task ApplyEffect(PlayerChoiceContext c, CardPlay p)
    {
        if (!p.Target!.IsDead)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        }
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars.Block.UpgradeValueBy(2m); }
}

// ============ 罕见 Uncommon ============

// U19 学校展出：借用 1，借到才获得能量。
[Pool(typeof(LaughmanCardPool))]
public class SchoolExhibition : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Energy", 2m) };
    public SchoolExhibition() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override bool IsPlayable => BorrowUtils.FindBorrowable(Owner) != null;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (await BorrowUtils.TryBorrow(c, Owner, 1))
        {
            await PlayerCmd.GainEnergy(DynamicVars["Energy"].BaseValue, Owner);
        }
    }
    protected override void OnUpgrade() => DynamicVars["Energy"].UpgradeValueBy(1m);
}

// U20 集训封闭：抽牌，借用 1 追加抽牌+能量。消耗。
[Pool(typeof(LaughmanCardPool))]
public class TrainingCamp : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DynamicVar("Draw", 2m), new DynamicVar("BonusDraw", 1m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public TrainingCamp() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars["Draw"].IntValue, Owner);
        if (await BorrowUtils.TryBorrow(c, Owner, 1))
        {
            await CardPileCmd.Draw(c, DynamicVars["BonusDraw"].IntValue, Owner);
            await PlayerCmd.GainEnergy(1m, Owner);
        }
    }
    protected override void OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);
}

// U21 外援教练：全队力量，借用 1 强化。
[Pool(typeof(LaughmanCardPool))]
public class GuestCoach : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DynamicVar("Strength", 2m), new DynamicVar("BorrowStrength", 3m), new DynamicVar("BorrowDex", 1m) };
    public GuestCoach() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override bool IsPlayable => TeamMemberUtils.AliveMechs(Owner).Count > 0;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        bool borrowed = await BorrowUtils.TryBorrow(c, Owner, 1);
        foreach (var mech in TeamMemberUtils.AliveMechs(Owner))
        {
            if (borrowed)
            {
                await PowerCmd.Apply<StrengthPower>(c, mech, DynamicVars["BorrowStrength"].IntValue, Owner.Creature, this);
                await PowerCmd.Apply<DexterityPower>(c, mech, DynamicVars["BorrowDex"].IntValue, Owner.Creature, this);
            }
            else
            {
                await PowerCmd.Apply<StrengthPower>(c, mech, DynamicVars["Strength"].IntValue, Owner.Creature, this);
            }
        }
    }
    protected override void OnUpgrade() { DynamicVars["BorrowStrength"].UpgradeValueBy(1m); DynamicVars["BorrowDex"].UpgradeValueBy(1m); }
}

// U22 交叉火力：借用提供额外波次，击杀继续连锁。
[Pool(typeof(LaughmanCardPool))]
public class BroadcastCut : LaughmanCard
{
    private const int MaxWaves = 100;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(7m, ValueProp.Move) };
    public BroadcastCut() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var state = Owner.Creature.CombatState;
        if (state == null) return;
        // 借用 1：此伤害翻倍。
        decimal damage = DynamicVars.Damage.BaseValue * (await BorrowUtils.TryBorrow(c, Owner, 1) ? 2m : 1m);
        // 每有一名敌人被此牌击杀，就重复一次（参考回响斩击）。
        int waves = 0;
        bool repeat = true;
        while (repeat && waves < MaxWaves && state.HittableEnemies.Any(e => !e.IsDead))
        {
            waves++;
            int aliveBefore = state.HittableEnemies.Count(e => !e.IsDead);
            await DamageCmd.Attack(damage).FromCard(this, p).TargetingAllOpponents(state).WithHitFx("vfx/vfx_attack_slash").Execute(c);
            int aliveAfter = state.HittableEnemies.Count(e => !e.IsDead);
            repeat = aliveAfter > 0 && aliveAfter < aliveBefore;
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

// U23 曼巴出去！：无人机存活时才能打出，牺牲无人机换 AoE。消耗。
[Pool(typeof(LaughmanCardPool))]
public class MambaOut : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(24m, ValueProp.Move), new DynamicVar("StrengthMultiplier", 8m), new DynamicVar("BonusCap", 24m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public MambaOut() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }
    protected override bool IsPlayable => Owner.Creature.Pets.Any(p => p.Monster is DroneMech && !p.IsDead);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var state = Owner.Creature.CombatState;
        if (state == null) return;
        var drone = Owner.Creature.Pets.FirstOrDefault(pet => pet.Monster is DroneMech && !pet.IsDead);
        int droneStrength = drone?.GetPower<StrengthPower>()?.Amount ?? 0;
        if (drone != null)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), drone, 99m, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature);
        }
        decimal bonus = Math.Min(DynamicVars["BonusCap"].BaseValue, Math.Max(0, droneStrength) * DynamicVars["StrengthMultiplier"].BaseValue);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this, p).TargetingAllOpponents(state).WithHitFx("vfx/vfx_attack_slash").Execute(c);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

// ============ 稀有 Rare ============

// U24 嵌入式比赛：支付 X，横向借用 X 台机甲并逐次获得攻防。
[Pool(typeof(LaughmanCardPool))]
public class EmbeddedContest : LaughmanCard
{
    protected override bool HasEnergyCostX => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(10m, ValueProp.Move), new BlockVar(6m, ValueProp.Move) };
    public override bool GainsBlock => true;
    public EmbeddedContest() : base(-1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (int i = 0; i < ResolveEnergyXValue(); i++)
        {
            if (!await BorrowUtils.TryBorrow(c, Owner, 1)) break;
            if (!p.Target.IsDead)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
            }
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        }
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars.Block.UpgradeValueBy(2m); }
}

// R11 数学建模：抽牌；借用成功后，本次抽到的牌各免费打出一次。
[Pool(typeof(LaughmanCardPool))]
public class MathModeling : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Draw", 3m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public MathModeling() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var drawn = (await CardPileCmd.Draw(c, DynamicVars["Draw"].IntValue, Owner)).ToList();
        if (await BorrowUtils.TryBorrow(c, Owner, 1))
        {
            foreach (var card in drawn)
            {
                card.SetToFreeThisTurn();
            }
        }
    }
    protected override void OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);
}

// R12 ACM 长期集训：支付 X，让一台机器人长期离场，X>=4 时伤害翻倍。
[Pool(typeof(LaughmanCardPool))]
public class AcmRegional : LaughmanCard
{
    protected override bool HasEnergyCostX => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(7m, ValueProp.Move), new DynamicVar("BonusX", 0m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public AcmRegional() : base(-1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var state = Owner.Creature.CombatState;
        if (state == null) return;
        int paidX = ResolveEnergyXValue();
        if (paidX > 0) await BorrowUtils.TryBorrow(c, Owner, paidX);
        decimal damage = DynamicVars.Damage.BaseValue * (paidX + DynamicVars["BonusX"].IntValue);
        if (paidX >= 4) damage *= 2;
        if (damage > 0)
            await DamageCmd.Attack(damage).FromCard(this, p).TargetingAllOpponents(state).WithHitFx("vfx/vfx_attack_slash").Execute(c);
    }
    protected override void OnUpgrade() => DynamicVars["BonusX"].UpgradeValueBy(1m);
}

// R13 机器人大赛决赛：机器人立即行动，借用 1 额外行动一次，升级降费。
[Pool(typeof(LaughmanCardPool))]
public class RoboticsFinals : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Actions", 1m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public RoboticsFinals() : base(3, CardType.Attack, CardRarity.Rare, TargetType.Self) { }
    protected override bool IsPlayable => TeamMemberUtils.AliveMechs(Owner).Any(mech => mech.Monster is MechModel model && !model.IsBorrowed);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;
        int times = DynamicVars["Actions"].IntValue + (await BorrowUtils.TryBorrow(c, Owner, 1) ? 1 : 0);
        for (int i = 0; i < times; i++)
        {
            var mechs = Owner.Creature.Pets
                .Select(pet => pet.Monster as MechModel)
                .Where(m => m != null && !m.Creature.IsDead && !m.IsBorrowed)
                .ToList();
            foreach (var mech in mechs)
            {
                await MechCoordinatorPower.TryPerformAction(c, Owner, combatState, mech!);
            }
        }
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

// R14 世界技能大赛：从 3 张随机罕见/稀有工程师牌中选一张免费打出，借用 1 再选一张。
[Pool(typeof(LaughmanCardPool))]
public class WorldSkills : LaughmanCard
{
    public WorldSkills() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await OfferOne(c);
        if (await BorrowUtils.TryBorrow(c, Owner, 1))
        {
            await OfferOne(c);
        }
    }
    private async Task OfferOne(PlayerChoiceContext c)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;
        var pool = ModelDb.CardPool<LaughmanCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Rarity is CardRarity.Uncommon or CardRarity.Rare && card.Id != Id)
            .ToList();
        if (pool.Count == 0) return;
        var choices = pool.OrderBy(_ => Owner.RunState.Rng.MonsterAi.NextInt(999)).Take(3)
            .Select(canonical => combatState.CreateCard(canonical, Owner)).ToList();
        if (IsUpgraded)
        {
            foreach (var choice in choices)
            {
                CardCmd.Upgrade(choice);
            }
        }
        var chosen = await CardSelectCmd.FromChooseACardScreen(c, choices, Owner, canSkip: false);
        if (chosen != null)
        {
            chosen.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, Owner);
        }
    }
}

// R15 冠军巡游：铺场收割大攻击，借用 1 改为 AoE。消耗。
[Pool(typeof(LaughmanCardPool))]
public class ChampionParade : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(20m, ValueProp.Move), new DynamicVar("PerMech", 6m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public ChampionParade() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var state = Owner.Creature.CombatState;
        int mechCount = TeamMemberUtils.AliveMechs(Owner).Count;
        decimal damage = DynamicVars.Damage.BaseValue + DynamicVars["PerMech"].BaseValue * mechCount;
        if (await BorrowUtils.TryBorrow(c, Owner, 1) && state != null)
        {
            await DamageCmd.Attack(damage).FromCard(this, p).TargetingAllOpponents(state).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        }
        else
        {
            await DamageCmd.Attack(damage).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}

// R16 熬夜调车：0 费透支攻击（对齐熬夜形态）。失 1 血、造成伤害，并给随机一辆吃力量的车 +1 力（调车）。借用 1：重复一次。
[Pool(typeof(LaughmanCardPool))]
public class AllNighterTuning : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DamageVar(6m, ValueProp.Move) };
    public AllNighterTuning() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await TuneOnce(c, p, gainEnergy: true);
        // 借用 1：重复一次（再失血、再打、再调一辆车）。
        if (await BorrowUtils.TryBorrow(c, Owner, 1))
        {
            await TuneOnce(c, p, gainEnergy: !Config.WeakHelper.IsWeak);
        }
    }
    private async Task TuneOnce(PlayerChoiceContext c, CardPlay p, bool gainEnergy)
    {
        if (gainEnergy)
        {
            await PlayerCmd.GainEnergy(1m, Owner);
        }
        await CreatureCmd.Damage(c, Owner.Creature, 1m, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature);
        if (p.Target != null && !p.Target.IsDead)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        }
        var car = TeamMemberUtils.RandomStrengthMech(Owner);
        if (car != null)
        {
            await PowerCmd.Apply<StrengthPower>(c, car, 1m, Owner.Creature, this);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

// R17 技术暂停：为主角争取格挡，并抢修生命比例最低的机甲。
[Pool(typeof(LaughmanCardPool))]
public class TacticalTimeout : LaughmanCard, IOwnedMechTargetingCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(12m, ValueProp.Move), new DynamicVar("Heal", 8m), new DynamicVar("Plating", 4m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public TacticalTimeout() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly) { }
    protected override bool IsPlayable => TeamMemberUtils.AliveMechs(Owner).Count > 0;
    public override bool GainsBlock => true;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        var mech = p.Target;
        if (mech != null)
        {
            await CreatureCmd.Heal(mech, DynamicVars["Heal"].BaseValue);
            await PowerCmd.Apply<PlatingPower>(c, mech, DynamicVars["Plating"].IntValue, Owner.Creature, this);
        }
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); DynamicVars["Heal"].UpgradeValueBy(4m); DynamicVars["Plating"].UpgradeValueBy(2m); }
}

// R18 赛制改革：借用触发时抽牌+格挡+提前归队。
[Pool(typeof(LaughmanCardPool))]
public class RuleOverhaul : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new BlockVar(1m, ValueProp.Move) };
    public RuleOverhaul() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var power = await PowerCmd.Apply<RuleOverhaulPower>(c, Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.BlockAmount = DynamicVars.Block.IntValue;
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);
}

// R21 单刀直入：借用步兵突进，步兵承受可被防护减免的赛场伤害。消耗。
[Pool(typeof(LaughmanCardPool))]
public class MvpOfTheMatch : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(16m, ValueProp.Move), new DynamicVar("InfantryDamage", 13m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public MvpOfTheMatch() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var infantry = await BorrowUtils.TryBorrow(c, Owner, 1,
            mech => mech.GetType() == typeof(InfantryMech) || mech.GetType() == typeof(InfantryNo4Mech));
        decimal damage = DynamicVars.Damage.BaseValue;
        if (infantry != null)
        {
            int infantryStrength = infantry.Creature.GetPower<StrengthPower>()?.Amount ?? 0;
            damage = DynamicVars.Damage.BaseValue * 2 + infantryStrength;
            await CreatureCmd.Damage(c, infantry.Creature, DynamicVars["InfantryDamage"].BaseValue,
                ValueProp.Unpowered, Owner.Creature);
        }
        await DamageCmd.Attack(damage).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}

// C22 召回通知：基础归队循环牌。
[Pool(typeof(LaughmanCardPool))]
public class RecallNotice : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Return", 1m), new DynamicVar("Draw", 1m) };
    public RecallNotice() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override bool IsPlayable => BorrowUtils.FindBorrowed(Owner) != null;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await BorrowUtils.TryReturn(c, Owner, DynamicVars["Return"].IntValue);
        await CardPileCmd.Draw(c, DynamicVars["Draw"].IntValue, Owner);
    }
    protected override void OnUpgrade() => DynamicVars["Return"].UpgradeValueBy(1m);
}

// U26 紧急召回：深度归队并让目标获得小陀螺。
[Pool(typeof(LaughmanCardPool))]
public class EmergencyRecall : LaughmanCard, IOwnedMechTargetingCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Return", 3m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public EmergencyRecall() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly) { }
    public bool IsAllowedMechTarget(Creature target) => target.Monster is MechModel mech && mech.IsBorrowed;
    protected override bool IsPlayable => BorrowUtils.FindBorrowed(Owner) != null;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var mech = (MechModel)p.Target.Monster!;
        if (await BorrowUtils.TryReturn(c, Owner, mech, DynamicVars["Return"].IntValue))
            await PowerCmd.Apply<GyroSpinPower>(c, p.Target, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

// R20 赛前调度会议：分三次归队，并按完全归队的机器人数量抽牌。
[Pool(typeof(LaughmanCardPool))]
public class PreMatchDispatchMeeting : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Repeats", 3m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public PreMatchDispatchMeeting() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override bool IsPlayable => BorrowUtils.FindBorrowed(Owner) != null;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        int returned = await BorrowUtils.ReturnRepeated(c, Owner, DynamicVars["Repeats"].IntValue);
        if (returned > 0) await CardPileCmd.Draw(c, returned, Owner);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

// C23 招新展示：低伤害借用牌，借用成功时增加随机队员。
[Pool(typeof(LaughmanCardPool))]
public class RecruitmentShowcase : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(4m, ValueProp.Move), new DynamicVar("Members", 1m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public RecruitmentShowcase() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        if (await BorrowUtils.TryBorrow(c, Owner, 1))
            await TeamMemberUtils.AddRandom(c, Owner, DynamicVars["Members"].IntValue);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars["Members"].UpgradeValueBy(1m); }
}

// R21 散伙饭：借用一台机甲，将所有队员一次性转化为伤害。
[Pool(typeof(LaughmanCardPool))]
public class FarewellDinner : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("PerMember", 7m)
    };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public FarewellDinner() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        int members = 0;
        if (await BorrowUtils.TryBorrow(c, Owner, 1))
        {
            members = await TeamMemberUtils.ClearAll(c, Owner);
        }
        decimal damage = DynamicVars.Damage.BaseValue + members * DynamicVars["PerMember"].BaseValue;
        await DamageCmd.Attack(damage).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["PerMember"].UpgradeValueBy(2m);
    }
}

// U27 清退通知：清退最高层队员，以烧牌风险换取能量。
[Pool(typeof(LaughmanCardPool))]
public class DismissalNotice : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DynamicVar("Energy", 2m), new DynamicVar("Cards", 2m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public DismissalNotice() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override bool IsPlayable => TeamMemberUtils.HasAny(Owner);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (!await TeamMemberUtils.ReduceHighest(c, Owner))
        {
            return;
        }
        await PlayerCmd.GainEnergy(DynamicVars["Energy"].BaseValue, Owner);
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, DynamicVars["Cards"].IntValue);
        foreach (var card in await CardSelectCmd.FromHand(c, Owner, prefs, null, this))
            await CardCmd.Exhaust(c, card);
    }
    protected override void OnUpgrade() { DynamicVars["Energy"].UpgradeValueBy(1m); DynamicVars["Cards"].UpgradeValueBy(1m); }
}

// U28 机甲冷却：借用一台机甲，定向将弃牌堆中的牌置于抽牌堆顶。
[Pool(typeof(LaughmanCardPool))]
public class MechCooldown : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(8m, ValueProp.Move), new DynamicVar("Cards", 2m) };
    public override bool GainsBlock => true;
    public MechCooldown() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (!await BorrowUtils.TryBorrow(c, Owner, 1)) return;
        for (int i = 0; i < DynamicVars["Cards"].IntValue; i++)
        {
            var discard = PileType.Discard.GetPile(Owner);
            if (discard.Cards.Count == 0) break;

            // 完全复用原版「宇宙冷漠」的单选流程；多张效果通过重复单选实现，
            // 避免弃牌堆网格的多选模式在确认后无法正常关闭。
            var selected = (await CardSelectCmd.FromSimpleGrid(
                c, discard.Cards, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();
            if (selected?.Pile?.Type is PileType.Draw or PileType.Discard)
            {
                await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
            }
        }
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(2m); DynamicVars["Cards"].UpgradeValueBy(1m); }
}

// U29 集火指令：造成伤害并标记目标，使其被机甲优先攻击且受到机甲伤害 +1。
[Pool(typeof(LaughmanCardPool))]
public class CollectiveFire : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(7m, ValueProp.Move), new DynamicVar("Turns", 2m) };
    public CollectiveFire() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        if (!p.Target.IsDead)
        {
            await PowerCmd.Apply<FocusFirePower>(c, p.Target, DynamicVars["Turns"].IntValue, Owner.Creature, this);
        }
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(4m); DynamicVars["Turns"].UpgradeValueBy(2m); }
}

// R21 我即浪潮：召唤英雄并进入浪潮模式（全体攻击 + 每回合行动两次）。
// 行动后自我借用，归队效果可以跳过休息窗口。部署牌始终消耗，升级提高生存能力。
[Pool(typeof(LaughmanCardPool))]
public class IAmTheWave : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("MechHp", 25m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public IAmTheWave() : base(3, CardType.Attack, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var creature = await MechManager.SummonMech<HeroMech>(Owner, DynamicVars["MechHp"].IntValue);
        var hero = creature?.Monster as HeroMech
            ?? Owner.Creature.Pets.Select(pet => pet.Monster as HeroMech).FirstOrDefault(m => m != null && !m.Creature.IsDead);
        // 进入浪潮模式：英雄每回合攻击所有敌人两次，行动后自我借用隐身一回合。
        hero?.EnableTidal();
    }
    protected override void OnUpgrade() => DynamicVars["MechHp"].UpgradeValueBy(10m);
}

// R22 新约无人机：召唤新约无人机（偷力量/双方护盾/自残血/攻击0×5）。消耗。
[Pool(typeof(LaughmanCardPool))]
public class NewCovenantDrone : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("MechHp", 9m), new DynamicVar("Shield", 5m), new DynamicVar("StrengthSteal", 3m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public NewCovenantDrone() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var mech = await MechManager.SummonMech<CovenantDroneMech>(Owner, DynamicVars["MechHp"].IntValue);
        if (mech?.Monster is CovenantDroneMech drone)
        {
            drone.ShieldAmount = DynamicVars["Shield"].IntValue;
            drone.StrengthSteal = DynamicVars["StrengthSteal"].IntValue;
        }
    }
    protected override void OnUpgrade() { DynamicVars["Shield"].UpgradeValueBy(2m); DynamicVars["StrengthSteal"].UpgradeValueBy(1m); }
}

// R23 信火一体飞镖：召唤飞镖，蓄力回合对生命最高敌人施加集火标记。消耗。
[Pool(typeof(LaughmanCardPool))]
public class BeaconDart : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("MechHp", 1m), new DynamicVar("ChargeTurns", 2m), new DynamicVar("BurstDamage", 25m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public BeaconDart() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var mech = await MechManager.SummonMech<DartBotMech>(Owner, DynamicVars["MechHp"].IntValue);
        if (mech?.Monster is DartBotMech dart)
        {
            dart.ChargeTurns = DynamicVars["ChargeTurns"].IntValue;
            dart.BurstDamage = DynamicVars["BurstDamage"].IntValue;
            dart.MarksWhileCharging = true;
        }
    }
    protected override void OnUpgrade() { DynamicVars["MechHp"].UpgradeValueBy(1m); DynamicVars["ChargeTurns"].UpgradeValueBy(-1m); }
}

// R24 精密工程：召唤工程（同工程部署），并使存活工程期间你的 X 费牌 X 值 +1/+2。
[Pool(typeof(LaughmanCardPool))]
public class PrecisionEngineering : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("MechHp", 14m), new DynamicVar("Block", 4m), new DynamicVar("Gold", 2m), new DynamicVar("Actions", 6m), new DynamicVar("XBonus", 1m) };
    public PrecisionEngineering() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var mech = await MechManager.SummonEngineer(Owner, DynamicVars["MechHp"].IntValue, preferPrecision: true);
        if (mech?.Monster is EngineerMech engineer)
        {
            engineer.BlockPerTurn = DynamicVars["Block"].IntValue;
            engineer.GoldPerTurn = DynamicVars["Gold"].IntValue;
            engineer.MaxActions = DynamicVars["Actions"].IntValue;
        }
        var power = await PowerCmd.Apply<PrecisionEngineeringPower>(c, Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.Increase = Math.Max(power.Increase, DynamicVars["XBonus"].IntValue);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["MechHp"].UpgradeValueBy(3m);
        DynamicVars["Block"].UpgradeValueBy(3m);
        DynamicVars["Gold"].UpgradeValueBy(1m);
        DynamicVars["Actions"].UpgradeValueBy(2m);
        DynamicVars["XBonus"].UpgradeValueBy(Config.WeakHelper.V(0m, 1m));
    }
}

// 先古卡 冲击UL（UL = 3v3 中等赛事：英雄/步兵/哨兵）：
// 4 费·先古·技能·固有·消耗。将本回合免费的 3号步兵部署 / 英雄部署 / 趴窝哨兵 各一张加入手牌。
// 升级后给的三张也是升级版。可由古老牙齿（3号步兵部署→本牌）与尘封魔典获得（见 DustyTomePatch）。
[Pool(typeof(LaughmanCardPool))]
public class ImpactUL : LaughmanCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Innate, CardKeyword.Exhaust };
    public ImpactUL() : base(4, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    // 每场战斗第一次打出时费用为 3：使用幂等的绝对费用上限，避免未打出时跨战斗叠加减费。
    // 修正会在打出后失效；被复制/回收再次打出则需付全额 4 费。
    public override Task BeforeCombatStart()
    {
        EnergyCost.SetUntilPlayed(3, reduceOnly: true);
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;
        var cards = new CardModel[]
        {
            combatState.CreateCard<InfantrySummon>(Owner),
            combatState.CreateCard<HeroDeployment>(Owner),
            combatState.CreateCard<StalledSentinel>(Owner)
        };
        foreach (var card in cards)
        {
            if (IsUpgraded) CardCmd.Upgrade(card);
            card.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }
}

// R25 天眼雷达：回合结束时，攻击意图敌人获得镣铐，否则获得持续夹击。
[Pool(typeof(LaughmanCardPool))]
public class SkyEyeRadar : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("Shackles", 2m), new DynamicVar("Flanking", 0m) };
    public SkyEyeRadar() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var power = await PowerCmd.Apply<TianyanRadarPower>(c, Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) { power.ShacklesAmount = DynamicVars["Shackles"].IntValue; power.FlankingAmount = DynamicVars["Flanking"].IntValue; }
    }
    protected override void OnUpgrade() { DynamicVars["Shackles"].UpgradeValueBy(1m); DynamicVars["Flanking"].UpgradeValueBy(1m); }
}

// U 专注打击：高基础伤，机甲越多伤害越低（反机甲流补偿，服务技能惩罚/少机甲局）。
[Pool(typeof(LaughmanCardPool))]
public class FocusedStrike : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DamageVar(21m, ValueProp.Move) };
    public FocusedStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        int mechs = TeamMemberUtils.AliveMechs(Owner).Count;
        decimal damage = Math.Max(0m, DynamicVars.Damage.BaseValue - 3m * mechs);
        await DamageCmd.Attack(damage).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}

// U 测试打击：低费小伤，然后最近部署的机器人立即行动，并优先攻击该目标。
[Pool(typeof(LaughmanCardPool))]
public class CoordinatedStrike : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DamageVar(6m, ValueProp.Move) };
    public CoordinatedStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;
        // 最近部署的机器人 = 宠物列表中最后加入且存活、未借用的机甲。
        var mech = Owner.Creature.Pets
            .Select(pet => pet.Monster as MechModel)
            .Where(m => m != null && !m.Creature.IsDead && !m.IsBorrowed)
            .LastOrDefault();
        if (mech != null)
        {
            mech.ForcedAttackTarget = p.Target;
            try
            {
                await MechCoordinatorPower.TryPerformAction(c, Owner, combatState, mech);
            }
            finally
            {
                mech.ForcedAttackTarget = null;
            }
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
