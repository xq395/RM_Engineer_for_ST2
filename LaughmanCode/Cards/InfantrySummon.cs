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

// C4 步兵部署（初始牌）：召唤一台步兵（-1 力量 -1 敏捷，收敛前期强度）。固有 + 消耗。
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
        var infantry = await MechManager.SummonMech<InfantryMech>(Owner, DynamicVars["MechHp"].IntValue);
        if (infantry != null)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, infantry, -1m, Owner.Creature, this);
            await PowerCmd.Apply<DexterityPower>(choiceContext, infantry, -1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MechHp"].UpgradeValueBy(5m);
    }
}
