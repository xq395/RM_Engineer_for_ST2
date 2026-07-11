using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Laughman.LaughmanCode.Cards;

public static class LaughmanHoverTips
{
    public static IHoverTip Term(string id) =>
        new HoverTip(new LocString("cards", $"LAUGHMAN-HOVER_{id}.title"),
            new LocString("cards", $"LAUGHMAN-HOVER_{id}.description"));

    // 覆甲：引用原版 PlatingPower 自带的名字、图标和机制说明，避免自定义副本名字/图标对不上。
    public static IHoverTip PlatedArmor() => HoverTipFactory.FromPower<PlatingPower>();
}
