using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Cards;

// C6 弹药补给：抽牌，并给随机机器人力量。消耗。
[Pool(typeof(LaughmanCardPool))]
public class AmmoResupply : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("DrawCount", 1m),
        new DynamicVar("StrengthPower", 1m)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public AmmoResupply() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars["DrawCount"].IntValue, Owner);

        var mechs = Owner.Creature.Pets.Where(p => p.Monster is MechModel && !p.IsDead).ToList();
        if (mechs.Count > 0)
        {
            var target = mechs[Owner.RunState.Rng.MonsterAi.NextInt(mechs.Count)];
            await PowerCmd.Apply<StrengthPower>(
                choiceContext, target, DynamicVars["StrengthPower"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DrawCount"].UpgradeValueBy(1m);
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}
