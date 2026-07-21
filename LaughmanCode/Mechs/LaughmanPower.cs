using BaseLib.Abstracts;
using BaseLib.Extensions;
using Laughman.LaughmanCode.Extensions;

namespace Laughman.LaughmanCode.Mechs;

// 自定义 Power 默认按模型 ID 读取 images/powers/<id>.png。
public abstract class LaughmanPower : CustomPowerModel
{
    public override string? CustomPackedIconPath =>
        "res://" + $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
}
