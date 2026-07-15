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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Cards;

// U4 过载协议：你的所有机器人各获得 2 点力量，并受到 3 点伤害。消耗。
[Pool(typeof(LaughmanCardPool))]
public class Overclock : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(2m),
        new DynamicVar("SelfDamage", 3m)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public Overclock() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.Self) { }
    protected override bool IsPlayable => TeamMemberUtils.AliveMechs(Owner).Count > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var mechs = Owner.Creature.Pets.Where(p => p.Monster is MechModel && !p.IsDead).ToList();
        foreach (var mech in mechs)
        {
            await PowerCmd.Apply<StrengthPower>(
                choiceContext, mech, DynamicVars["StrengthPower"].BaseValue, Owner.Creature, this);
            await CreatureCmd.Damage(
                choiceContext, mech, DynamicVars["SelfDamage"].BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}
