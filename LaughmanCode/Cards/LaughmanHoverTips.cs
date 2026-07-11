using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Laughman.LaughmanCode.Cards;

public static class LaughmanHoverTips
{
    public static IHoverTip Term(string id) =>
        new HoverTip(new LocString("cards", $"LAUGHMAN-HOVER_{id}.title"),
            new LocString("cards", $"LAUGHMAN-HOVER_{id}.description"));
}
