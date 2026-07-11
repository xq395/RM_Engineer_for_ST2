using BaseLib.Utils;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

// U8 买活：花费 10 金币，使一台阵亡的步兵/英雄/哨兵复活。行动正常走，复活时获得一层小陀螺。
[Pool(typeof(LaughmanCardPool))]
public class PayToRespawn : LaughmanCard
{
    private const int GoldCost = 6;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new DynamicVar("ReviveHp", 10m) };

    public PayToRespawn() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 金币不足则不生效。
        if (Owner.Gold < GoldCost)
        {
            return;
        }

        // 找一台阵亡且可买活的机器人（仍留在宠物列表中，因协调器保留了它们）。
        Creature? dead = Owner.Creature.Pets
            .FirstOrDefault(p => p.Monster is IRevivableMech && p.IsDead);
        if (dead == null)
        {
            return;
        }

        await PlayerCmd.LoseGold(GoldCost, Owner, GoldLossType.Spent);
        await CreatureCmd.Heal(dead, DynamicVars["ReviveHp"].BaseValue);
        // 复活即获得一层小陀螺（本回合 50% 减伤），不再晕眩。
        await PowerCmd.Apply<GyroSpinPower>(choiceContext, dead, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
