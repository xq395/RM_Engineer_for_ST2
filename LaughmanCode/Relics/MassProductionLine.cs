using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Laughman.LaughmanCode.Relics;

// 量产流水线（稀有）：每场战斗第二次及以后召唤机器人时，获得 1 点能量。
[Pool(typeof(LaughmanRelicPool))]
public class MassProductionLine : LaughmanRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    // 本场战斗的召唤次数（每场重置）。
    private int _summonsThisCombat;

    public override Task BeforeCombatStart()
    {
        _summonsThisCombat = 0;
        return Task.CompletedTask;
    }

    // 由 MechManager 在每次召唤（含同名叠加）后调用。
    public async Task OnSummon(Player owner)
    {
        _summonsThisCombat++;
        if (_summonsThisCombat < 2)
        {
            return;
        }
        Flash();
        await PlayerCmd.GainEnergy(1m, owner);
    }
}
