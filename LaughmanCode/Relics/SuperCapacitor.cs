using BaseLib.Utils;
using System.Collections.Generic;
using System.Linq;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Laughman.LaughmanCode.Relics;

// 超级电容（稀有）：每当你打出 17 张牌，一名随机友方步兵/哨兵/英雄获得 1 层飞坡。
// 出牌计数可跨战斗继承，触发后清零。
[Pool(typeof(LaughmanRelicPool))]
public class SuperCapacitor : LaughmanRelic
{
    private const int Threshold = 17;

    public override RelicRarity Rarity => RelicRarity.Rare;

    // 跨战斗继承的出牌计数（0..16 循环）。
    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int CardsPlayed { get; set; }

    public override bool ShowCounter => true;
    public override int DisplayAmount => CardsPlayed % Threshold;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 只统计玩家自己打出的牌。
        if (cardPlay.Card.Owner != Owner)
        {
            return;
        }
        CardsPlayed++;
        InvokeDisplayAmountChanged();
        if (CardsPlayed % Threshold != 0)
        {
            return;
        }

        Flash();
        // 从存活的步兵/哨兵/英雄中随机选一台，给 1 层飞坡。
        var candidates = Owner.Creature.Pets
            .Select(p => p.Monster as MechModel)
            .Where(m => m != null && !m!.Creature.IsDead
                && (m is InfantryMech || m is InfantryNo4Mech || m is SentinelMech || m is HeroMech))
            .ToList();
        if (candidates.Count == 0)
        {
            return;
        }
        var target = candidates[Owner.RunState.Rng.MonsterAi.NextInt(candidates.Count)]!;
        await PowerCmd.Apply<RampJumpPower>(choiceContext, target.Creature, 1m, Owner.Creature, null);
    }
}
