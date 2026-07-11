using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using Laughman.LaughmanCode.Cards;
using Laughman.LaughmanCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;

namespace Laughman.LaughmanCode.Events;

// 事件2「决赛准备」（第三层特殊事件）：只能由任务卡 冲刺UC 的 ModifyNextEvent 触发，
// 不进普通事件池（IsAllowed 恒 false，避免随机刷出）。
//  - 「我已做好准备！」：将 冲击UL 替换为 剑指春茧（升级版），获得遗物 最后检查。
//  - 「跑路了兄弟，跑路了」：获得遗物 美美撤离。
// 两个选项都会清除牌组里的任务卡 冲刺UC。
public sealed class UcSprintEvent : CustomEventModel
{
    // 不参与普通事件随机池；仅通过任务卡强制触发。
    public override bool IsAllowed(IRunState runState) => false;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new List<EventOption>
        {
            new EventOption(this, Ready, "UC_SPRINT_EVENT.pages.INITIAL.options.READY"),
            new EventOption(this, RunAway, "UC_SPRINT_EVENT.pages.INITIAL.options.RUN_AWAY"),
        };
    }

    // 「我已做好准备！」：冲击UL → 剑指春茧（升级），获得遗物 最后检查。
    private async Task Ready()
    {
        var impacts = Owner.Deck.Cards.Where(c => c is ImpactUL).ToList();
        foreach (var impact in impacts)
        {
            var cocoon = Owner.RunState.CreateCard<RoadToSpringCocoon>(Owner);
            if (!cocoon.IsUpgraded)
            {
                CardCmd.Upgrade(cocoon);
            }
            await CardCmd.Transform(impact, cocoon);
        }

        var relic = ModelDb.Relic<FinalCheck>().ToMutable();
        await RelicCmd.Obtain(relic, Owner);

        await RemoveQuestCard();
        SetEventFinished(L10NLookup("UC_SPRINT_EVENT.pages.RESULT_READY.description"));
    }

    // 「跑路了兄弟，跑路了」：获得遗物 美美撤离。
    private async Task RunAway()
    {
        var relic = ModelDb.Relic<GracefulRetreat>().ToMutable();
        await RelicCmd.Obtain(relic, Owner);

        await RemoveQuestCard();
        SetEventFinished(L10NLookup("UC_SPRINT_EVENT.pages.RESULT_RUN.description"));
    }

    // 移除牌组里的任务卡 冲刺UC（事件已消费）。
    private async Task RemoveQuestCard()
    {
        var quests = Owner.Deck.Cards.Where(c => c is SprintUC).ToList();
        if (quests.Count > 0)
        {
            await CardPileCmd.RemoveFromDeck(quests);
        }
    }
}
