using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Laughman.LaughmanCode.Cards;

[Pool(typeof(LaughmanCardPool))]
public sealed class DrawTalisman : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Turns", 3m),
        new DynamicVar("Damage", 10m),
        new PowerVar<StrengthPower>(2m)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public DrawTalisman() : base(1, CardType.Attack, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<DrawTalismanPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Turns"].BaseValue,
            Owner.Creature,
            this);
        power?.Configure(
            DynamicVars["Damage"].BaseValue,
            DynamicVars["StrengthPower"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Turns"].UpgradeValueBy(-1m);
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}
