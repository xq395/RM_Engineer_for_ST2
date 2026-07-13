using System;
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
//
// AfterDamageReceived 是 async 方法，`dealer.Monster is Osty` 编译进状态机 MoveNext 里的
// `isinst Osty`，因此必须用 MethodType.Async patch MoveNext 才能改到它。
// 诊断日志：patch 应用时打印替换了几处 isinst，用于确认补丁是否真正命中该 IL。
[HarmonyPatch(typeof(PersonalHivePower), nameof(PersonalHivePower.AfterDamageReceived), MethodType.Async)]
public static class PersonalHivePowerPatch
{
    private static bool IsHivePetDealer(MonsterModel? monster) =>
        monster is Osty or MechModel;

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        int replaced = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Isinst && instruction.operand as Type == typeof(Osty))
            {
                instruction.opcode = OpCodes.Call;
                instruction.operand = AccessTools.Method(typeof(PersonalHivePowerPatch), nameof(IsHivePetDealer));
                replaced++;
            }
            yield return instruction;
        }

        if (replaced > 0)
        {
            MainFile.Logger.Info($"PersonalHivePowerPatch: replaced {replaced} isinst Osty in AfterDamageReceived state machine.");
        }
        else
        {
            MainFile.Logger.Warn("PersonalHivePowerPatch: NO isinst Osty found — transpiler did not match, patch ineffective.");
        }
    }
}
