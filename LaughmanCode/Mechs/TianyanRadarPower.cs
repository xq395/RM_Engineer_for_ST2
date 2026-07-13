using BaseLib.Abstracts;
using System.Linq;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 天眼雷达（能力）：机甲行动前，对每个敌人：
//  - 若其意图包含攻击：施加 Shackles 层数的 [镣铐]（DarkShackles，当回合失去等量力量）。
//  - 否则：施加 Flanking 层数的自定义 [夹击]（本轮受到的机器人伤害 +100%）。
//  - 基础 Shackles=2 / Flanking=0；升级 Shackles=3 / Flanking=1。
public class TianyanRadarPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

    // 由天眼雷达卡设置。
    public int ShacklesAmount { get; set; } = 2;
    public int FlankingAmount { get; set; }

    public async Task PrepareMechVolley(PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (Owner.IsDead || CombatManager.Instance == null || CombatManager.Instance.IsEnding)
        {
            return;
        }

        foreach (var enemy in combatState.HittableEnemies.Where(e => !e.IsDead).ToList())
        {
            bool attacks = enemy.Monster?.NextMove.Intents.Any(i => i is AttackIntent) == true;
            if (attacks)
            {
                if (ShacklesAmount > 0)
                {
                    await PowerCmd.Apply<DarkShacklesPower>(choiceContext, enemy, ShacklesAmount, Owner, null);
                }
            }
            else if (FlankingAmount > 0)
            {
                await PowerCmd.Apply<SustainedFlankingPower>(choiceContext, enemy, FlankingAmount, Owner, null);
            }
        }
    }
}
