using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Cards;

// C2 装甲展开：给一个单位格挡；若其带屏卫，再获得覆甲。
// 自动选择目标：优先带屏卫的机器人（给覆甲收益最高），否则给玩家。
// 原版 TargetType.AnyAlly 不把玩家宠物算作“队友”，不能直接用来选机器人。
[Pool(typeof(LaughmanCardPool))]
public class ArmorDeploy : LaughmanCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, ValueProp.Move),
        new DynamicVar("PlatedArmor", 3m)
    };

    public ArmorDeploy() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    public override bool GainsBlock => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature target = Owner.Creature.Pets
            .FirstOrDefault(p => p.Monster is MechModel m && m.IsGuard && !p.IsDead)
            ?? Owner.Creature;

        await CreatureCmd.GainBlock(target, DynamicVars.Block, cardPlay);

        if (target.Monster is MechModel mech && mech.IsGuard)
        {
            await PowerCmd.Apply<PlatingPower>(
                choiceContext, target, DynamicVars["PlatedArmor"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["PlatedArmor"].UpgradeValueBy(1m);
    }
}
