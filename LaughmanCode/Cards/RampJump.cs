using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

// U7 飞坡：选中的机器人本回合造成伤害 ×3、受到伤害减半。
[Pool(typeof(LaughmanCardPool))]
public class RampJump : LaughmanCard, IOwnedMechTargetingCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public RampJump() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly) { }
    protected override bool IsPlayable => TeamMemberUtils.AliveMechs(Owner).Count > 0;
    public bool IsAllowedMechTarget(Creature target) =>
        target.Monster is InfantryMech or InfantryNo4Mech or SentinelMech or HeroMech;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var target = cardPlay.Target;

        await PowerCmd.Apply<RampJumpPower>(choiceContext, target, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
