using BaseLib.Abstracts;
using BaseLib.Extensions;
using Laughman.LaughmanCode.Extensions;
using Godot;

namespace Laughman.LaughmanCode.Potions;

// 招笑人药水基类：按 id 约定解析 PNG 图标，缺失时回退到原版图集路径（返回 null）。
// 例如 id "LAUGHMAN-SPARE_BATTERY" -> images/potions/spare_battery.png。
public abstract class LaughmanPotion : CustomPotionModel
{
    // 占位图：暂无专属药水美术时统一回退到卡牌占位图。
    private static string PlaceholderPath => "placeholder.png".CardImagePath();

    public override string? CustomPackedImagePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : PlaceholderPath;
        }
    }

    public override string? CustomPackedOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : CustomPackedImagePath;
        }
    }
}
