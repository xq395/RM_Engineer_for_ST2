using BaseLib.Utils;
using System.Collections.Generic;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;

namespace Laughman.LaughmanCode.Cards;

// 任务卡「冲刺UC」（UC = 全阵容大赛）：无法被打出。
// 持有它且身处第三层（Act3）时，把下一个未知房间导向事件，并将下一个事件替换为
// 「决赛准备」特殊事件（UcSprintEvent）。
[Pool(typeof(LaughmanCardPool))]
public class SprintUC : LaughmanCard
{
    // Act3 的索引（0=Act1, 1=Act2, 2=Act3）。
    private const int Act3Index = 2;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Unplayable };

    public SprintUC() : base(-1, CardType.Quest, CardRarity.Quest, TargetType.None) { }

    // 永不可打出。
    protected override bool IsPlayable => false;

    // 第三层时，让未知房间倾向于事件房，提高特殊事件触发概率。
    public override IReadOnlySet<RoomType> ModifyUnknownMapPointRoomTypes(IReadOnlySet<RoomType> roomTypes)
    {
        if (Owner.RunState.CurrentActIndex != Act3Index)
        {
            return roomTypes;
        }
        return new HashSet<RoomType> { RoomType.Event };
    }

    // 持有此任务卡且在第三层时，把下一个事件替换为特殊事件。
    public override EventModel ModifyNextEvent(EventModel currentEvent)
    {
        if (Owner.RunState.CurrentActIndex != Act3Index)
        {
            return currentEvent;
        }
        return ModelDb.Event<UcSprintEvent>();
    }
}
