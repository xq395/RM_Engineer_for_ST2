using BaseLib.Utils;
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

// U 哨兵部署：召唤一台哨兵（9/13 血）。为给新的普通召唤牌「趴窝哨兵」腾出普通牌名额，升为罕见。
[Pool(typeof(LaughmanCardPool))]
public class SentinelDeployment : LaughmanCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("MechHp", 9m) };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public SentinelDeployment() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await MechManager.SummonSentinel(Owner, DynamicVars["MechHp"].IntValue, preferOperational: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MechHp"].UpgradeValueBy(4m);
    }
}

// C 趴窝哨兵：0 费召唤一台独立的早期故障版哨兵，并带有力量/敏捷减值。
[Pool(typeof(LaughmanCardPool))]
public class StalledSentinel : LaughmanCard
{
    // MechHp 与哨兵部署一致；Penalty 为负的力量/敏捷值（基础 -3，升级后 -2）。
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("MechHp", 9m), new DynamicVar("Penalty", -3m) };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[] { CardKeyword.Exhaust };

    public StalledSentinel() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sentinel = await MechManager.SummonSentinel(Owner, DynamicVars["MechHp"].IntValue, preferOperational: false);
        // 已有正常哨兵时，趴窝哨兵只强化高阶机体，不再施加故障减值。
        if (sentinel?.Monster is StalledSentinelMech)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, sentinel, DynamicVars["Penalty"].IntValue, Owner.Creature, this);
            await PowerCmd.Apply<DexterityPower>(choiceContext, sentinel, DynamicVars["Penalty"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Penalty"].UpgradeValueBy(1m);
    }
}
