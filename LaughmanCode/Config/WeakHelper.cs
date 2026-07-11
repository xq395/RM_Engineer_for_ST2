namespace Laughman.LaughmanCode.Config;

// 削弱版数值读取入口。仿蕾忍 NinjaHelper.GetValueByChallengeMode 的模式。
// 卡牌 / 机甲 / 队员在数值处调用 WeakHelper.V(削弱值, 正常值)：
//   正常模式返回正常值，削弱模式返回削弱值。
// 由于数值在注册时定型，修改开关后需重启游戏才能生效。
public static class WeakHelper
{
    public static bool IsWeak => LaughmanConfig.WeakMode;

    // 参数顺序固定为 (削弱值, 正常值)。
    public static int V(int weak, int normal) => IsWeak ? weak : normal;

    public static decimal V(decimal weak, decimal normal) => IsWeak ? weak : normal;
}
