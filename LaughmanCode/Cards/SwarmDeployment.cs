using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

// U 飞镖部署：召唤一台飞镖机器人（基础 1 血 / 升级 2 血，休眠后 25 伤害 ×4）。消耗。
// 为配合新增的稀有召唤牌，费用由 3 降为 2，稀有度由稀有降为罕见。
[Pool(typeof(LaughmanCardPool))]
public class SwarmDeployment : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[]
        {
            new DynamicVar("MechHp", 1m),
            new DynamicVar("ChargeTurns", 2m),
            new DynamicVar("BurstDamage", 25m)
        };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public SwarmDeployment() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var mech = await MechManager.SummonMech<DartBotMech>(Owner, DynamicVars["MechHp"].IntValue);
        if (mech?.Monster is DartBotMech dart)
        {
            dart.ChargeTurns = DynamicVars["ChargeTurns"].IntValue;
            dart.BurstDamage = DynamicVars["BurstDamage"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MechHp"].UpgradeValueBy(1m);
        DynamicVars["ChargeTurns"].UpgradeValueBy(-1m);
    }
}
