using BaseLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Laughman.LaughmanCode.Cards;
using Laughman.LaughmanCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Laughman.LaughmanCode.Relics;

// 最后检查（事件遗物，事件2「我已做好准备！」选项奖励）：
// 获得时触发两次：在所有升级后的“能部署”卡（基础部署 + 异格部署）中三选一获得。
[Pool(typeof(LaughmanRelicPool))]
public class FinalCheck : LaughmanRelic
{
    private const int Offers = 2;
    private const int ChoicesPerOffer = 3;

    // 全部“能部署”卡：基础部署 + 异格部署。
    private static readonly Type[] DeployCards =
    {
        typeof(InfantrySummon), typeof(InfantryNo4), typeof(SentinelDeployment), typeof(StalledSentinel),
        typeof(HeroDeployment), typeof(EngineerDeployment), typeof(DroneDeployment), typeof(SwarmDeployment),
        typeof(RadarLock),
        typeof(NewCovenantDrone), typeof(BeaconDart), typeof(PrecisionEngineering), typeof(SkyEyeRadar), typeof(IAmTheWave)
    };

    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task AfterObtained()
    {
        for (int i = 0; i < Offers; i++)
        {
            await OfferOne();
        }
    }

    private async Task OfferOne()
    {
        // 从部署卡类型里随机取 3 种，各造一张并升级。
        var chosenTypes = DeployCards
            .OrderBy(_ => Owner.RunState.Rng.Niche.NextInt(999))
            .Take(ChoicesPerOffer)
            .ToList();

        var pool = ModelDb.CardPool<LaughmanCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint);
        var options = new List<CardModel>();
        foreach (var type in chosenTypes)
        {
            var canonical = pool.FirstOrDefault(c => c.GetType() == type);
            if (canonical == null)
            {
                continue;
            }
            var card = Owner.RunState.CreateCard(canonical, Owner);
            if (!card.IsUpgraded)
            {
                CardCmd.Upgrade(card);
            }
            options.Add(card);
        }
        if (options.Count == 0)
        {
            return;
        }

        var chosen = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), options, Owner, canSkip: false);
        if (chosen != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(chosen, PileType.Deck), 2f);
        }
    }
}
