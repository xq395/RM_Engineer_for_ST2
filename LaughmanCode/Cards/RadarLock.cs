using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

// U 雷达锁定（能力）：部署雷达，每回合给随机敌人施加易伤（升级附带虚弱）。
// 为配合新增的稀有能力「天眼雷达」，稀有度由稀有降为罕见。
[Pool(typeof(LaughmanCardPool))]
public class RadarLock : LaughmanCard
{
    private bool _appliesWeak;

    public RadarLock() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<RadarPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.AppliesWeak = _appliesWeak;
        }
    }

    protected override void OnUpgrade()
    {
        _appliesWeak = true;
    }
}
