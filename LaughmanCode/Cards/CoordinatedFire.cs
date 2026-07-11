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
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Cards;

// C8 协同射击：造成 5 点伤害，你每控制 1 台机器人额外造成 4 点伤害。
[Pool(typeof(LaughmanCardPool))]
public class CoordinatedFire : LaughmanCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("PerMech", 4m)
    };

    public CoordinatedFire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        int mechCount = Owner.Creature.Pets.Count(p => p.Monster is MechModel && !p.IsDead);
        decimal total = DynamicVars.Damage.BaseValue + DynamicVars["PerMech"].BaseValue * mechCount;
        await DamageCmd.Attack(total)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["PerMech"].UpgradeValueBy(2m);
    }
}
