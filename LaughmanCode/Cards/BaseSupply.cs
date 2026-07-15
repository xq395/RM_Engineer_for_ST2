using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Laughman.LaughmanCode.Config;

namespace Laughman.LaughmanCode.Cards;

// 初始卡：维修/复活单台机甲，并给予力量。当前自动选择目标，等机器人目标 UI 完成后再改为手动指定。
[Pool(typeof(LaughmanCardPool))]
public class BaseSupply : LaughmanCard, IOwnedMechTargetingCard
{
    public OwnedMechTargetMode MechTargetMode => OwnedMechTargetMode.DeadElseAlive;
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[]
        {
            new DynamicVar("HealAmount", 7m),
            new DynamicVar("StrengthPower", 1m)
        };

    public BaseSupply() : base(1, CardType.Skill, CardRarity.Basic, TargetType.AnyAlly) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var target = cardPlay.Target;
        if (target.IsDead)
        {
            await CreatureCmd.Heal(target, target.MaxHp);
            await PowerCmd.Apply<StrengthPower>(choiceContext, target, DynamicVars["StrengthPower"].IntValue, Owner.Creature, this);
            if (WeakHelper.IsWeak)
            {
                await CreatureCmd.Stun(target, (string?)null);
            }
            MechManager.RestoreRevivedMechUi(Owner, target);
            return;
        }

        await CreatureCmd.Heal(target, DynamicVars["HealAmount"].IntValue);
        await PowerCmd.Apply<StrengthPower>(choiceContext, target, DynamicVars["StrengthPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HealAmount"].UpgradeValueBy(3m);
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}
