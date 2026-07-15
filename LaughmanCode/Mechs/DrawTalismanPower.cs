using BaseLib.Abstracts;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Mechs;

public sealed class DrawTalismanPower : CustomPowerModel
{
    private const int DamagePerCountdown = 25;
    private bool _isDetonating;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Unpowered),
        new PowerVar<StrengthPower>(2m),
        new DynamicVar("MechDamage", 0m)
    };

    public void Configure(decimal damage, decimal strength)
    {
        AssertMutable();
        DynamicVars.Damage.BaseValue = damage;
        DynamicVars["StrengthPower"].BaseValue = strength;
    }

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || _isDetonating)
        {
            return;
        }
        if (Amount <= 1)
        {
            await Detonate(choiceContext);
        }
        else
        {
            await PowerCmd.Decrement(this);
        }
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (_isDetonating
            || dealer?.Monster is not MechModel
            || dealer.PetOwner?.Creature != Owner
            || !props.IsPoweredAttack()
            || dealer.Side == target.Side
            || result.TotalDamage <= 0)
        {
            return;
        }

        int accumulatedDamage = DynamicVars["MechDamage"].IntValue + result.TotalDamage;
        int countdowns = accumulatedDamage / DamagePerCountdown;
        DynamicVars["MechDamage"].BaseValue = accumulatedDamage % DamagePerCountdown;
        if (countdowns <= 0)
        {
            return;
        }
        if (countdowns >= Amount)
        {
            await Detonate(choiceContext);
        }
        else
        {
            await PowerCmd.ModifyAmount(choiceContext, this, -countdowns, null, null);
        }
    }

    private async Task Detonate(PlayerChoiceContext choiceContext)
    {
        if (_isDetonating || CombatState == null)
        {
            return;
        }
        _isDetonating = true;
        Flash();

        var enemies = CombatState.HittableEnemies.Where(enemy => !enemy.IsDead).ToList();
        if (enemies.Count > 0)
        {
            await CreatureCmd.Damage(
                choiceContext,
                enemies,
                DynamicVars.Damage,
                Owner);
        }

        var allies = CombatState.GetTeammatesOf(Owner).Where(ally => !ally.IsDead).ToList();
        var alliedPets = allies
            .Where(ally => ally.Player != null)
            .SelectMany(ally => ally.Pets)
            .Where(pet => !pet.IsDead);
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            allies.Concat(alliedPets).Distinct().ToList(),
            DynamicVars["StrengthPower"].BaseValue,
            Owner,
            null);
        await PowerCmd.Remove(this);
    }
}
