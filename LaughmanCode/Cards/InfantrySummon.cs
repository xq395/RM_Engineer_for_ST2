using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

// C4 步兵部署（初始牌）：召唤一台步兵。固有 + 消耗（保证首回合必上一台，只上一次）。
[Pool(typeof(LaughmanCardPool))]
public class InfantrySummon : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("MechHp", 13m) };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Innate, CardKeyword.Exhaust };

    public InfantrySummon() : base(2, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await MechManager.SummonMech<InfantryMech>(Owner, DynamicVars["MechHp"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MechHp"].UpgradeValueBy(5m);
    }
}
