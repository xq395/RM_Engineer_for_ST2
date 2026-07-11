using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Relics;

// 招笑避雷（初始遗物）：谁来谁倒霉。
//  - 每场战斗开始，力量 -1、敏捷 -1（固定，不再随先古访问数回升）。
//  - 进入先古之民后，下一场战斗结束时选择并变化一张打击牌和一张防御牌。
//  - 被欧洛巴斯之触升级时替换为「工程师之愿」。
[Pool(typeof(LaughmanRelicPool))]
public class LaughingThunder : LaughingThunderBase
{
    // 固定 -1，不随先古访问数回升。
    protected override int StatModifier => -1;

    // 欧洛巴斯之触：将初始遗物替换为先古版本「工程师之愿」。
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<EngineerWish>();
}
