# RM工程师卡牌美工总表

> 自动生成文件。数据来源：`LaughmanCode/Cards/*.cs` 与 `Laughman/localization/eng/cards.json`。
> 重新生成：在仓库根目录执行 `powershell -ExecutionPolicy Bypass -File .\export-card-art-catalog.ps1`。

- 卡牌总数：89
- 普通卡图建议尺寸：`1000x760`（可用 `500x380` 测试）
- 大图建议尺寸：`1000x760`（与普通卡图保持相同构图比例）
- 图片中不要生成卡名、数值或说明文字，游戏会自行渲染。
- `Token`、`Quest`、`Ancient` 虽不一定进入普通奖励池，仍需要卡图。

## 基础 / Basic（4 张）

### 基地补给（BASE_SUPPLY）

- 类名：`BaseSupply`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：一台机甲回复 {HealAmount:diff()} 点生命，并获得 {StrengthPower:diff()} 点 [gold]力量[/gold]。<br>若目标机甲已阵亡，改为复活至血量上限，并 [gold]晕眩[/gold] 一回合。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("HealAmount", 7m), new DynamicVar("StrengthPower", 1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars["HealAmount"].UpgradeValueBy(3m); DynamicVars["StrengthPower"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/BaseSupply.cs:17`
- 普通卡图：`Laughman/images/card_portraits/base_supply.png`
- 大图：`Laughman/images/card_portraits/big/base_supply.png`
- 美术构图备注：待填写

### 防御（ENGINEER_DEFEND）

- 类名：`EngineerDefend`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：获得 {Block:diff()} 点 [gold]格挡[/gold]。
- 数值定义：`CanonicalVars => new[] { new BlockVar(5m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); }`
- 代码：`LaughmanCode/Cards/EngineerDefend.cs:14`
- 普通卡图：`Laughman/images/card_portraits/engineer_defend.png`
- 大图：`Laughman/images/card_portraits/big/engineer_defend.png`
- 美术构图备注：待填写

### 打击（ENGINEER_STRIKE）

- 类名：`EngineerStrike`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。
- 数值定义：`CanonicalVars => new[] { new DamageVar(6m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); }`
- 代码：`LaughmanCode/Cards/EngineerStrike.cs:14`
- 普通卡图：`Laughman/images/card_portraits/engineer_strike.png`
- 大图：`Laughman/images/card_portraits/big/engineer_strike.png`
- 美术构图备注：待填写

### 3号步兵部署（INFANTRY_SUMMON）

- 类名：`InfantrySummon`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]3号步兵机甲{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）。<br>[gold]屏卫[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 13m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Innate, CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["MechHp"].UpgradeValueBy(5m); }`
- 代码：`LaughmanCode/Cards/InfantrySummon.cs:16`
- 普通卡图：`Laughman/images/card_portraits/infantry_summon.png`
- 大图：`Laughman/images/card_portraits/big/infantry_summon.png`
- 美术构图备注：待填写

## 普通 / Common（20 张）

### 弹药补给（AMMO_RESUPPLY）

- 类名：`AmmoResupply`
- 类型：技能 / `Skill`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：抽 {DrawCount:diff()} 张牌。<br>随机一台机器人获得 {StrengthPower:diff()} 点 [gold]力量[/gold]。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DynamicVar("DrawCount", 1m), new DynamicVar("StrengthPower", 1m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["DrawCount"].UpgradeValueBy(1m); DynamicVars["StrengthPower"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/AmmoResupply.cs:18`
- 普通卡图：`Laughman/images/card_portraits/ammo_resupply.png`
- 大图：`Laughman/images/card_portraits/big/ammo_resupply.png`
- 美术构图备注：待填写

### 装甲展开（ARMOR_DEPLOY）

- 类名：`ArmorDeploy`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：一个单位获得 {Block:diff()} 点 [gold]格挡[/gold]（优先选择带 [gold]屏卫[/gold] 的机器人）。<br>若你有电控，立即触发一次电控效果。
- 数值定义：`CanonicalVars => new DynamicVar[] { new BlockVar(6m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); }`
- 代码：`LaughmanCode/Cards/ArmorDeploy.cs:20`
- 普通卡图：`Laughman/images/card_portraits/armor_deploy.png`
- 大图：`Laughman/images/card_portraits/big/armor_deploy.png`
- 美术构图备注：待填写

### 分工明确（CLEAR_ROLES）

- 类名：`ClearRoles`
- 类型：技能 / `Skill`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：获得你当前层数最低的 {Types:diff()} 类队员各 1 层。若有并列，随机选择。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Types", 1m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars["Types"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:90`
- 普通卡图：`Laughman/images/card_portraits/clear_roles.png`
- 大图：`Laughman/images/card_portraits/big/clear_roles.png`
- 美术构图备注：待填写

### 协同打击（COORDINATED_FIRE）

- 类名：`CoordinatedFire`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。<br>你每控制 1 台机器人，额外造成 {PerMech:diff()} 点伤害。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(5m, ValueProp.Move), new DynamicVar("PerMech", 4m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars["PerMech"].UpgradeValueBy(2m); }`
- 代码：`LaughmanCode/Cards/CoordinatedFire.cs:17`
- 普通卡图：`Laughman/images/card_portraits/coordinated_fire.png`
- 大图：`Laughman/images/card_portraits/big/coordinated_fire.png`
- 美术构图备注：待填写

### 调试补刀（DEBUG_FINISH）

- 类名：`DebugFinish`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。<br>若目标生命低于其上限的一半，改为造成 {BigDamage:diff()} 点伤害。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(8m, ValueProp.Move), new DynamicVar("BigDamage", 12m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["BigDamage"].UpgradeValueBy(4m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:42`
- 普通卡图：`Laughman/images/card_portraits/debug_finish.png`
- 大图：`Laughman/images/card_portraits/big/debug_finish.png`
- 美术构图备注：待填写

### 电控报名（ELECTRONICS_TRYOUT）

- 类名：`ElectronicsTryout`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：获得 1 层 [gold]电控[/gold]。随机一台机器人获得 {Plating:diff()} 层 [gold]覆甲[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Plating", 3m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars["Plating"].UpgradeValueBy(2m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:31`
- 普通卡图：`Laughman/images/card_portraits/electronics_tryout.png`
- 大图：`Laughman/images/card_portraits/big/electronics_tryout.png`
- 美术构图备注：待填写

### 硬件报名（HARDWARE_TRYOUT）

- 类名：`HardwareTryout`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：获得 1 层 [gold]硬件[/gold]。抽 {DrawCount:diff()} 张牌。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("DrawCount", 2m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars["DrawCount"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:51`
- 普通卡图：`Laughman/images/card_portraits/hardware_tryout.png`
- 大图：`Laughman/images/card_portraits/big/hardware_tryout.png`
- 美术构图备注：待填写

### 干扰测试（INTERFERENCE_TEST）

- 类名：`InterferenceTest`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：所有敌人 / `AllEnemies`
- 效果：对所有敌人施加 {Weak:diff()} 层 [gold]虚弱[/gold]。若你没有机器人，这张牌费用 -1。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Weak", 1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:70`
- 普通卡图：`Laughman/images/card_portraits/interference_test.png`
- 大图：`Laughman/images/card_portraits/big/interference_test.png`
- 美术构图备注：待填写

### 机械报名（MECHANICAL_TRYOUT）

- 类名：`MechanicalTryout`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：获得 1 层 [gold]机械[/gold]。获得 {Block:diff()} 点 [gold]格挡[/gold]。
- 数值定义：`CanonicalVars => new DynamicVar[] { new BlockVar(5m, ValueProp.Move) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:20`
- 普通卡图：`Laughman/images/card_portraits/mechanical_tryout.png`
- 大图：`Laughman/images/card_portraits/big/mechanical_tryout.png`
- 美术构图备注：待填写

### 现场答辩（ON_SITE_DEFENSE）

- 类名：`OnSiteDefense`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。获得 {Block:diff()} 点 [gold]格挡[/gold]。<br>[gold]借用[/gold] 1：重复一次。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(6m, ValueProp.Move), new BlockVar(3m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars.Block.UpgradeValueBy(2m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:57`
- 普通卡图：`Laughman/images/card_portraits/on_site_defense.png`
- 大图：`Laughman/images/card_portraits/big/on_site_defense.png`
- 美术构图备注：待填写

### 零件验收（PARTS_INSPECTION）

- 类名：`PartsInspection`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。若你控制至少 1 台机器人，施加 {Vulnerable:diff()} 层 [gold]易伤[/gold]。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(8m, ValueProp.Move), new DynamicVar("Vulnerable", 1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["Vulnerable"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:61`
- 普通卡图：`Laughman/images/card_portraits/parts_inspection.png`
- 大图：`Laughman/images/card_portraits/big/parts_inspection.png`
- 美术构图备注：待填写

### 手动发弹（PRECISE_SHOT）

- 类名：`PreciseShot`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {SmallDamage:diff()} 点伤害。<br>然后造成 {Damage:diff()} 点伤害。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(10m, ValueProp.Move), new DynamicVar("SmallDamage", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["SmallDamage"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/PreciseShot.cs:15`
- 普通卡图：`Laughman/images/card_portraits/precise_shot.png`
- 大图：`Laughman/images/card_portraits/big/precise_shot.png`
- 美术构图备注：待填写

### 召回通知（RECALL_NOTICE）

- 类名：`RecallNotice`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：[gold]归队[/gold] {Return}。<br>抽 {Draw} 张牌。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Return", 1m), new DynamicVar("Draw", 1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Return"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:456`
- 普通卡图：`Laughman/images/card_portraits/recall_notice.png`
- 大图：`Laughman/images/card_portraits/big/recall_notice.png`
- 美术构图备注：待填写

### 招新展示（RECRUITMENT_SHOWCASE）

- 类名：`RecruitmentShowcase`
- 类型：攻击 / `Attack`
- 费用：`0`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。<br>[gold]借用[/gold] 1：随机获得 {Members:diff()} 层队员。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(4m, ValueProp.Move), new DynamicVar("Members", 1m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars["Members"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:501`
- 普通卡图：`Laughman/images/card_portraits/recruitment_showcase.png`
- 大图：`Laughman/images/card_portraits/big/recruitment_showcase.png`
- 美术构图备注：待填写

### 维修清单（REPAIR_CHECKLIST）

- 类名：`RepairChecklist`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：获得 {Block:diff()} 点 [gold]格挡[/gold]。<br>若你有机械，立即触发一次机械效果。
- 数值定义：`CanonicalVars => new DynamicVar[] { new BlockVar(6m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:80`
- 普通卡图：`Laughman/images/card_portraits/repair_checklist.png`
- 大图：`Laughman/images/card_portraits/big/repair_checklist.png`
- 美术构图备注：待填写

### 例行训练（ROUTINE_PRACTICE）

- 类名：`RoutinePractice`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。抽 1 张牌。<br>若你控制至少 1 台机器人，再抽 1 张牌。
- 数值定义：`CanonicalVars => new[] { new DamageVar(8m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:23`
- 普通卡图：`Laughman/images/card_portraits/routine_practice.png`
- 大图：`Laughman/images/card_portraits/big/routine_practice.png`
- 美术构图备注：待填写

### 趴窝哨兵（STALLED_SENTINEL）

- 类名：`StalledSentinel`
- 类型：技能 / `Skill`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]哨兵机甲{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）。它获得 {Penalty:diff()} 点 [gold]力量[/gold]和{Penalty:diff()} 点 [gold]敏捷[/gold]。<br>[gold]屏卫[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 9m), new DynamicVar("Penalty", -3m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["Penalty"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/SentinelDeployment.cs:39`
- 普通卡图：`Laughman/images/card_portraits/stalled_sentinel.png`
- 大图：`Laughman/images/card_portraits/big/stalled_sentinel.png`
- 美术构图备注：待填写

### 泼洒弹仓（SUPPRESSING_FIRE）

- 类名：`SuppressingFire`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：所有敌人 / `AllEnemies`
- 效果：对所有敌人造成 {Damage} 点伤害 {HitCount:diff()} 次。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(2m, ValueProp.Move), new DynamicVar("HitCount", 3m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars["HitCount"].UpgradeValueBy(2m); }`
- 代码：`LaughmanCode/Cards/SuppressingFire.cs:15`
- 普通卡图：`Laughman/images/card_portraits/suppressing_fire.png`
- 大图：`Laughman/images/card_portraits/big/suppressing_fire.png`
- 美术构图备注：待填写

### 战术转移（TACTICAL_MOVE）

- 类名：`TacticalMove`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：你和一台机甲各获得 {Block:diff()} 点 [gold]格挡[/gold]（优先选择带 [gold]屏卫[/gold] 的机甲）。<br>抽 {DrawCount:diff()} 张牌。
- 数值定义：`CanonicalVars => new DynamicVar[] { new BlockVar(4m, ValueProp.Move), new DynamicVar("DrawCount", 1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Block.UpgradeValueBy(1m); DynamicVars["DrawCount"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/TacticalMove.cs:17`
- 普通卡图：`Laughman/images/card_portraits/tactical_move.png`
- 大图：`Laughman/images/card_portraits/big/tactical_move.png`
- 美术构图备注：待填写

### 视觉报名（VISION_TRYOUT）

- 类名：`VisionTryout`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。获得 1 层 [gold]视觉[/gold]。
- 数值定义：`CanonicalVars => new[] { new DamageVar(7m, ValueProp.Move) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:41`
- 普通卡图：`Laughman/images/card_portraits/vision_tryout.png`
- 大图：`Laughman/images/card_portraits/big/vision_tryout.png`
- 美术构图备注：待填写

## 罕见 / Uncommon（34 张）

### AI哨兵（AI_SENTINEL）

- 类名：`AiSentinel`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：改变哨兵的行动：敌人将攻击时，获得 {Block:diff()} 点 [gold]格挡[/gold]和[gold]小陀螺[/gold]；否则攻击 5 点 {HitCount:diff()} 次。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DynamicVar("Block", 10m), new DynamicVar("HitCount", 5m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["Block"].UpgradeValueBy(2m); DynamicVars["HitCount"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:134`
- 普通卡图：`Laughman/images/card_portraits/ai_sentinel.png`
- 大图：`Laughman/images/card_portraits/big/ai_sentinel.png`
- 美术构图备注：待填写

### 交叉火力（BROADCAST_CUT）

- 类名：`BroadcastCut`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：所有敌人 / `AllEnemies`
- 效果：对所有敌人造成 {Damage:diff()} 点伤害。每有一名敌人被击杀，就重复此效果。<br>[gold]借用[/gold] 1：此伤害翻倍。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(7m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:146`
- 普通卡图：`Laughman/images/card_portraits/broadcast_cut.png`
- 大图：`Laughman/images/card_portraits/big/broadcast_cut.png`
- 美术构图备注：待填写

### 集火指令（COLLECTIVE_FIRE）

- 类名：`CollectiveFire`
- 类型：攻击 / `Attack`
- 费用：`2`
- 目标：单个敌人 / `AnyEnemy`
- 效果：对一个敌人造成 {Damage:diff()} 点伤害并 [gold]集火标记[/gold] {Turns:diff()} 回合。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(7m, ValueProp.Move), new DynamicVar("Turns", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(4m); DynamicVars["Turns"].UpgradeValueBy(2m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:594`
- 普通卡图：`Laughman/images/card_portraits/collective_fire.png`
- 大图：`Laughman/images/card_portraits/big/collective_fire.png`
- 美术构图备注：待填写

### 测试打击（COORDINATED_STRIKE）

- 类名：`CoordinatedStrike`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。<br>你最近部署的机器人立即行动一次，并优先攻击该目标。
- 数值定义：`CanonicalVars => new[] { new DamageVar(6m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:768`
- 普通卡图：`Laughman/images/card_portraits/coordinated_strike.png`
- 大图：`Laughman/images/card_portraits/big/coordinated_strike.png`
- 美术构图备注：待填写

### 快速培训（CRASH_COURSE）

- 类名：`CrashCourse`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：选择一张 0 费、获得 {IfUpgraded:show:3\|2} 层队员的能力牌加入手牌。
- 数值定义：`-`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`-`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:145`
- 普通卡图：`Laughman/images/card_portraits/crash_course.png`
- 大图：`Laughman/images/card_portraits/big/crash_course.png`
- 美术构图备注：待填写

### 清退通知（DISMISSAL_NOTICE）

- 类名：`DismissalNotice`
- 类型：技能 / `Skill`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：使层数最多的一类队员减少 1 层。获得 {Energy:diff()} 点能量。<br>选择并 [gold]消耗[/gold] 至多 {Cards:diff()} 张其他手牌。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DynamicVar("Energy", 2m), new DynamicVar("Cards", 2m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["Energy"].UpgradeValueBy(1m); DynamicVars["Cards"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:547`
- 普通卡图：`Laughman/images/card_portraits/dismissal_notice.png`
- 大图：`Laughman/images/card_portraits/big/dismissal_notice.png`
- 美术构图备注：待填写

### 无人机部署（DRONE_DEPLOYMENT）

- 类名：`DroneDeployment`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]无人机{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）：消耗金币，进行多段攻击。<br>[gold]飞行[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 9m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["MechHp"].UpgradeValueBy(3m); _hitCount = 5; }`
- 代码：`LaughmanCode/Cards/DroneDeployment.cs:14`
- 普通卡图：`Laughman/images/card_portraits/drone_deployment.png`
- 大图：`Laughman/images/card_portraits/big/drone_deployment.png`
- 美术构图备注：待填写

### 嵌入式比赛（EMBEDDED_CONTEST）

- 类名：`EmbeddedContest`
- 类型：攻击 / `Attack`
- 费用：`X`
- 目标：单个敌人 / `AnyEnemy`
- 效果：[gold]借用[/gold] 1，重复 X 次。<br>每次借用成功，造成 {Damage:diff()} 点伤害并获得 {Block:diff()} 点 [gold]格挡[/gold]。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(10m, ValueProp.Move), new BlockVar(6m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars.Block.UpgradeValueBy(2m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:200`
- 普通卡图：`Laughman/images/card_portraits/embedded_contest.png`
- 大图：`Laughman/images/card_portraits/big/embedded_contest.png`
- 美术构图备注：待填写

### 紧急召回（EMERGENCY_RECALL）

- 类名：`EmergencyRecall`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：[gold]归队[/gold] {Return}。该机器人获得 [gold]小陀螺[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Return", 3m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => EnergyCost.UpgradeBy(-1);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:470`
- 普通卡图：`Laughman/images/card_portraits/emergency_recall.png`
- 大图：`Laughman/images/card_portraits/big/emergency_recall.png`
- 美术构图备注：待填写

### 工程部署（ENGINEER_DEPLOYMENT）

- 类名：`EngineerDeployment`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]工程机甲{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）：提供 [gold]格挡[/gold]、金币和机器人 [gold]力量[/gold]。行动次数有限。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 14m), new DynamicVar("Block", 4m), new DynamicVar("Gold", 2m), new DynamicVar("Actions", 6m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["MechHp"].UpgradeValueBy(3m); DynamicVars["Block"].UpgradeValueBy(3m); DynamicVars["Gold"].UpgradeValueBy(1m); DynamicVars["Actions"].UpgradeValueBy(2m); }`
- 代码：`LaughmanCode/Cards/EngineerDeployment.cs:15`
- 普通卡图：`Laughman/images/card_portraits/engineer_deployment.png`
- 大图：`Laughman/images/card_portraits/big/engineer_deployment.png`
- 美术构图备注：待填写

### 现场维修（FIELD_REPAIR）

- 类名：`FieldRepair`
- 类型：技能 / `Skill`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：治疗你所有机器人 {HealAmount:diff()} 点。<br>获得 {Block:diff()} 点 [gold]格挡[/gold]。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DynamicVar("HealAmount", 7m), new BlockVar(4m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars["HealAmount"].UpgradeValueBy(3m); DynamicVars.Block.UpgradeValueBy(2m); }`
- 代码：`LaughmanCode/Cards/FieldRepair.cs:17`
- 普通卡图：`Laughman/images/card_portraits/field_repair.png`
- 大图：`Laughman/images/card_portraits/big/field_repair.png`
- 美术构图备注：待填写

### 专注打击（FOCUSED_STRIKE）

- 类名：`FocusedStrike`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。<br>你每控制 1 台机器人，此伤害 -3。
- 数值定义：`CanonicalVars => new[] { new DamageVar(21m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:752`
- 普通卡图：`Laughman/images/card_portraits/focused_strike.png`
- 大图：`Laughman/images/card_portraits/big/focused_strike.png`
- 美术构图备注：待填写

### 全军出击（FULL_ASSAULT）

- 类名：`FullAssault`
- 类型：攻击 / `Attack`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：你的所有机器人立即行动一次。{IfUpgraded:show:<br>每台机器人使你获得 2 点 [gold]格挡[/gold]。}
- 数值定义：`CanonicalVars => new[] { new DynamicVar("BlockPerMech", 0m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["BlockPerMech"].UpgradeValueBy(2m); }`
- 代码：`LaughmanCode/Cards/FullAssault.cs:17`
- 普通卡图：`Laughman/images/card_portraits/full_assault.png`
- 大图：`Laughman/images/card_portraits/big/full_assault.png`
- 美术构图备注：待填写

### 外援教练（GUEST_COACH）

- 类名：`GuestCoach`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：你的所有机器人获得 {Strength} 点 [gold]力量[/gold]。<br>[gold]借用[/gold] 1：改为获得 {BorrowStrength:diff()} 点力量和 {BorrowDex:diff()} 点敏捷。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DynamicVar("Strength", 2m), new DynamicVar("BorrowStrength", 3m), new DynamicVar("BorrowDex", 1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars["BorrowStrength"].UpgradeValueBy(1m); DynamicVars["BorrowDex"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:121`
- 普通卡图：`Laughman/images/card_portraits/guest_coach.png`
- 大图：`Laughman/images/card_portraits/big/guest_coach.png`
- 美术构图备注：待填写

### 指令：小陀螺（GYRO_SPIN_COMMAND）

- 类名：`GyroSpinCommand`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：你的步兵、哨兵和英雄获得 {Turns:diff()} 层 [gold]小陀螺[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Turns", 1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Turns"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:125`
- 普通卡图：`Laughman/images/card_portraits/gyro_spin_command.png`
- 大图：`Laughman/images/card_portraits/big/gyro_spin_command.png`
- 美术构图备注：待填写

### 英雄部署（HERO_DEPLOYMENT）

- 类名：`HeroDeployment`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]英雄机甲{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 15m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["MechHp"].UpgradeValueBy(5m); }`
- 代码：`LaughmanCode/Cards/HeroDeployment.cs:14`
- 普通卡图：`Laughman/images/card_portraits/hero_deployment.png`
- 大图：`Laughman/images/card_portraits/big/hero_deployment.png`
- 美术构图备注：待填写

### 4号步兵部署（INFANTRY_NO4）

- 类名：`InfantryNo4`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：召唤一台独立的 [gold]4号步兵机甲{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）。<br>[gold]屏卫[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 13m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars["MechHp"].UpgradeValueBy(5m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:106`
- 普通卡图：`Laughman/images/card_portraits/infantry_no4.png`
- 大图：`Laughman/images/card_portraits/big/infantry_no4.png`
- 美术构图备注：待填写

### 吊射指令（LOB_FIRE_COMMAND）

- 类名：`LobFireCommand`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：使你的 [gold]英雄机甲[/gold] 改为重击 {LobDamage:diff()} 点 → [gold]休眠[/gold]的循环。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("LobDamage", 35m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["LobDamage"].UpgradeValueBy(7m); }`
- 代码：`LaughmanCode/Cards/LobFireCommand.cs:15`
- 普通卡图：`Laughman/images/card_portraits/lob_fire_command.png`
- 大图：`Laughman/images/card_portraits/big/lob_fire_command.png`
- 美术构图备注：待填写

### 曼巴出去！（MAMBA_OUT）

- 类名：`MambaOut`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：所有敌人 / `AllEnemies`
- 效果：只有在无人机存活时才能打出。<br>摧毁无人机。对所有敌人造成 {Damage:diff()} 点伤害，并额外造成其力量 8 倍的伤害（最多 24）。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(24m, ValueProp.Move), new DynamicVar("StrengthMultiplier", 8m), new DynamicVar("BonusCap", 24m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => EnergyCost.UpgradeBy(-1);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:174`
- 普通卡图：`Laughman/images/card_portraits/mamba_out.png`
- 大图：`Laughman/images/card_portraits/big/mamba_out.png`
- 美术构图备注：待填写

### 机甲冷却（MECH_COOLDOWN）

- 类名：`MechCooldown`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：获得 {Block:diff()} 点 [gold]格挡[/gold]。<br>[gold]借用[/gold] 1：从弃牌堆选择 1 张牌置于抽牌堆顶，重复 {Cards:diff()} 次。
- 数值定义：`CanonicalVars => new DynamicVar[] { new BlockVar(8m, ValueProp.Move), new DynamicVar("Cards", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Block.UpgradeValueBy(2m); DynamicVars["Cards"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:565`
- 普通卡图：`Laughman/images/card_portraits/mech_cooldown.png`
- 大图：`Laughman/images/card_portraits/big/mech_cooldown.png`
- 美术构图备注：待填写

### 过载协议（OVERCLOCK）

- 类名：`Overclock`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：你的所有机器人各获得 {StrengthPower:diff()} 点 [gold]力量[/gold]，并受到 {SelfDamage} 点伤害。
- 数值定义：`CanonicalVars => new DynamicVar[] { new PowerVar<StrengthPower>(2m), new DynamicVar("SelfDamage", 3m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["StrengthPower"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Overclock.cs:18`
- 普通卡图：`Laughman/images/card_portraits/overclock.png`
- 大图：`Laughman/images/card_portraits/big/overclock.png`
- 美术构图备注：待填写

### 买活（PAY_TO_RESPAWN）

- 类名：`PayToRespawn`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：花费 6 金币。复活一台步兵、英雄或哨兵，并使其获得 [gold]小陀螺[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("ReviveHp", 10m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { EnergyCost.UpgradeBy(-1); }`
- 代码：`LaughmanCode/Cards/PayToRespawn.cs:18`
- 普通卡图：`Laughman/images/card_portraits/pay_to_respawn.png`
- 大图：`Laughman/images/card_portraits/big/pay_to_respawn.png`
- 美术构图备注：待填写

### 赛前联调（PRE_MATCH_CALIBRATION）

- 类名：`PreMatchCalibration`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：立即触发一次你的所有队员效果。
- 数值定义：`-`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => EnergyCost.UpgradeBy(-1);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:116`
- 普通卡图：`Laughman/images/card_portraits/pre_match_calibration.png`
- 大图：`Laughman/images/card_portraits/big/pre_match_calibration.png`
- 美术构图备注：待填写

### 精确制导（PRECISION_GUIDANCE）

- 类名：`PrecisionGuidance`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：你的飞镖机器人伤害无视格挡，并且造成伤害 +{MultiplierPct:diff()}%。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MultiplierPct", 25m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["MultiplierPct"].UpgradeValueBy(25m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:268`
- 普通卡图：`Laughman/images/card_portraits/precision_guidance.png`
- 大图：`Laughman/images/card_portraits/big/precision_guidance.png`
- 美术构图备注：待填写

### 备战复盘（PREP_REVIEW）

- 类名：`PrepReview`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：选择你当前层数最多的一类队员，立即触发该类队员效果 {TriggerCount:diff()} 次。若并列则随机。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("TriggerCount", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["TriggerCount"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:300`
- 普通卡图：`Laughman/images/card_portraits/prep_review.png`
- 大图：`Laughman/images/card_portraits/big/prep_review.png`
- 美术构图备注：待填写

### 雷达部署（RADAR_LOCK）

- 类名：`RadarLock`
- 类型：能力 / `Power`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：部署雷达。每回合开始时，随机一名敌人获得 1 层 [gold]易伤[/gold]{IfUpgraded:show:和 1 层 [gold]虚弱[/gold]}。
- 数值定义：`-`
- 关键词：`-`
- 升级实现：`OnUpgrade() { _appliesWeak = true; }`
- 代码：`LaughmanCode/Cards/RadarLock.cs:15`
- 普通卡图：`Laughman/images/card_portraits/radar_lock.png`
- 大图：`Laughman/images/card_portraits/big/radar_lock.png`
- 美术构图备注：待填写

### 飞坡（RAMP_JUMP）

- 类名：`RampJump`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：一台步兵、哨兵或英雄获得 [gold]飞坡[/gold]。
- 数值定义：`-`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { EnergyCost.UpgradeBy(-1); }`
- 代码：`LaughmanCode/Cards/RampJump.cs:16`
- 普通卡图：`Laughman/images/card_portraits/ramp_jump.png`
- 大图：`Laughman/images/card_portraits/big/ramp_jump.png`
- 美术构图备注：待填写

### 学校展出（SCHOOL_EXHIBITION）

- 类名：`SchoolExhibition`
- 类型：技能 / `Skill`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：[gold]借用[/gold] 1。<br>若借用成功，获得 {Energy:diff()} 点能量。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Energy", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Energy"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:86`
- 普通卡图：`Laughman/images/card_portraits/school_exhibition.png`
- 大图：`Laughman/images/card_portraits/big/school_exhibition.png`
- 美术构图备注：待填写

### 哨兵部署（SENTINEL_DEPLOYMENT）

- 类名：`SentinelDeployment`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]哨兵机甲{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）。<br>[gold]屏卫[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 9m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["MechHp"].UpgradeValueBy(4m); }`
- 代码：`LaughmanCode/Cards/SentinelDeployment.cs:16`
- 普通卡图：`Laughman/images/card_portraits/sentinel_deployment.png`
- 大图：`Laughman/images/card_portraits/big/sentinel_deployment.png`
- 美术构图备注：待填写

### 飞镖部署（SWARM_DEPLOYMENT）

- 类名：`SwarmDeployment`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]飞镖机器人{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）：[gold]休眠[/gold] → 连续 4 次重击 25 点。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 1m), new DynamicVar("ChargeTurns", 2m), new DynamicVar("BurstDamage", 25m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["MechHp"].UpgradeValueBy(1m); DynamicVars["ChargeTurns"].UpgradeValueBy(-1m); }`
- 代码：`LaughmanCode/Cards/SwarmDeployment.cs:15`
- 普通卡图：`Laughman/images/card_portraits/swarm_deployment.png`
- 大图：`Laughman/images/card_portraits/big/swarm_deployment.png`
- 美术构图备注：待填写

### 同步标定（SYNC_CALIBRATION）

- 类名：`SyncCalibration`
- 类型：攻击 / `Attack`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。你每有 1 层队员，额外造成 {PerMember:diff()} 点伤害。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(4m, ValueProp.Move), new DynamicVar("PerMember", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars["PerMember"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:249`
- 普通卡图：`Laughman/images/card_portraits/sync_calibration.png`
- 大图：`Laughman/images/card_portraits/big/sync_calibration.png`
- 美术构图备注：待填写

### 感谢开源（THANKS_OPEN_SOURCE）

- 类名：`ThanksOpenSource`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：从 3 张随机{IfUpgraded:show:升级后的}部署牌中选择 1 张，本回合免费打出。
- 数值定义：`-`
- 关键词：`-`
- 升级实现：`-`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:278`
- 普通卡图：`Laughman/images/card_portraits/thanks_open_source.png`
- 大图：`Laughman/images/card_portraits/big/thanks_open_source.png`
- 美术构图备注：待填写

### 集训封闭（TRAINING_CAMP）

- 类名：`TrainingCamp`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：抽 {Draw:diff()} 张牌。<br>[gold]借用[/gold] 1：额外抽 {BonusDraw} 张牌并获得 1 点能量。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DynamicVar("Draw", 2m), new DynamicVar("BonusDraw", 1m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:102`
- 普通卡图：`Laughman/images/card_portraits/training_camp.png`
- 大图：`Laughman/images/card_portraits/big/training_camp.png`
- 美术构图备注：待填写

### 弱点标注（WEAKPOINT_MARKING）

- 类名：`WeakpointMarking`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：单个敌人 / `AnyEnemy`
- 效果：对一名敌人施加 {Vulnerable:diff()} 层 [gold]易伤[/gold]和 {Weak:diff()} 层 [gold]虚弱[/gold]。若你有视觉，立即触发一次视觉效果。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Vulnerable", 2m), new DynamicVar("Weak", 1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars["Vulnerable"].UpgradeValueBy(1m); DynamicVars["Weak"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:259`
- 普通卡图：`Laughman/images/card_portraits/weakpoint_marking.png`
- 大图：`Laughman/images/card_portraits/big/weakpoint_marking.png`
- 美术构图备注：待填写

## 稀有 / Rare（22 张）

### ACM长期集训（ACM_REGIONAL）

- 类名：`AcmRegional`
- 类型：攻击 / `Attack`
- 费用：`X`
- 目标：所有敌人 / `AllEnemies`
- 效果：一台机器人获得 X 层 [gold]借用[/gold]。<br>对所有敌人造成 X{IfUpgraded:show:+1} × {Damage} 点伤害。X 至少为 4 时翻倍。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(7m, ValueProp.Move), new DynamicVar("BonusX", 0m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars["BonusX"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:246`
- 普通卡图：`Laughman/images/card_portraits/acm_regional.png`
- 大图：`Laughman/images/card_portraits/big/acm_regional.png`
- 美术构图备注：待填写

### 熬夜调车（ALL_NIGHTER_TUNING）

- 类名：`AllNighterTuning`
- 类型：攻击 / `Attack`
- 费用：`0`
- 目标：单个敌人 / `AnyEnemy`
- 效果：获得 1 点能量。失去 1 点生命，造成 {Damage:diff()} 点伤害，随机一辆吃力量的机器人获得 1 点 [gold]力量[/gold]。<br>[gold]借用[/gold] 1：重复一次。
- 数值定义：`CanonicalVars => new[] { new DamageVar(6m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:361`
- 普通卡图：`Laughman/images/card_portraits/all_nighter_tuning.png`
- 大图：`Laughman/images/card_portraits/big/all_nighter_tuning.png`
- 美术构图备注：待填写

### 年度招新（ANNUAL_RECRUITMENT）

- 类名：`AnnualRecruitment`
- 类型：能力 / `Power`
- 费用：`3`
- 目标：自身 / `Self`
- 效果：获得 1 层机械、电控、视觉和硬件。{IfUpgraded:show:<br>下回合再获得一次。}
- 数值定义：`-`
- 关键词：`-`
- 升级实现：`OnUpgrade() => _delaysSecond = true;`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:317`
- 普通卡图：`Laughman/images/card_portraits/annual_recruitment.png`
- 大图：`Laughman/images/card_portraits/big/annual_recruitment.png`
- 美术构图备注：待填写

### 信火一体飞镖（BEACON_DART）

- 类名：`BeaconDart`
- 类型：技能 / `Skill`
- 费用：`3`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]飞镖机器人{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）：[gold]休眠[/gold]时施加[gold]集火标记[/gold]，随后连续 4 次重击 25 点。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 1m), new DynamicVar("ChargeTurns", 2m), new DynamicVar("BurstDamage", 25m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["MechHp"].UpgradeValueBy(1m); DynamicVars["ChargeTurns"].UpgradeValueBy(-1m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:651`
- 普通卡图：`Laughman/images/card_portraits/beacon_dart.png`
- 大图：`Laughman/images/card_portraits/big/beacon_dart.png`
- 美术构图备注：待填写

### 冠军巡游（CHAMPION_PARADE）

- 类名：`ChampionParade`
- 类型：攻击 / `Attack`
- 费用：`3`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。你每控制 1 台机器人，额外造成 {PerMech} 点伤害。<br>[gold]借用[/gold] 1：改为攻击所有敌人。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(20m, ValueProp.Move), new DynamicVar("PerMech", 6m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:336`
- 普通卡图：`Laughman/images/card_portraits/champion_parade.png`
- 大图：`Laughman/images/card_portraits/big/champion_parade.png`
- 美术构图备注：待填写

### 散伙饭（FAREWELL_DINNER）

- 类名：`FarewellDinner`
- 类型：攻击 / `Attack`
- 费用：`2`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。<br>[gold]借用[/gold] 1：清除你的所有队员，每因此清除 1 层，额外造成 {PerMember:diff()} 点伤害。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(10m, ValueProp.Move), new DynamicVar("PerMember", 4m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["PerMember"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:518`
- 普通卡图：`Laughman/images/card_portraits/farewell_dinner.png`
- 大图：`Laughman/images/card_portraits/big/farewell_dinner.png`
- 美术构图备注：待填写

### 高速穿插（HIGH_SPEED_FLANK）

- 类名：`HighSpeedFlank`
- 类型：攻击 / `Attack`
- 费用：`2`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。你的所有机器人获得 [gold]小陀螺[/gold]。
- 数值定义：`CanonicalVars => new[] { new DamageVar(10m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:474`
- 普通卡图：`Laughman/images/card_portraits/high_speed_flank.png`
- 大图：`Laughman/images/card_portraits/big/high_speed_flank.png`
- 美术构图备注：待填写

### 我即浪潮（I_AM_THE_WAVE）

- 类名：`IAmTheWave`
- 类型：攻击 / `Attack`
- 费用：`3`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]英雄机甲{IfUpgraded:show:+}[/gold]：每回合攻击所有敌人两次，然后 [gold]借用[/gold] 1（隐身一回合）。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 25m) };`
- 关键词：`CanonicalKeywords => _exhausts ? new[] { CardKeyword.Exhaust } : Array.Empty<CardKeyword>();`
- 升级实现：`OnUpgrade() => _exhausts = false;`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:612`
- 普通卡图：`Laughman/images/card_portraits/i_am_the_wave.png`
- 大图：`Laughman/images/card_portraits/big/i_am_the_wave.png`
- 美术构图备注：待填写

### 熬夜形态（INSOMNIA_FORM）

- 类名：`InsomniaForm`
- 类型：能力 / `Power`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：打出时失去 1 点 [gold]力量[/gold]和 1 点 [gold]敏捷[/gold]。每回合开始时，失去 1 点生命，获得 1 点能量，抽 1 张牌。
- 数值定义：`-`
- 关键词：`CanonicalKeywords => IsUpgraded ? new[] { CardKeyword.Innate } : Array.Empty<CardKeyword>();`
- 升级实现：`-`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:363`
- 普通卡图：`Laughman/images/card_portraits/insomnia_form.png`
- 大图：`Laughman/images/card_portraits/big/insomnia_form.png`
- 美术构图备注：待填写

### 百炼老兵（LOYAL_GUARD）

- 类名：`LoyalGuard`
- 类型：技能 / `Skill`
- 费用：`X`
- 目标：自身 / `Self`
- 效果：重复 X{IfUpgraded:show:+1} 次。若3号步兵不存在，首次改为召唤它。<br>否则其提升并回复 6 点生命、获得 1 点 [gold]力量[/gold]和[gold]敏捷[/gold]，你获得 1 层 [gold]覆甲[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Extra", 0m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Extra"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:326`
- 普通卡图：`Laughman/images/card_portraits/loyal_guard.png`
- 大图：`Laughman/images/card_portraits/big/loyal_guard.png`
- 美术构图备注：待填写

### 全国大学生数学建模（MATH_MODELING）

- 类名：`MathModeling`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：抽 {Draw:diff()} 张牌，本回合它们的费用 -1。<br>[gold]借用[/gold] 1：获得 1 点能量。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Draw", 3m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:224`
- 普通卡图：`Laughman/images/card_portraits/math_modeling.png`
- 大图：`Laughman/images/card_portraits/big/math_modeling.png`
- 美术构图备注：待填写

### 视觉开源（MECHA_PROTOCOL）

- 类名：`MechaProtocol`
- 类型：能力 / `Power`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：每回合开始时，你的所有机器人各获得 1 点 [gold]力量[/gold]。
- 数值定义：`-`
- 关键词：`-`
- 升级实现：`OnUpgrade() { EnergyCost.UpgradeBy(-1); }`
- 代码：`LaughmanCode/Cards/MechaProtocol.cs:14`
- 普通卡图：`Laughman/images/card_portraits/mecha_protocol.png`
- 大图：`Laughman/images/card_portraits/big/mecha_protocol.png`
- 美术构图备注：待填写

### 单刀直入（MVP_OF_THE_MATCH）

- 类名：`MvpOfTheMatch`
- 类型：攻击 / `Attack`
- 费用：`2`
- 目标：单个敌人 / `AnyEnemy`
- 效果：造成 {Damage:diff()} 点伤害。<br>[gold]借用[/gold] 1 台步兵：改为造成双倍伤害，再额外增加被借步兵的力量。该步兵受到 {InfantryDamage} 点伤害。
- 数值定义：`CanonicalVars => new DynamicVar[] { new DamageVar(16m, ValueProp.Move), new DynamicVar("InfantryDamage", 13m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:431`
- 普通卡图：`Laughman/images/card_portraits/mvp_of_the_match.png`
- 大图：`Laughman/images/card_portraits/big/mvp_of_the_match.png`
- 美术构图备注：待填写

### 新约无人机（NEW_COVENANT_DRONE）

- 类名：`NewCovenantDrone`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：召唤一台 [gold]新约无人机{IfUpgraded:show:+}[/gold]（生命值 {MechHp}）：临时偷取队友 {StrengthSteal:diff()} 点力量并给予 {Shield:diff()} 点护盾。<br>[gold]飞行[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 9m), new DynamicVar("Shield", 5m), new DynamicVar("StrengthSteal", 3m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars["Shield"].UpgradeValueBy(2m); DynamicVars["StrengthSteal"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:631`
- 普通卡图：`Laughman/images/card_portraits/new_covenant_drone.png`
- 大图：`Laughman/images/card_portraits/big/new_covenant_drone.png`
- 美术构图备注：待填写

### 赛前调度会议（PRE_MATCH_DISPATCH_MEETING）

- 类名：`PreMatchDispatchMeeting`
- 类型：技能 / `Skill`
- 费用：`1`
- 目标：自身 / `Self`
- 效果：[gold]归队[/gold] 1，重复 {Repeats} 次。<br>每当一台机器人因此完全归队，抽 1 张牌。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Repeats", 3m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => EnergyCost.UpgradeBy(-1);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:486`
- 普通卡图：`Laughman/images/card_portraits/pre_match_dispatch_meeting.png`
- 大图：`Laughman/images/card_portraits/big/pre_match_dispatch_meeting.png`
- 美术构图备注：待填写

### 精密工程（PRECISION_ENGINEERING）

- 类名：`PrecisionEngineering`
- 类型：能力 / `Power`
- 费用：`3`
- 目标：自身 / `Self`
- 效果：召唤一台强化的 [gold]工程机甲{IfUpgraded:show:+}[/gold]（生命值 {MechHp:diff()}）：提供 [gold]格挡[/gold]、金币和机器人 [gold]力量[/gold]。<br>其存活时，你的 X 值 +{XBonus:diff()}。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("MechHp", 14m), new DynamicVar("Block", 4m), new DynamicVar("Gold", 2m), new DynamicVar("Actions", 6m), new DynamicVar("XBonus", 1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars["MechHp"].UpgradeValueBy(3m); DynamicVars["Block"].UpgradeValueBy(3m); DynamicVars["Gold"].UpgradeValueBy(1m); DynamicVars["Actions"].UpgradeValueBy(2m); DynamicVars["XBonus"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:672`
- 普通卡图：`Laughman/images/card_portraits/precision_engineering.png`
- 大图：`Laughman/images/card_portraits/big/precision_engineering.png`
- 美术构图备注：待填写

### 精准兑换（PRECISION_EXCHANGE）

- 类名：`PrecisionExchange`
- 类型：技能 / `Skill`
- 费用：`X`
- 目标：自身 / `Self`
- 效果：只能在你控制工程机器人时打出。获得 {IfUpgraded:show:X\|X-1} 层 [gold]兑矿[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Bonus", -1m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Bonus"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:371`
- 普通卡图：`Laughman/images/card_portraits/precision_exchange.png`
- 大图：`Laughman/images/card_portraits/big/precision_exchange.png`
- 美术构图备注：待填写

### 机器人大赛决赛（ROBOTICS_FINALS）

- 类名：`RoboticsFinals`
- 类型：攻击 / `Attack`
- 费用：`3`
- 目标：自身 / `Self`
- 效果：你的所有机器人立即行动 {Actions:diff()} 次。<br>[gold]借用[/gold] 1：额外行动一次。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Actions", 1m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars["Actions"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:268`
- 普通卡图：`Laughman/images/card_portraits/robotics_finals.png`
- 大图：`Laughman/images/card_portraits/big/robotics_finals.png`
- 美术构图备注：待填写

### 时间管理大师（RULE_OVERHAUL）

- 类名：`RuleOverhaul`
- 类型：能力 / `Power`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：每台机器人每回合第一次被 [gold]借用[/gold] 时，抽 1 张牌，获得 {Block:diff()} 点 [gold]格挡[/gold]，并 [gold]归队[/gold] 1。
- 数值定义：`CanonicalVars => new[] { new BlockVar(1m, ValueProp.Move) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:417`
- 普通卡图：`Laughman/images/card_portraits/rule_overhaul.png`
- 大图：`Laughman/images/card_portraits/big/rule_overhaul.png`
- 美术构图备注：待填写

### 天眼雷达（SKY_EYE_RADAR）

- 类名：`SkyEyeRadar`
- 类型：能力 / `Power`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：部署天眼雷达。<br>机器人行动前，攻击意图的敌人获得 {Shackles:diff()} 层 [gold]镣铐[/gold]；其他敌人在本轮机器人齐射中获得 {Flanking:diff()} 层 [gold]夹击[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Shackles", 2m), new DynamicVar("Flanking", 0m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() { DynamicVars["Shackles"].UpgradeValueBy(1m); DynamicVars["Flanking"].UpgradeValueBy(1m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:737`
- 普通卡图：`Laughman/images/card_portraits/sky_eye_radar.png`
- 大图：`Laughman/images/card_portraits/big/sky_eye_radar.png`
- 美术构图备注：待填写

### 技术暂停（TACTICAL_TIMEOUT）

- 类名：`TacticalTimeout`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：获得 {Block:diff()} 点 [gold]格挡[/gold]。<br>生命比例最低的机器人回复 {Heal:diff()} 点生命并获得 {Plating:diff()} 层 [gold]覆甲[/gold]。
- 数值定义：`CanonicalVars => new DynamicVar[] { new BlockVar(12m, ValueProp.Move), new DynamicVar("Heal", 8m), new DynamicVar("Plating", 4m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); DynamicVars["Heal"].UpgradeValueBy(4m); DynamicVars["Plating"].UpgradeValueBy(2m); }`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:394`
- 普通卡图：`Laughman/images/card_portraits/tactical_timeout.png`
- 大图：`Laughman/images/card_portraits/big/tactical_timeout.png`
- 美术构图备注：待填写

### 世界技能大赛（WORLD_SKILLS）

- 类名：`WorldSkills`
- 类型：技能 / `Skill`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：从 3 张随机{IfUpgraded:show:升级后的}罕见或稀有牌中选择 1 张，本回合免费打出。<br>[gold]借用[/gold] 1：额外选择 1 张。
- 数值定义：`-`
- 关键词：`-`
- 升级实现：`-`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:296`
- 普通卡图：`Laughman/images/card_portraits/world_skills.png`
- 大图：`Laughman/images/card_portraits/big/world_skills.png`
- 美术构图备注：待填写

## 先古 / Ancient（3 张）

### 冠军形态（CHAMPION_FORM）

- 类名：`ChampionForm`
- 类型：能力 / `Power`
- 费用：`2`
- 目标：自身 / `Self`
- 效果：一台机器人获得 12 点生命上限并回复 12 点生命，获得 5 点力量、5 层覆甲和 5 点敏捷。<br>它每次行动后随机触发一种队员效果。
- 数值定义：`-`
- 关键词：`-`
- 升级实现：`OnUpgrade() => EnergyCost.UpgradeBy(-1);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:447`
- 普通卡图：`Laughman/images/card_portraits/champion_form.png`
- 大图：`Laughman/images/card_portraits/big/champion_form.png`
- 美术构图备注：待填写

### 冲击UL（IMPACT_U_L）

- 类名：`ImpactUL`
- 类型：技能 / `Skill`
- 费用：`4`
- 目标：自身 / `Self`
- 效果：首次打出费用 -1。将本回合免费的 [gold]3号步兵部署{IfUpgraded:show:+}[/gold]、[gold]英雄部署{IfUpgraded:show:+}[/gold]和[gold]趴窝哨兵{IfUpgraded:show:+}[/gold]加入手牌。
- 数值定义：`-`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Innate, CardKeyword.Exhaust };`
- 升级实现：`-`
- 代码：`LaughmanCode/Cards/Batch3Cards.cs:703`
- 普通卡图：`Laughman/images/card_portraits/impact_u_l.png`
- 大图：`Laughman/images/card_portraits/big/impact_u_l.png`
- 美术构图备注：待填写

### 剑指春茧（ROAD_TO_SPRING_COCOON）

- 类名：`RoadToSpringCocoon`
- 类型：技能 / `Skill`
- 费用：`6`
- 目标：自身 / `Self`
- 效果：首次打出费用 -3。<br>牌组集齐 7 类单体召唤牌时才能打出。随机升级并打出其中 {Count:diff()} 张。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Count", 3m) };`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Innate, CardKeyword.Exhaust };`
- 升级实现：`OnUpgrade() => DynamicVars["Count"].UpgradeValueBy(2m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:385`
- 普通卡图：`Laughman/images/card_portraits/road_to_spring_cocoon.png`
- 大图：`Laughman/images/card_portraits/big/road_to_spring_cocoon.png`
- 美术构图备注：待填写

## 衍生 / Token（5 张）

### 电控（ELECTRICAL_TRAINING）

- 类名：`ElectricalTraining`
- 类型：能力 / `Power`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：获得 {Amount:diff()} 层 [gold]电控[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Amount", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Amount"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:221`
- 普通卡图：`Laughman/images/card_portraits/electrical_training.png`
- 大图：`Laughman/images/card_portraits/big/electrical_training.png`
- 美术构图备注：待填写

### 硬件（HARDWARE_TRAINING）

- 类名：`HardwareTraining`
- 类型：能力 / `Power`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：获得 {Amount:diff()} 层 [gold]硬件[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Amount", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Amount"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:242`
- 普通卡图：`Laughman/images/card_portraits/hardware_training.png`
- 大图：`Laughman/images/card_portraits/big/hardware_training.png`
- 美术构图备注：待填写

### 机械（MECHANICAL_TRAINING）

- 类名：`MechanicalTraining`
- 类型：能力 / `Power`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：获得 {Amount:diff()} 层 [gold]机械[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Amount", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Amount"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:214`
- 普通卡图：`Laughman/images/card_portraits/mechanical_training.png`
- 大图：`Laughman/images/card_portraits/big/mechanical_training.png`
- 美术构图备注：待填写

### 视觉or硬件（SOFTWARE_TRAINING_CHOICE）

- 类名：`SoftwareTrainingChoice`
- 类型：技能 / `Skill`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：从视觉与硬件训练中选择一张。
- 数值定义：`-`
- 关键词：`-`
- 升级实现：`-`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:228`
- 普通卡图：`Laughman/images/card_portraits/software_training_choice.png`
- 大图：`Laughman/images/card_portraits/big/software_training_choice.png`
- 美术构图备注：待填写

### 视觉（VISION_TRAINING）

- 类名：`VisionTraining`
- 类型：能力 / `Power`
- 费用：`0`
- 目标：自身 / `Self`
- 效果：获得 {Amount:diff()} 层 [gold]视觉[/gold]。
- 数值定义：`CanonicalVars => new[] { new DynamicVar("Amount", 2m) };`
- 关键词：`-`
- 升级实现：`OnUpgrade() => DynamicVars["Amount"].UpgradeValueBy(1m);`
- 代码：`LaughmanCode/Cards/Batch2Cards.cs:235`
- 普通卡图：`Laughman/images/card_portraits/vision_training.png`
- 大图：`Laughman/images/card_portraits/big/vision_training.png`
- 美术构图备注：待填写

## 任务 / Quest（1 张）

### 冲刺UC（SPRINT_U_C）

- 类名：`SprintUC`
- 类型：Quest / `Quest`
- 费用：`-1`
- 目标：None / `None`
- 效果：无法打出。持有它进入第三层时，你的下一个事件将变为决赛准备。
- 数值定义：`-`
- 关键词：`CanonicalKeywords => new[] { CardKeyword.Unplayable };`
- 升级实现：`-`
- 代码：`LaughmanCode/Cards/SprintUC.cs:16`
- 普通卡图：`Laughman/images/card_portraits/sprint_u_c.png`
- 大图：`Laughman/images/card_portraits/big/sprint_u_c.png`
- 美术构图备注：待填写
