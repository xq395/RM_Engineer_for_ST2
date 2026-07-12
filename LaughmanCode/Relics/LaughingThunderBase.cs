using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using MegaCrit.Sts2.Core;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Laughman.LaughmanCode.Relics;

// 招笑避雷 / 工程师之愿 的共享逻辑。
//
// 计数器改为“从本次运行的地图历史中统计已进入的先古之民（Ancient）数量”，
// 上限 3。这样先古升级替换初始遗物（欧洛巴斯之触只按遗物 ID 新建替换实例）后，
// 工程师之愿能天然读到相同的访问数，无需依赖 [SavedProperty] 跨替换保存。
//
// 力量/敏捷修正为固定值（由子类给出），不再随访问数回升。
// 进入先古之民房间时累计一次“变化打击/防御”机会，战斗胜利后在牌组干净状态下兑现。
public abstract class LaughingThunderBase : LaughmanRelic
{
    private const int MaxAncientVisits = 3;

    // 力量/敏捷修正。招笑避雷返回固定值；工程师之愿返回随先古访问数回升的值。
    protected abstract int StatModifier { get; }

    // 已计入但尚未兑现的“变化打击/防御”机会。进入先古之民房间时累加（非阻塞），
    // 战斗结束后（牌组处于战斗外干净状态）才执行变卡，避免破坏战斗与牌组克隆联动。
    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int PendingTransforms { get; set; }

    // 地图历史可能在房间进入 hook 前后各触发一次更新；记录已经排队到哪次先古访问，
    // 避免同一房间重复加入变卡机会。
    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int ScheduledAncientVisits { get; set; }

    public override RelicRarity Rarity => RelicRarity.Starter;

    // 从地图历史推算已访问的先古之民数量，上限 3。替换遗物后仍然一致。
    protected int AncientVisits
    {
        get
        {
            int count = 0;
            foreach (var mapPoint in Owner.RunState.MapPointHistory)
            {
                foreach (var entry in mapPoint)
                {
                    if (entry.MapPointType == MapPointType.Ancient)
                    {
                        count++;
                    }
                }
            }
            return Math.Min(count, MaxAncientVisits);
        }
    }

    public override bool ShowCounter => AncientVisits > 0;
    public override int DisplayAmount => AncientVisits;

    public override async Task BeforeCombatStart()
    {
        // 战斗刚建立，牌组已克隆到抽牌堆，此时绝不能改牌组（会破坏 DeckVersion 联动）。
        Flash();
        PlayerChoiceContext choiceContext = new BlockingPlayerChoiceContext();
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, StatModifier, null, null);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, StatModifier, null, null);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        // 事件房进入过程中不能弹阻塞式 UI（会死锁黑屏），这里只做非阻塞的计数。
        // 访问数由地图历史统计，这里只累加待兑现的变卡机会。
        if (room is not EventRoom eventRoom || eventRoom.CanonicalEvent is not AncientEventModel)
        {
            return Task.CompletedTask;
        }

        int visits = AncientVisits;
        if (visits > MaxAncientVisits || visits <= ScheduledAncientVisits)
        {
            return Task.CompletedTask;
        }

        PendingTransforms += visits - ScheduledAncientVisits;
        ScheduledAncientVisits = visits;
        Flash();
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        // 战斗胜利后、奖励界面之前：战斗克隆已回收，牌组处于干净的战斗外状态，
        // UI 稳定，可安全地弹选牌并永久变化牌组。
        if (PendingTransforms <= 0)
        {
            return;
        }

        while (PendingTransforms > 0)
        {
            PendingTransforms--;
            await TransformOne(CardTag.Strike);
            await TransformOne(CardTag.Defend);
        }

        InvokeDisplayAmountChanged();
    }

    private async Task TransformOne(CardTag tag)
    {
        LocString prompt = CardSelectorPrefs.TransformSelectionPrompt;
        // 0~1 张：可以跳过（例如打/防已被全部变化）。
        CardSelectorPrefs prefs = new(prompt, 0, 1)
        {
            RequireManualConfirmation = true,
            Cancelable = true
        };

        // 只展示带对应标签且可变化的牌，玩家从中挑一张。
        var selected = (await CardSelectCmd.FromDeckGeneric(
            Owner,
            prefs,
            c => c.IsTransformable && c.Tags.Contains(tag))).ToList();

        foreach (CardModel original in selected)
        {
            var options = ModelDb.CardPool<LaughmanCardPool>()
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Where(c => c.Rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare)
                .ToList();
            // 战斗外对牌组执行 Transform 即为永久替换，替换池显式使用工程师自己的卡池，
            // 避免 Basic 原卡回到战士池。
            CardModel canonical = Owner.RunState.Rng.Niche.NextItem(options)
                ?? throw new InvalidOperationException("Laughman transform pool is empty.");
            CardModel replacement = original.CardScope!.CreateCard(canonical, Owner);
            await CardCmd.Transform(original, replacement);
        }
    }
}
