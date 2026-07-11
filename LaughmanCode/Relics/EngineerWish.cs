using BaseLib.Utils;
using Laughman.LaughmanCode.Character;

namespace Laughman.LaughmanCode.Relics;

// 工程师之愿（初始遗物的先古升级版，通过欧洛巴斯之触获得）。
//  - 每场战斗开始，力量/敏捷 = -1 + 已访问的先古之民数量（上限 3，即最高 +2）。
//  - 保留进入先古之民后战斗结束变化打击/防御牌的效果。
//  - 相比招笑避雷，基础力量/敏捷各上调 1 点，并随先古访问回升。
//  - 必须带 [Pool]（BaseLib 要求所有自定义遗物注册到池），但 Starter 稀有度不会随机掉落，
//    只能由欧洛巴斯之触替换获得；访问数由地图历史推算，天然继承。
[Pool(typeof(LaughmanRelicPool))]
public class EngineerWish : LaughingThunderBase
{
    // 基础 -1，每访问一次先古之民回升 1 点，最高 +2/+2。
    protected override int StatModifier => -1 + AncientVisits;
}
