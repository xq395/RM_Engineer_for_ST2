using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Laughman.LaughmanCode.Patches;

// 原版 PersonalHivePower 只把 Osty 宠物映射回主人玩家。
// Laughman 机甲同样是玩家宠物，否则 dealer.Player 为空，蜂群术士生成晕眩牌时会空引用。
[HarmonyPatch(typeof(PersonalHivePower), nameof(PersonalHivePower.AfterDamageReceived), MethodType.Async)]
public static class PersonalHivePowerPatch
{
    private static bool IsHivePetDealer(MonsterModel? monster) =>
        monster is Osty or MechModel;

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Isinst && instruction.operand as Type == typeof(Osty))
            {
                instruction.opcode = OpCodes.Call;
                instruction.operand = AccessTools.Method(typeof(PersonalHivePowerPatch), nameof(IsHivePetDealer));
            }
            yield return instruction;
        }
    }
}
