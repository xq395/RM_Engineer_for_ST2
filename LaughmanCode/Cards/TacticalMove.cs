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

// C7 战术转移：你和一台机甲获得格挡，抽牌。
[Pool(typeof(LaughmanCardPool))]
public class TacticalMove : LaughmanCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(4m, ValueProp.Move),
        new DynamicVar("DrawCount", 1m)
    };

    public TacticalMove() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var target = Owner.Creature.Pets
            .Where(p => p.Monster is MechModel && !p.IsDead)
            .OrderByDescending(p => p.Monster is MechModel m && m.IsGuard)
            .FirstOrDefault();
        if (target != null)
        {
            await CreatureCmd.GainBlock(target, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        }
        await CardPileCmd.Draw(choiceContext, DynamicVars["DrawCount"].IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars["DrawCount"].UpgradeValueBy(1m);
    }
}
