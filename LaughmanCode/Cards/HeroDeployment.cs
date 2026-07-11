using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

// U1 英雄部署：召唤一台英雄（20/25 血）。
[Pool(typeof(LaughmanCardPool))]
public class HeroDeployment : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("MechHp", 18m) };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public HeroDeployment() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await MechManager.SummonMech<HeroMech>(Owner, DynamicVars["MechHp"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MechHp"].UpgradeValueBy(5m);
    }
}
