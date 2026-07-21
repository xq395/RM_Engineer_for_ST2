using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

// U3 无人机部署：召唤一台无人机（9/12 血，飞行，多段 2×3 / 升级 2×4）。
[Pool(typeof(LaughmanCardPool))]
public class DroneDeployment : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("MechHp", 9m) };

    private int _hitCount = 3;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public DroneDeployment() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var mech = await MechManager.SummonMech<DroneMech>(Owner, DynamicVars["MechHp"].IntValue);
        if (mech?.Monster is DroneMech drone)
        {
            drone.HitCount = _hitCount;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MechHp"].UpgradeValueBy(3m);
        _hitCount = 4;
    }
}
