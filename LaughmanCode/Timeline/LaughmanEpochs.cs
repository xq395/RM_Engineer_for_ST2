using System;
using System.Collections.Generic;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Patches;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Timeline.Scaffolding;

namespace Laughman.LaughmanCode.Timeline;

// 招笑人时间线：一条“大学生机器人战队赛季”叙事线，把机制介绍串进剧情。
// 6 个纪元横向铺开，每个各占一列（一个 era），叙事从左到右推进。
//
// 布局说明：
//  - 时间线的列按 EpochEra 整数值从小到大、从左到右排列（见 NTimelineScreen 建列逻辑）。
//    因此给每个纪元指定一个递增的自定义 era（2727..2732），紧邻原版 Invitation0(2733) 左侧，
//    叙事起点“宣讲会”在最左、结局“剑指春茧”在最右。
//  - 纪元位置由 AutoTimelineSlot(era) 特性经 RitsuLib 布局注册决定，不重写 Era/EraPosition
//    （它们是 sealed override，值来自布局注册表）。
//  - 用显式 era 而非 InEpochColumn(根)：InEpochColumn 会把纪元塞进根的同一列（纵向堆叠），
//    且执行时依赖根 layout 先注册（有排序坑）。显式 era 无跨纪元依赖，各占一列，最干净。
//  - 自定义 era（非枚举定义值）渲染正常，只是列顶的时代图标会缺失（安全回退，不影响功能/剧情）。
//
// 揭示崩溃约束：
//  - 每个卡牌纪元的 RegisterEpochCards 必须恰好 3 张（CreateCardUnlockText 硬编码遍历 3 项，
//    少于 3 会越界；多于 3 只展示前 3）。
//  - 故事必须由 LaughmanModStory 上的 [RegisterStory] 注册，否则点击带 StoryId 的纪元时
//    StoryModel.Get("LAUGHMAN") 抛异常并锁死输入。
//
// 门控约束：
//  - 只门控正常随机掉落的卡（RequireEpoch 过滤随机/奖励池）。
//  - 先古卡（冲击UL/剑指春茧）靠变身与升级替换获得、不随机掉落，绝不门控——否则会锁死其取卡途径。
//    它们只作为纪元剧情高潮出现在文案里。
//
// 解锁触发（见 Laughman.cs 角色类上的特性）：渐进式，打得越多解锁越多。
//  - 完成任意一局 → 宣讲会（根，解锁角色）
//  - 胜利一次     → 报名·联调
//  - 击杀 3 精英  → 没那么美好
//  - 击杀 5 精英  → 战队管理层
//  - 击杀 3 Boss  → 曼巴出去·冲击UL
//  - A1 胜利      → 熬夜的滋味（药水）

// —— 纪元 1：宣讲会（根，最左列 era=2727）。解锁角色。介绍机甲部署与队员方向。 ——
[RegisterStoryEpoch(typeof(LaughmanModStory))]
[AutoTimelineSlot((EpochEra)2727)]
public sealed class LaughmanCharacterEpoch
    : CharacterUnlockEpochTemplate<global::Laughman.LaughmanCode.Character.Laughman>
{
    public override string Id => "LAUGHMAN_CHARACTER_EPOCH";
    public override string StoryId => "laughman";
    public override string CustomPackedPortraitPath => LaughmanTimelineArt.Small(Id);
    public override string CustomBigPortraitPath => LaughmanTimelineArt.Big(Id);
}

// —— 纪元 2：报名·联调（era=2728）。门控招新与报名卡。 ——
[RegisterStoryEpoch(typeof(LaughmanModStory))]
[AutoTimelineSlot((EpochEra)2728)]
[RegisterEpochCards(
    typeof(Cards.RecruitmentShowcase),
    typeof(Cards.MechanicalTryout),
    typeof(Cards.VisionTryout))]
public sealed class LaughmanCardEpoch : PackDeclaredCardUnlockEpochTemplate
{
    public override string Id => "LAUGHMAN_CARD_EPOCH";
    public override string StoryId => "laughman";
    public override string CustomPackedPortraitPath => LaughmanTimelineArt.Small(Id);
    public override string CustomBigPortraitPath => LaughmanTimelineArt.Big(Id);
}

// —— 纪元 3：没那么美好（era=2729）。熬夜、清退、朋友退队。 ——
[RegisterStoryEpoch(typeof(LaughmanModStory))]
[AutoTimelineSlot((EpochEra)2729)]
[RegisterEpochCards(
    typeof(Cards.InsomniaForm),
    typeof(Cards.DismissalNotice),
    typeof(Cards.AcmRegional))]
public sealed class LaughmanHardshipEpoch : PackDeclaredCardUnlockEpochTemplate
{
    public override string Id => "LAUGHMAN_HARDSHIP_EPOCH";
    public override string StoryId => "laughman";
    public override string CustomPackedPortraitPath => LaughmanTimelineArt.Small(Id);
    public override string CustomBigPortraitPath => LaughmanTimelineArt.Big(Id);
}

// —— 纪元 4：战队管理层（era=2730）。借用、借车赚钱维持运营。 ——
[RegisterStoryEpoch(typeof(LaughmanModStory))]
[AutoTimelineSlot((EpochEra)2730)]
[RegisterEpochCards(
    typeof(Cards.RuleOverhaul),
    typeof(Cards.GuestCoach),
    typeof(Cards.FarewellDinner))]
public sealed class LaughmanManagementEpoch : PackDeclaredCardUnlockEpochTemplate
{
    public override string Id => "LAUGHMAN_MANAGEMENT_EPOCH";
    public override string StoryId => "laughman";
    public override string CustomPackedPortraitPath => LaughmanTimelineArt.Small(Id);
    public override string CustomBigPortraitPath => LaughmanTimelineArt.Big(Id);

    // 揭示“战队管理层”时，同列展开纯剧情支线“车都被占用”（嵌入式大赛/借车赚钱）。
    protected override IEnumerable<Type> ExpansionEpochTypes => new[]
    {
        typeof(LaughmanBorrowedCarsEpoch)
    };
}

// —— 纪元 5：曼巴出去·冲击UL（era=2731）。无人机新规、千辛万苦冲联赛。 ——
[RegisterStoryEpoch(typeof(LaughmanModStory))]
[AutoTimelineSlot((EpochEra)2731)]
[RegisterEpochCards(
    typeof(Cards.MambaOut),
    typeof(Cards.NewCovenantDrone),
    typeof(Cards.ChampionParade))]
public sealed class LaughmanLeagueEpoch : PackDeclaredCardUnlockEpochTemplate
{
    public override string Id => "LAUGHMAN_LEAGUE_EPOCH";
    public override string StoryId => "laughman";
    public override string CustomPackedPortraitPath => LaughmanTimelineArt.Small(Id);
    public override string CustomBigPortraitPath => LaughmanTimelineArt.Big(Id);
}

// —— 药水纪元：剑指春茧（最右列 era=2732）。门控招笑人药水（整池门控见 LaughmanPotionPool）。 ——
[RegisterStoryEpoch(typeof(LaughmanModStory))]
[AutoTimelineSlot((EpochEra)2732)]
public sealed class LaughmanPotionEpoch : PotionUnlockEpochTemplate
{
    public override string Id => "LAUGHMAN_POTION_EPOCH";
    public override string StoryId => "laughman";
    public override string CustomPackedPortraitPath => LaughmanTimelineArt.Small(Id);
    public override string CustomBigPortraitPath => LaughmanTimelineArt.Big(Id);

    // CreatePotionUnlockText 固定展示 3 项，给 3 个代表药水。
    protected override IEnumerable<Type> PotionTypes => new[]
    {
        typeof(Potions.SpareBattery),
        typeof(Potions.CoolantFlask),
        typeof(Potions.CalibrationFluid)
    };
}

// —— 支线（纯剧情，不门控任何内容）：车都被占用（与“战队管理层”同列 era=2730 纵向堆叠）。 ——
// 讲嵌入式大赛/借车赚钱的具体故事。继承 ModEpochTemplate，QueueUnlocks 用基类默认（只揭示剧情，
// 不解锁卡/药水/遗物）。由 LaughmanManagementEpoch.ExpansionEpochTypes 在其揭示时展开出现。
// 不加独立解锁触发（不随主线渐进解锁），而是跟随管理层纪元一起展开。
[RegisterStoryEpoch(typeof(LaughmanModStory))]
[AutoTimelineSlot((EpochEra)2730)]
public sealed class LaughmanBorrowedCarsEpoch : ModEpochTemplate
{
    public override string Id => "LAUGHMAN_BORROWED_CARS_EPOCH";
    public override string StoryId => "laughman";
    public override string CustomPackedPortraitPath => LaughmanTimelineArt.Small(Id);
    public override string CustomBigPortraitPath => LaughmanTimelineArt.Big(Id);
}
