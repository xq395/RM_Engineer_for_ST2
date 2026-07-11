using BaseLib.Config;

namespace Laughman.LaughmanCode.Config;

// 招笑人 / RM工程师 的模组配置。基于 BaseLib 的 SimpleModConfig，属性自动生成设置界面。
// 削弱版开关：打开后（需重启游戏）大部分卡牌与机甲数值切换为削弱值，
// 目标强度约为"比原版略强、低于强势模组"。
//
// 注意：配置属性必须为 static，且本类位于命名空间内（BaseLib 要求）。
// 数值在卡牌注册时定型，因此修改开关后需重启游戏才能生效。
internal class LaughmanConfig : SimpleModConfig
{
    [ConfigSection("WEAK_MODE_SECTION")]
    public static bool WeakMode { get; set; } = false;
}
