using System;
using System.Collections.Generic;
using BaseLib.Abstracts;
using Godot;
using Laughman.LaughmanCode.Config;
using Laughman.LaughmanCode.Potions;
using Laughman.LaughmanCode.Timeline;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;

namespace Laughman.LaughmanCode.Character;

public class LaughmanPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Laughman.Color;

    protected override IEnumerable<PotionModel> GenerateAllPotions() =>
        new PotionModel[]
        {
            ModelDb.Potion<SpareBattery>(),
            ModelDb.Potion<CoolantFlask>(),
            ModelDb.Potion<CalibrationFluid>(),
            ModelDb.Potion<EmergencySolder>(),
            ModelDb.Potion<OverclockInjector>()
        };

    // 药水受时间线门控：只有揭示 LaughmanPotionEpoch 后，本角色药水才进入池。
    public override IEnumerable<PotionModel> GetUnlockedPotions(UnlockState unlockState)
        => LaughmanConfig.SharePotionsWithOtherCharacters
            ? Array.Empty<PotionModel>()
            : GetUnlockedPotionsForSharedPool(unlockState);

    internal IEnumerable<PotionModel> GetUnlockedPotionsForSharedPool(UnlockState unlockState)
    {
        if (unlockState == UnlockState.all)
        {
            return GenerateAllPotions();
        }
        // IsEpochRevealed<T>() 内部走 EpochModel.GetId<T>() 查字典；若 RitsuLib 尚未注册该 epoch
        // 会抛 KeyNotFound，这里用 try/catch 兜底：注册未就绪时视为未解锁（返回空），绝不崩。
        try
        {
            if (!unlockState.IsEpochRevealed<LaughmanPotionEpoch>())
            {
                return Array.Empty<PotionModel>();
            }
        }
        catch (Exception)
        {
            return Array.Empty<PotionModel>();
        }
        return GenerateAllPotions();
    }
}
