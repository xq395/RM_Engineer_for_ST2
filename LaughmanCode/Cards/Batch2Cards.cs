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

[Pool(typeof(LaughmanCardPool))]
public class MechanicalTryout : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(5m, ValueProp.Move) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public MechanicalTryout() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    public override bool GainsBlock => true;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { await PowerCmd.Apply<MechanicalMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this); await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p); }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

[Pool(typeof(LaughmanCardPool))]
public class ElectronicsTryout : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Plating", 3m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public ElectronicsTryout() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { await PowerCmd.Apply<ElectricalMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this); var target = TeamMemberUtils.RandomMech(Owner); if (target != null) await PowerCmd.Apply<PlatingPower>(c, target, DynamicVars["Plating"].IntValue, Owner.Creature, this); }
    protected override void OnUpgrade() => DynamicVars["Plating"].UpgradeValueBy(2m);
}

[Pool(typeof(LaughmanCardPool))]
public class VisionTryout : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DamageVar(7m, ValueProp.Move) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public VisionTryout() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { ArgumentNullException.ThrowIfNull(p.Target); await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c); await PowerCmd.Apply<VisionMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this); }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

[Pool(typeof(LaughmanCardPool))]
public class HardwareTryout : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("DrawCount", 2m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public HardwareTryout() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { await PowerCmd.Apply<HardwareMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this); await CardPileCmd.Draw(c, DynamicVars["DrawCount"].IntValue, Owner); }
    protected override void OnUpgrade() => DynamicVars["DrawCount"].UpgradeValueBy(1m);
}

[Pool(typeof(LaughmanCardPool))]
public class PartsInspection : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(8m, ValueProp.Move), new DynamicVar("Vulnerable", 1m) };
    public PartsInspection() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { ArgumentNullException.ThrowIfNull(p.Target); await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c); if (TeamMemberUtils.AliveMechs(Owner).Count > 0) await PowerCmd.Apply<VulnerablePower>(c, p.Target, DynamicVars["Vulnerable"].IntValue, Owner.Creature, this); }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["Vulnerable"].UpgradeValueBy(1m); }
}

[Pool(typeof(LaughmanCardPool))]
public class InterferenceTest : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Weak", 1m) };
    public InterferenceTest() : base(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies) { }
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost) { if (card == this && TeamMemberUtils.AliveMechs(Owner).Count == 0) { modifiedCost = Math.Max(0m, originalCost - 1m); return true; } modifiedCost = originalCost; return false; }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { var state = Owner.Creature.CombatState; if (state == null) return; foreach (var e in state.HittableEnemies.Where(e => !e.IsDead)) await PowerCmd.Apply<WeakPower>(c, e, DynamicVars["Weak"].IntValue, Owner.Creature, this); }
    protected override void OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);
}

[Pool(typeof(LaughmanCardPool))]
public class RepairChecklist : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(6m, ValueProp.Move) };
    public RepairChecklist() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    public override bool GainsBlock => true;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (TeamMemberUtils.Amount<MechanicalMemberPower>(Owner) > 0) await TeamMemberUtils.Trigger(Owner, TeamMemberType.Mechanical, 1, c); await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p); }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

[Pool(typeof(LaughmanCardPool))]
public class ClearRoles : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Types", 1m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public ClearRoles() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var entries = new List<(TeamMemberType Type, int Amount)> { (TeamMemberType.Mechanical, TeamMemberUtils.Amount<MechanicalMemberPower>(Owner)), (TeamMemberType.Electrical, TeamMemberUtils.Amount<ElectricalMemberPower>(Owner)), (TeamMemberType.Vision, TeamMemberUtils.Amount<VisionMemberPower>(Owner)), (TeamMemberType.Hardware, TeamMemberUtils.Amount<HardwareMemberPower>(Owner)) };
        int picks = DynamicVars["Types"].IntValue;
        foreach (var entry in entries.OrderBy(e => e.Amount).ThenBy(_ => Owner.RunState.Rng.MonsterAi.NextInt(999)).Take(picks)) await ApplyMember(c, entry.Type);
    }
    private Task ApplyMember(PlayerChoiceContext c, TeamMemberType type) => type switch { TeamMemberType.Mechanical => PowerCmd.Apply<MechanicalMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this), TeamMemberType.Electrical => PowerCmd.Apply<ElectricalMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this), TeamMemberType.Vision => PowerCmd.Apply<VisionMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this), _ => PowerCmd.Apply<HardwareMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this) };
    protected override void OnUpgrade() => DynamicVars["Types"].UpgradeValueBy(1m);
}

[Pool(typeof(LaughmanCardPool))]
public class InfantryNo4 : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("MechHp", 13m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public InfantryNo4() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) => await MechManager.SummonMech<InfantryNo4Mech>(Owner, DynamicVars["MechHp"].IntValue);
    protected override void OnUpgrade() => DynamicVars["MechHp"].UpgradeValueBy(5m);
}

[Pool(typeof(LaughmanCardPool))]
public class PreMatchCalibration : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new BlockVar(8m, ValueProp.Move) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public PreMatchCalibration() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    public override bool GainsBlock => true;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await TeamMemberUtils.TriggerAll(Owner, c);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        EnergyCost.UpgradeBy(-1);
    }
}

[Pool(typeof(LaughmanCardPool))]
public class GyroSpinCommand : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Turns", 1m) };
    public GyroSpinCommand() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override bool IsPlayable => TeamMemberUtils.AliveMechs(Owner).Count > 0;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { foreach (var mech in TeamMemberUtils.AliveMechs(Owner)) await PowerCmd.Apply<GyroSpinPower>(c, mech, DynamicVars["Turns"].IntValue, Owner.Creature, this); }
    protected override void OnUpgrade() => DynamicVars["Turns"].UpgradeValueBy(1m);
}

[Pool(typeof(LaughmanCardPool))]
public class AiSentinel : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(7m, ValueProp.Move), new DynamicVar("MechBlock", 3m), new DynamicVar("HitCount", 5m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public AiSentinel() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.Self) { }
    public override bool GainsBlock => true;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        foreach (var s in Owner.Creature.Pets.Where(pet => pet.Monster is SentinelMech && !pet.IsDead))
        {
            var power = await PowerCmd.Apply<AiSentinelPower>(c, s, 1m, Owner.Creature, this);
            if (power != null) { power.BlockAmount = DynamicVars["MechBlock"].IntValue; power.HitCount = DynamicVars["HitCount"].IntValue; }
        }
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); DynamicVars["MechBlock"].UpgradeValueBy(1m); DynamicVars["HitCount"].UpgradeValueBy(1m); }
}

[Pool(typeof(LaughmanCardPool))]
public class CrashCourse : LaughmanCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public CrashCourse() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var choices = new List<CardModel>
        {
            combatState.CreateCard<MechanicalTraining>(Owner),
            combatState.CreateCard<ElectricalTraining>(Owner),
            combatState.CreateCard<SoftwareTrainingChoice>(Owner)
        };
        if (IsUpgraded)
        {
            foreach (var choice in choices.Where(choice => choice is not SoftwareTrainingChoice))
            {
                CardCmd.Upgrade(choice);
            }
        }

        var chosen = await CardSelectCmd.FromChooseACardScreen(c, choices, Owner, canSkip: false);
        if (chosen is SoftwareTrainingChoice)
        {
            var softwareChoices = new List<CardModel>
            {
                combatState.CreateCard<VisionTraining>(Owner),
                combatState.CreateCard<HardwareTraining>(Owner)
            };
            if (IsUpgraded)
            {
                foreach (var choice in softwareChoices)
                {
                    CardCmd.Upgrade(choice);
                }
            }
            chosen = await CardSelectCmd.FromChooseACardScreen(c, softwareChoices, Owner, canSkip: false);
        }
        if (chosen != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, Owner);
        }
    }
}

public abstract class TeamMemberTraining : LaughmanCard
{
    protected abstract TeamMemberType MemberType { get; }
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Amount", 2m) };

    protected TeamMemberTraining() : base(0, CardType.Power, CardRarity.Token, TargetType.Self) { }

    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => MemberType switch
    {
        TeamMemberType.Mechanical => PowerCmd.Apply<MechanicalMemberPower>(c, Owner.Creature, DynamicVars["Amount"].IntValue, Owner.Creature, this),
        TeamMemberType.Electrical => PowerCmd.Apply<ElectricalMemberPower>(c, Owner.Creature, DynamicVars["Amount"].IntValue, Owner.Creature, this),
        TeamMemberType.Vision => PowerCmd.Apply<VisionMemberPower>(c, Owner.Creature, DynamicVars["Amount"].IntValue, Owner.Creature, this),
        _ => PowerCmd.Apply<HardwareMemberPower>(c, Owner.Creature, DynamicVars["Amount"].IntValue, Owner.Creature, this)
    };

    protected override void OnUpgrade() => DynamicVars["Amount"].UpgradeValueBy(1m);
}

[Pool(typeof(LaughmanCardPool))]
public class MechanicalTraining : TeamMemberTraining
{
    protected override TeamMemberType MemberType => TeamMemberType.Mechanical;
    public MechanicalTraining() { }
}

[Pool(typeof(LaughmanCardPool))]
public class ElectricalTraining : TeamMemberTraining
{
    protected override TeamMemberType MemberType => TeamMemberType.Electrical;
    public ElectricalTraining() { }
}

[Pool(typeof(LaughmanCardPool))]
public class SoftwareTrainingChoice : LaughmanCard
{
    private bool _upgradesChoice;

    public SoftwareTrainingChoice() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var choices = new List<CardModel>
        {
            combatState.CreateCard<VisionTraining>(Owner),
            combatState.CreateCard<HardwareTraining>(Owner)
        };
        if (_upgradesChoice)
        {
            foreach (var choice in choices)
            {
                CardCmd.Upgrade(choice);
            }
        }

        var chosen = await CardSelectCmd.FromChooseACardScreen(c, choices, Owner, canSkip: false);
        if (chosen != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade() => _upgradesChoice = true;
}

[Pool(typeof(LaughmanCardPool))]
public class VisionTraining : TeamMemberTraining
{
    protected override TeamMemberType MemberType => TeamMemberType.Vision;
    public VisionTraining() { }
}

[Pool(typeof(LaughmanCardPool))]
public class HardwareTraining : TeamMemberTraining
{
    protected override TeamMemberType MemberType => TeamMemberType.Hardware;
    public HardwareTraining() { }
}

[Pool(typeof(LaughmanCardPool))]
public class SyncCalibration : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(4m, ValueProp.Move), new DynamicVar("PerMember", 2m) };
    public SyncCalibration() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { ArgumentNullException.ThrowIfNull(p.Target); decimal damage = DynamicVars.Damage.BaseValue + DynamicVars["PerMember"].BaseValue * TotalMembers(); await DamageCmd.Attack(damage).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c); }
    private int TotalMembers() => TeamMemberUtils.Amount<MechanicalMemberPower>(Owner) + TeamMemberUtils.Amount<ElectricalMemberPower>(Owner) + TeamMemberUtils.Amount<VisionMemberPower>(Owner) + TeamMemberUtils.Amount<HardwareMemberPower>(Owner);
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars["PerMember"].UpgradeValueBy(1m); }
}

[Pool(typeof(LaughmanCardPool))]
public class WeakpointMarking : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Vulnerable", 2m), new DynamicVar("Weak", 1m) };
    public WeakpointMarking() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { ArgumentNullException.ThrowIfNull(p.Target); await PowerCmd.Apply<VulnerablePower>(c, p.Target, DynamicVars["Vulnerable"].IntValue, Owner.Creature, this); await PowerCmd.Apply<WeakPower>(c, p.Target, DynamicVars["Weak"].IntValue, Owner.Creature, this); if (TeamMemberUtils.Amount<VisionMemberPower>(Owner) > 0) await TeamMemberUtils.Trigger(Owner, TeamMemberType.Vision, 1, c); }
    protected override void OnUpgrade() { DynamicVars["Vulnerable"].UpgradeValueBy(1m); DynamicVars["Weak"].UpgradeValueBy(1m); }
}

[Pool(typeof(LaughmanCardPool))]
public class PrecisionGuidance : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("MultiplierPct", 25m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public PrecisionGuidance() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override bool IsPlayable => TeamMemberUtils.AliveMechs(Owner).Count > 0;
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        foreach (var mech in TeamMemberUtils.AliveMechs(Owner))
        {
            var power = await PowerCmd.Apply<PrecisionGuidancePower>(c, mech, 1m, Owner.Creature, this);
            if (power != null)
            {
                power.BonusDamage = 0;
                power.DamageMultiplier = 1m + DynamicVars["MultiplierPct"].BaseValue / 100m;
                power.IgnoresBlock = mech.Monster is DartBotMech;
            }
        }
    }
    protected override void OnUpgrade() => DynamicVars["MultiplierPct"].UpgradeValueBy(25m);
}

[Pool(typeof(LaughmanCardPool))]
public class ThanksOpenSource : LaughmanCard
{
    public ThanksOpenSource() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }
        var cards = new List<CardModel>
        {
            combatState.CreateCard<InfantrySummon>(Owner), combatState.CreateCard<SentinelDeployment>(Owner), combatState.CreateCard<HeroDeployment>(Owner),
            combatState.CreateCard<EngineerDeployment>(Owner), combatState.CreateCard<DroneDeployment>(Owner), combatState.CreateCard<SwarmDeployment>(Owner), combatState.CreateCard<InfantryNo4>(Owner)
        }.OrderBy(_ => Owner.RunState.Rng.MonsterAi.NextInt(999)).Take(3).ToList();
        if (IsUpgraded) foreach (var card in cards) CardCmd.Upgrade(card);
        var chosen = await CardSelectCmd.FromChooseACardScreen(c, cards, Owner, canSkip: false);
        if (chosen != null) { chosen.SetToFreeThisTurn(); await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, Owner); }
    }
}

[Pool(typeof(LaughmanCardPool))]
public class PrepReview : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("TriggerCount", 2m) };
    public PrepReview() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var entries = new List<(TeamMemberType Type, int Amount)> { (TeamMemberType.Mechanical, TeamMemberUtils.Amount<MechanicalMemberPower>(Owner)), (TeamMemberType.Electrical, TeamMemberUtils.Amount<ElectricalMemberPower>(Owner)), (TeamMemberType.Vision, TeamMemberUtils.Amount<VisionMemberPower>(Owner)), (TeamMemberType.Hardware, TeamMemberUtils.Amount<HardwareMemberPower>(Owner)) }.Where(e => e.Amount > 0).ToList();
        if (entries.Count == 0)
        {
            var type = (TeamMemberType)Owner.RunState.Rng.MonsterAi.NextInt(4);
            await TeamMemberUtils.Add(c, Owner, type);
            await TeamMemberUtils.Trigger(Owner, type, 1, c);
            return;
        }
        int maxAmount = entries.Max(e => e.Amount);
        var highest = entries.Where(e => e.Amount == maxAmount).ToList();
        var chosen = highest[Owner.RunState.Rng.MonsterAi.NextInt(highest.Count)].Type;
        await TeamMemberUtils.Trigger(Owner, chosen, DynamicVars["TriggerCount"].IntValue, c);
    }
    protected override void OnUpgrade() => DynamicVars["TriggerCount"].UpgradeValueBy(1m);
}

[Pool(typeof(LaughmanCardPool))]
public class AnnualRecruitment : LaughmanCard
{
    private bool _delaysSecond;
    public AnnualRecruitment() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { await PowerCmd.Apply<MechanicalMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this); await PowerCmd.Apply<ElectricalMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this); await PowerCmd.Apply<VisionMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this); await PowerCmd.Apply<HardwareMemberPower>(c, Owner.Creature, 1m, Owner.Creature, this); if (_delaysSecond) await PowerCmd.Apply<AnnualRecruitmentNextPower>(c, Owner.Creature, 1m, Owner.Creature, this); }
    protected override void OnUpgrade() => _delaysSecond = true;
}

[Pool(typeof(LaughmanCardPool))]
public class LoyalGuard : LaughmanCard
{
    protected override bool HasEnergyCostX => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Extra", 0m) };
    public LoyalGuard() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        int times = ResolveEnergyXValue() + DynamicVars["Extra"].IntValue;
        for (int i = 0; i < times; i++)
        {
            var infantry = MechManager.FindMech<InfantryMech>(Owner);
            if (infantry == null)
            {
                await MechManager.SummonMech<InfantryMech>(Owner, IsUpgraded ? 18 : 13);
                continue;
            }
            if (infantry.IsDead)
            {
                continue;
            }
            await CreatureCmd.GainMaxHp(infantry, 6m);
            await PowerCmd.Apply<PlatingPower>(c, Owner.Creature, 1m, Owner.Creature, this);
            await PowerCmd.Apply<StrengthPower>(c, infantry, 1m, Owner.Creature, this);
            await PowerCmd.Apply<DexterityPower>(c, infantry, 1m, Owner.Creature, this);
        }
    }
    protected override void OnUpgrade() => DynamicVars["Extra"].UpgradeValueBy(1m);
}

[Pool(typeof(LaughmanCardPool))]
public class InsomniaForm : LaughmanCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => IsUpgraded ? new[] { CardKeyword.Innate } : Array.Empty<CardKeyword>();
    public InsomniaForm() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { await PowerCmd.Apply<StrengthPower>(c, Owner.Creature, -1m, Owner.Creature, this); await PowerCmd.Apply<DexterityPower>(c, Owner.Creature, -1m, Owner.Creature, this); await PowerCmd.Apply<InsomniaModePower>(c, Owner.Creature, 1m, Owner.Creature, this); }
}

[Pool(typeof(LaughmanCardPool))]
public class PrecisionExchange : LaughmanCard
{
    protected override bool HasEnergyCostX => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Bonus", -1m) };
    public PrecisionExchange() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override bool IsPlayable => Owner.Creature.Pets.Any(p => p.Monster is EngineerMech && !p.IsDead);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { int amount = Math.Max(0, ResolveEnergyXValue() + DynamicVars["Bonus"].IntValue); if (amount > 0) await PowerCmd.Apply<ExchangeMiningPower>(c, Owner.Creature, amount, Owner.Creature, this); }
    protected override void OnUpgrade() => DynamicVars["Bonus"].UpgradeValueBy(1m);
}

// 剑指春茧（先古卡）：只能在三层特殊事件里由「冲击UL」替换升级获得，不进随机先古卡池
// （尘封魔典/古老牙齿只给冲击UL、冠军形态）。
// 检测条件：7 类单体召唤位各需一张——每位由“基础召唤”或“对应异格召唤”任一满足即可。
[Pool(typeof(LaughmanCardPool))]
public class RoadToSpringCocoon : LaughmanCard
{
    // 每个召唤位 = 一组可满足它的卡类型（基础 + 异格）。
    private static readonly Type[][] SummonSlots =
    {
        new[] { typeof(InfantryNo4) },                                  // 步兵位（4号步兵，独立召唤）
        new[] { typeof(SentinelDeployment), typeof(StalledSentinel) },  // 哨兵位（哨兵部署 / 趴窝哨兵）
        new[] { typeof(HeroDeployment), typeof(IAmTheWave) },           // 英雄位（英雄部署 / 我即浪潮）
        new[] { typeof(EngineerDeployment), typeof(PrecisionEngineering) }, // 工程位（工程部署 / 精密工程）
        new[] { typeof(DroneDeployment), typeof(NewCovenantDrone) },    // 无人机位（无人机部署 / 新约无人机）
        new[] { typeof(SwarmDeployment), typeof(BeaconDart) },          // 飞镖位（飞镖部署 / 信火一体飞镖）
        new[] { typeof(RadarLock), typeof(SkyEyeRadar) },               // 雷达位（雷达部署 / 天眼雷达）
    };

    // 打出时可自动施放的部署卡（基础 + 异格全部）。
    private static readonly Type[] PlayableTypes = SummonSlots.SelectMany(s => s).ToArray();

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DynamicVar("Count", 3m) };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Innate, CardKeyword.Exhaust };
    public RoadToSpringCocoon() : base(6, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    protected override bool IsPlayable => HasAllRequiredSummons();

    // 第一次打出时费用 -3（6→3）：每场战斗开始时施加一个“打出即失效”的 -3 费修正。
    public override Task BeforeCombatStart()
    {
        EnergyCost.AddUntilPlayed(-3);
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }
        var candidates = Owner.Deck.Cards
            .Where(card => PlayableTypes.Contains(card.GetType()))
            .OrderBy(_ => Owner.RunState.Rng.MonsterAi.NextInt(999))
            .Take(DynamicVars["Count"].IntValue)
            .ToList();
        foreach (var original in candidates)
        {
            var card = combatState.CreateCard(original.CanonicalInstance, Owner);
            if (!card.IsUpgraded)
            {
                CardCmd.Upgrade(card);
            }
            await CardCmd.AutoPlay(c, card, null, AutoPlayType.Default, skipXCapture: true, skipCardPileVisuals: false);
        }
    }

    // 每个召唤位至少有一张对应卡（基础或异格）即满足。
    private bool HasAllRequiredSummons() =>
        SummonSlots.All(slot => Owner.Deck.Cards.Any(card => slot.Contains(card.GetType())));

    protected override void OnUpgrade() => DynamicVars["Count"].UpgradeValueBy(2m);
}

// 冠军形态（先古卡）：由古老牙齿把「基地补给」变身获得。升级后费用 2→1。
[Pool(typeof(LaughmanCardPool))]
public class ChampionForm : LaughmanCard, IOwnedMechTargetingCard
{
    public ChampionForm() : base(2, CardType.Power, CardRarity.Ancient, TargetType.AnyAlly) { }

    protected override bool IsPlayable => TeamMemberUtils.AliveMechs(Owner).Count > 0;

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var target = p.Target;
        await CreatureCmd.GainMaxHp(target, 12m);
        await CreatureCmd.Heal(target, 12m);
        await PowerCmd.Apply<StrengthPower>(c, target, 5m, Owner.Creature, this);
        await PowerCmd.Apply<PlatingPower>(c, target, 5m, Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(c, target, 5m, Owner.Creature, this);
        await PowerCmd.Apply<ChampionFormPower>(c, target, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[Pool(typeof(LaughmanCardPool))]
public class HighSpeedFlank : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DamageVar(10m, ValueProp.Move) };
    public HighSpeedFlank() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        foreach (var mech in TeamMemberUtils.AliveMechs(Owner))
        {
            await PowerCmd.Apply<GyroSpinPower>(c, mech, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
