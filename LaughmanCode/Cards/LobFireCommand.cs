using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

// U5 吊射指令：把英雄机器人切换为吊射模式（休息 / 大招循环，35/42 伤害）。消耗。
[Pool(typeof(LaughmanCardPool))]
public class LobFireCommand : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("LobDamage", 35m) };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public LobFireCommand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hero = Owner.Creature.Pets
            .Where(p => p.Monster is HeroMech && !p.IsDead)
            .Select(p => p.Monster as HeroMech)
            .FirstOrDefault();
        if (hero != null)
        {
            hero.EnableLobFire(DynamicVars["LobDamage"].IntValue);
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["LobDamage"].UpgradeValueBy(7m);
    }
}
