using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Potions;

// 过载注入剂（稀有）：你的所有机器人各获得 3 点力量，但各受到 2 点无法阻挡的伤害。
[Pool(typeof(LaughmanPotionPool))]
public sealed class OverclockInjector : LaughmanPotion
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(3m),
        new DynamicVar("SelfDamage", 2m)
    };

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { HoverTipFactory.FromPower<StrengthPower>(null) };

    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        foreach (var mech in Owner.Creature.Pets.Where(p => p.Monster is MechModel && !p.IsDead))
        {
            await PowerCmd.Apply<StrengthPower>(
                choiceContext, mech, DynamicVars["StrengthPower"].BaseValue, Owner.Creature, null);
            await CreatureCmd.Damage(
                choiceContext, mech, DynamicVars["SelfDamage"].BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature);
        }
    }
}
