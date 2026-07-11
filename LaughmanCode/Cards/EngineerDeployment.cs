using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

// U2 工程部署：召唤一台工程（15/18 血）。升级版召唤瞬间一次性 +1 能量。
[Pool(typeof(LaughmanCardPool))]
public class EngineerDeployment : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[]
        {
            new DynamicVar("MechHp", 14m),
            new DynamicVar("Block", 4m),
            new DynamicVar("Gold", 2m),
            new DynamicVar("Actions", 6m)
        };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public EngineerDeployment() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var mech = await MechManager.SummonMech<EngineerMech>(Owner, DynamicVars["MechHp"].IntValue);
        if (mech?.Monster is EngineerMech engineer)
        {
            engineer.BlockPerTurn = DynamicVars["Block"].IntValue;
            engineer.GoldPerTurn = DynamicVars["Gold"].IntValue;
            engineer.MaxActions = DynamicVars["Actions"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MechHp"].UpgradeValueBy(3m);
        DynamicVars["Block"].UpgradeValueBy(3m);
        DynamicVars["Gold"].UpgradeValueBy(1m);
        DynamicVars["Actions"].UpgradeValueBy(2m);
    }
}
