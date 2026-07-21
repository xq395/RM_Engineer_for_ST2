using BaseLib.Abstracts;
using System.Linq;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Mechs;

// 精密工程（能力）：挂在玩家身上。只要有存活的工程机甲，你打出的 X 费牌的 X 值 +Increase。
//  - Increase 基础 1，升级 2（由卡在施加后设置）。
//  - 参考原版遗物通过 ModifyXValue 钩子给 X 费牌加值的实现。
public class PrecisionEngineeringPower : LaughmanPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    // X 值加成，由 PrecisionEngineering 卡设置（基础 1 / 升级 2）。
    public int Increase { get; set; } = 1;

    public override int ModifyXValue(CardModel card, int originalValue)
    {
        if (card.Owner != Owner.Player)
        {
            return originalValue;
        }
        // 需要有存活的工程机甲。
        bool engineerAlive = Owner.Player.Creature.Pets
            .Any(p => p.Monster is EngineerMech && !p.IsDead);
        return engineerAlive ? originalValue + Increase : originalValue;
    }
}
