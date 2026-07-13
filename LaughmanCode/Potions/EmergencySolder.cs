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
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Potions;

// 应急焊料（罕见）：你和所有友方机器人各获得 8 点格挡。
[Pool(typeof(LaughmanPotionPool))]
public sealed class EmergencySolder : LaughmanPotion
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[] { new BlockVar(8m, ValueProp.Move) };

    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
        foreach (var mech in Owner.Creature.Pets.Where(p => p.Monster is MechModel && !p.IsDead))
        {
            await CreatureCmd.GainBlock(mech, DynamicVars.Block, null);
        }
    }
}
