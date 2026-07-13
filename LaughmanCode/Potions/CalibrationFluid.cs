using BaseLib.Utils;
using System.Collections.Generic;
using System.Linq;
using Laughman.LaughmanCode.Cards;
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

namespace Laughman.LaughmanCode.Potions;

// WD40（稀有）：一台随机存活的屏卫机器人回复 10 点生命并获得 1 层小陀螺。
[Pool(typeof(LaughmanPotionPool))]
public sealed class CalibrationFluid : LaughmanPotion
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Heal", 10m),
        new DynamicVar("Turns", 1m)
    };

    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[] { LaughmanHoverTips.Term("GUARD"), LaughmanHoverTips.Term("GYRO_SPIN") };

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var candidates = Owner.Creature.Pets
            .Select(p => p.Monster as MechModel)
            .Where(m => m != null && !m.Creature.IsDead && m.IsGuard)
            .ToList();
        if (candidates.Count == 0)
        {
            return;
        }
        var mech = candidates[Owner.RunState.Rng.MonsterAi.NextInt(candidates.Count)]!;
        await CreatureCmd.Heal(mech.Creature, DynamicVars["Heal"].BaseValue);
        await PowerCmd.Apply<GyroSpinPower>(
            choiceContext, mech.Creature, DynamicVars["Turns"].BaseValue, Owner.Creature, null);
    }
}
