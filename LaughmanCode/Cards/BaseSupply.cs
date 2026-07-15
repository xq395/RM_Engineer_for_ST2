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

namespace Laughman.LaughmanCode.Cards;

// 初始卡：维修/复活单台机甲，并给予力量。当前自动选择目标，等机器人目标 UI 完成后再改为手动指定。
[Pool(typeof(LaughmanCardPool))]
public class BaseSupply : LaughmanCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[]
        {
            new DynamicVar("HealAmount", 7m),
            new DynamicVar("StrengthPower", 1m)
        };

    public BaseSupply() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var dead = Owner.Creature.Pets
            .FirstOrDefault(p => p.Monster is MechModel && p.IsDead);
        if (dead != null)
        {
            await CreatureCmd.Heal(dead, dead.MaxHp);
            await PowerCmd.Apply<StrengthPower>(choiceContext, dead, DynamicVars["StrengthPower"].IntValue, Owner.Creature, this);
            await CreatureCmd.Stun(dead, (string?)null);
            MechManager.RestoreRevivedMechUi(Owner, dead);
            return;
        }

        var target = Owner.Creature.Pets
            .Where(p => p.Monster is MechModel && !p.IsDead)
            .OrderByDescending(p => p.MaxHp - p.CurrentHp)
            .FirstOrDefault();
        if (target == null)
        {
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
