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

// R3 全军突击：你的所有机器人立即行动一次（额外行动）。升级后每台机器人额外给你 2 点格挡。
[Pool(typeof(LaughmanCardPool))]
public class FullAssault : LaughmanCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("BlockPerMech", 0m) };

    public FullAssault() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.Self) { }
    protected override bool IsPlayable => TeamMemberUtils.AliveMechs(Owner).Any(mech => mech.Monster is MechModel model && !model.IsBorrowed);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var mechs = Owner.Creature.Pets
            .Where(p => p.Monster is MechModel && !p.IsDead)
            .Select(p => p.Monster as MechModel)
            .ToList();
        int acted = 0;

        foreach (var mech in mechs)
        {
            if (mech == null || mech.Creature.IsDead)
            {
                continue;
            }
            if (await MechCoordinatorPower.TryPerformAction(choiceContext, Owner, combatState, mech))
            {
                acted++;
            }
        }

        decimal blockPerMech = DynamicVars["BlockPerMech"].BaseValue;
        if (blockPerMech > 0m)
        {
            if (acted > 0)
            {
                await CreatureCmd.GainBlock(Owner.Creature, blockPerMech * acted, ValueProp.Move, cardPlay);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockPerMech"].UpgradeValueBy(2m);
    }
}
