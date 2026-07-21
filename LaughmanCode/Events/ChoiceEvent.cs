using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using Laughman.LaughmanCode.Cards;
using Laughman.LaughmanCode.Config;
using Laughman.LaughmanCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;

namespace Laughman.LaughmanCode.Events;

// 事件1「抉择……抉择……」（第2、3 层）：出线了！要不要打 UC 呢？
//  - 「必须！」：获得任务牌 冲刺UC（第三层解锁特殊事件）。
//  - 「从来没觉得打RM开心过……」：获得遗物 涂满的笔记本（加一张神化+一张悔恨）。
public sealed class ChoiceEvent : CustomEventModel
{
    private const int Act2Index = 1;
    private const int Act3Index = 2;

    public override string? CustomInitialPortraitPath =>
        "res://Laughman/images/events/choice_event.png";

    public override bool IsAllowed(IRunState runState)
        => (runState.CurrentActIndex == Act2Index || runState.CurrentActIndex == Act3Index)
           && CrossCharacterContent.AllowForRun(
               LaughmanConfig.ShareEventsWithOtherCharacters,
               runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new List<EventOption>
        {
            Option(MustGo),
            Option(NeverHappy),
        };
    }

    // 「必须！」：把任务卡 冲刺UC 加入牌组。
    private async Task MustGo()
    {
        var quest = Owner.RunState.CreateCard<SprintUC>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(quest, PileType.Deck), 2f);
        SetEventFinished(L10NLookup($"{Id.Entry}.pages.RESULT_MUST.description"));
    }

    // 「从来没觉得打RM开心过……」：获得遗物 涂满的笔记本。
    private async Task NeverHappy()
    {
        var relic = ModelDb.Relic<ScribbledNotebook>().ToMutable();
        await RelicCmd.Obtain(relic, Owner);
        SetEventFinished(L10NLookup($"{Id.Entry}.pages.RESULT_NEVER.description"));
    }
}
