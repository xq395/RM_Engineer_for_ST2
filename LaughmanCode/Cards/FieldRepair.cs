using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Cards;

// U6 现场维修：治疗你所有机器人 7 点，获得 4 点格挡。
[Pool(typeof(LaughmanCardPool))]
public class FieldRepair : LaughmanCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("HealAmount", 7m),
        new BlockVar(4m, ValueProp.Move)
    };

    public FieldRepair() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var mechs = Owner.Creature.Pets.Where(p => p.Monster is MechModel && !p.IsDead).ToList();
        foreach (var mech in mechs)
        {
            await CreatureCmd.Heal(mech, DynamicVars["HealAmount"].BaseValue);
        }
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HealAmount"].UpgradeValueBy(3m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
