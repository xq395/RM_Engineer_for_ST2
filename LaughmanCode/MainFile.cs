using BaseLib.Config;
using Godot;
using HarmonyLib;
using Laughman.LaughmanCode.Config;
using MegaCrit.Sts2.Core.Modding;

namespace Laughman.LaughmanCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Laughman";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        // 注册模组配置（削弱版开关等）。
        ModConfigRegistry.Register(ModId, new LaughmanConfig());

        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}
