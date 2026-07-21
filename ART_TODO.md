# RM工程师美工待办

> 当前状态基于 `F:\st2\Laughman` 工作树。游戏资源源文件均使用 PNG；Godot 构建时会自动导入到 PCK。

## 正式版收口清单

当前玩法和内容已基本完整。`v1.0` 的美术目标不是补齐所有可选资源，而是保证正常游玩路径不再出现明显占位图，角色与机器人具备基础动作反馈。

### P0：正式版必须完成

| 项目 | 新增源素材 | 复用方案 | 接入难度 |
|---|---:|---|---:|
| 角色战斗形象（暂缓） | 0 | 正式版前暂用 `PlaceholderCharacterModel` 提供的战士资源 | 暂不处理 |
| 玩家机甲战斗图（已完成） | 10 张透明立绘 | 10 类机甲均已按模型 ID 独立接入 | 已完成 |
| 三冠王 7 个随从（已完成） | 7 张独立原画 | 已完成透明化并按随从槽位接入 | 已完成 |
| Power 图标（已完成） | 24 张本地图标 | 15 张专门绘制、8 张卡图中心裁切、联防由机甲协同翻转；其余复用原版/已有图标 | 已完成 |
| 10 件遗物图标（已完成） | 2 张独立图 + 2 张四宫格 | 已切分并导出普通图、outline 和 big | 已完成 |
| Mod 图标 | 1 张主视觉，或由角色主视觉派生 | 与 Workshop 封面、角色 Logo 共用设计 | 低 |
| 通用战斗动作反馈 | 1 套程序动画 | 覆盖全部玩家机甲、三冠王随从和静态 Boss | 中 |

Power 图标已全部接入；夹击使用原版图标，敌方小陀螺/飞坡复用玩家版图标。

通用程序动画至少包括：待机浮动、攻击前冲/后坐、受击震动闪白、防御反馈、晕眩倾斜、借用淡出、归队淡入、死亡下沉和复活显现。它不依赖逐帧素材，是动画部分性价比最高的工作。

### P1：推荐随正式版完成

| 项目 | 数量 | 复用方案 |
|---|---:|---|
| 全部自定义 Power 图标（已完成） | 27 个具体 Power | 24 张本地文件 + 原版夹击 + 2 个敌方状态复用玩家图标 |
| 药水普通图（已完成） | 5 张 | 5/5 已按药水 ID 接入 |
| 药水 outline（可选） | 5 张 | 当前普通图均为完整方形画面，不能直接从 Alpha 自动描边 |
| 三冠王战斗记录/顶栏图标（已完成） | 1 张 Logo | 普通层与 outline 层统一使用 `triple_crown_logo.png` |
| 召唤、借用、归队特效 | 1 套模板 | 所有机甲共享，按机型调整颜色和位移 |
| 关键攻击 VFX | 4 套模板 | 常规弹道、英雄炮弹、工程支援、飞镖高速弹道 |

### P2：不阻塞正式版

- 遗物 big 和 outline 已从源图自动派生完成。
- 自定义能量图标：`big_energy.png` 与 `text_energy.png`。
- 2 个事件背景：可复用对应主题卡图后重新构图。
- 角色自定义形象和完整 Spine/逐帧动画：当前暂用战士资源，后续再替换。
- 三冠王 Boss 完整动画：正式版至少保证待机、受击、死亡反馈即可。

### P3：长期可选

- 6 个 Era 图标和 7 个 Epoch 插画：可复用卡图主体。
- 92 张 Beta 卡图：正常游戏会回退普通卡图，不建议投入正式版工期。
- 事件场景动画和卡牌动态插画。

### 复用边界

可以直接复用或派生：玩家机甲到三冠王同型随从、Boss 图到战斗记录图标、卡图主体到事件/时间线、普通图到 outline/big、同一 Power 基础符号的颜色和角标变体、同一套程序动画参数。

不应继续复用：所有机甲共用一个轮廓、所有 Power 共用遗物图、10 件遗物共用同一图标、角色战斗形象使用低分辨率选择图、Mod 图标直接缩小复杂卡图。

机甲、三冠王随从、Power、遗物和药水普通图均已完成，角色形象和动画暂用战士资源。当前唯一仍缺的核心源素材是品牌主视觉/Mod 图标 1 张；其余缺项均为可选派生资源或程序动画。

## 总览

| 优先级 | 类别 | 当前状态 | 是否只补文件即可 |
|---|---|---|---|
| 1 | 卡牌插画 | 92/92，普通图与大图均已齐全 | 已完成 |
| 2 | 10 类机甲战斗图 | 10/10 透明立绘已完成并按机甲 ID 接入 | 已完成 |
| 3 | 三冠王 7 个随从 | 7/7 独立立绘已透明化并按槽位接入 | 已完成 |
| 4 | Power 图标 | 27/27 已接入；24 张本地图标，3 个复用 | 已完成 |
| 暂缓 | 角色战斗形象 | 使用 `PlaceholderCharacterModel` 的战士资源 | 暂不处理 |
| 6 | 遗物普通图 | 10/10 已切分并按 ID 接入 | 已完成 |
| 7 | 药水普通图 | 5/5 已按 ID 接入 | 已完成 |
| 8 | Mod 图标 | `mod_image.png` 仍是占位图 | 是 |
| 9 | 遗物 outline / big | 10/10 outline，10/10 big | 已完成 |
| 10 | 能量图标 | 使用 Ironclad 能量图标 | 否，需要代码接入 |
| 11 | 事件/时间线 | 使用默认视觉 | 否，需要代码接入 |
| 最后 | Beta 卡图 | 0/92，但回退普通卡图 | 是，优先级低 |

## 第一批：只补文件即可

### 卡牌插画

规格：

- 普通图：`250x190 PNG`
- 大图：`1000x760 PNG`
- 背景：可不透明
- 不要画卡框、费用、卡名、描述文字

当前 92 张卡牌的普通图和大图均已齐全。

工作流：把中文同名图片放入 `F:\st2\美工`，运行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\import-art.ps1
```

### 药水图标

规格：`64x64 PNG`，透明背景。

| 药水 | ID | 文件 |
|---|---|---|
| 应急焊料 | `EMERGENCY_SOLDER` | `Laughman/images/potions/emergency_solder.png` |
| 过载注入剂 | `OVERCLOCK_INJECTOR` | `Laughman/images/potions/overclock_injector.png` |

普通图已全部完成。以下 outline 仍为可选项：

- `spare_battery_outline.png`
- `coolant_flask_outline.png`
- `calibration_fluid_outline.png`
- `emergency_solder_outline.png`
- `overclock_injector_outline.png`

outline 规格同普通图：`64x64 PNG`。当前五张普通图均为完整方形画面，没有透明主体边缘，不能可靠自动生成轮廓；运行时会回退普通图，不影响显示。

### Mod 图标

当前 `Laughman/mod_image.png` 是占位图。

建议规格：

- `420x420 PNG`
- 可透明或方形背景
- 主体放中央安全区

目标文件：

```text
Laughman/mod_image.png
```

### 遗物普通图

10 件遗物的普通图、outline 和 big 均已完成并按 ID 自动加载。

普通图规格：`94x94 PNG`，透明背景。

| 优先级 | 遗物 | ID | 文件 |
|---:|---|---|---|
| 1 | 招笑避雷 | `LAUGHING_THUNDER` | `Laughman/images/relics/laughing_thunder.png` |
| 2 | 工程师之愿 | `ENGINEER_WISH` | `Laughman/images/relics/engineer_wish.png` |
| 3 | 租借合同 | `LEASE_CONTRACT` | `Laughman/images/relics/lease_contract.png` |
| 4 | 借调防线 | `BORROWED_DEFENSE_LINE` | `Laughman/images/relics/borrowed_defense_line.png` |
| 5 | 超级电容 | `SUPER_CAPACITOR` | `Laughman/images/relics/super_capacitor.png` |
| 6 | 量产流水线 | `MASS_PRODUCTION_LINE` | `Laughman/images/relics/mass_production_line.png` |
| 7 | 失控底盘 | `RUNAWAY_CHASSIS` | `Laughman/images/relics/runaway_chassis.png` |
| 8 | 涂满的笔记本 | `SCRIBBLED_NOTEBOOK` | `Laughman/images/relics/scribbled_notebook.png` |
| 9 | 最后检查 | `FINAL_CHECK` | `Laughman/images/relics/final_check.png` |
| 10 | 美美撤离 | `GRACEFUL_RETREAT` | `Laughman/images/relics/graceful_retreat.png` |

## 第二批：需要代码接入，但收益最高

### 机甲与三冠王随从战斗静态图

玩家侧 10 类机甲均已完成独立透明战斗立绘。`MechModel` 按机甲 ID 自动读取
`images/monsters/<id>.png`；资源缺失时仍回退 `card_portraits/placeholder.png`。

三冠王 7 个随从也已完成独立透明立绘。`TripleCrownMinion` 按 `SlotName` 自动读取
`images/monsters/triple_crown_<slot>.png`：

- `triple_crown_infantry3.png`
- `triple_crown_infantry4.png`
- `triple_crown_sentinel.png`
- `triple_crown_hero.png`
- `triple_crown_engineer.png`
- `triple_crown_drone.png`
- `triple_crown_dart.png`

完成规格：玩家机甲 10/10、三冠王随从 7/7，均统一为 `512x512 RGBA PNG`；
地面机甲按脚底基线排布，无人机和飞镖按悬停构图居中。

### Power 图标

规格：`64x64 PNG`，透明背景。大图 `256x256 PNG` 可后补。

27 个 Power 图标均已接入。15 张使用 `F:\st2\美工\power` 的专门素材，8 张从对应卡图中心裁最大正方形后缩为 `64x64`，联防由机甲协同水平翻转生成。

专门绘制并接入：

| 优先级 | Power | 建议符号 |
|---:|---|---|
| 1 | 机械 | 齿轮/扳手 |
| 2 | 电控 | 电路/闪电 |
| 3 | 视觉 | 镜头/准星 |
| 4 | 硬件 | 芯片/焊点 |
| 5 | 借用 | 借条/离场箭头 |
| 6 | 小陀螺 | 旋转箭头 |
| 7 | 飞坡 | 斜坡起跳 |
| 8 | 集火标记 | 多箭头准星 |
| 9 | 机甲协同 | 多机器人连线 |
| 10 | 雷达部署 | 雷达波 |

其余专门素材：视觉开源、招新预告、精确制导、兑矿、天眼雷达。

卡图裁切：AI哨兵、熬夜形态、冠军形态、熬夜调车、时间管理大师、精密工程、最是人间留不住、打符。

复用：夹击使用原版 `FlankingPower` 图标；敌方小陀螺/飞坡复用玩家版；联防使用水平翻转后的机甲协同。

代码已通过统一 `LaughmanPower` 基类读取 `Laughman/images/powers/<id>.png`。

### 角色战斗形象

当前角色战斗视觉和动画暂时继续由 `PlaceholderCharacterModel` 提供战士资源，不列入近期缺项。

建议先做静态立绘：

- `1024x1536 PNG`
- 透明背景
- 全身或大半身
- 脚底基线清晰

需要代码/场景：接入自定义 Creature Visual 或 Godot `.tscn`。

## 第三批：界面完整度

### 遗物 outline 和大图

每件遗物三件套已完成：

- 普通：`94x94 PNG`
- outline：`94x94 PNG`
- big：`256x256 PNG`

outline 文件名：`<id>_outline.png`。

big 文件名：`Laughman/images/relics/big/<id>.png`。

### 自定义能量图标

当前使用 Ironclad 能量图标。

建议素材：

- `big_energy.png`：`74x74 PNG`
- `text_energy.png`：`24x24 PNG`

需要代码：在 CardPool/RelicPool/PotionPool 接入自定义路径或自定义 `EnergyColorName`。

## 第四批：低频视觉

### 事件图

自定义事件：

- 抉择……抉择……：已有源图 `F:\st2\美工\事件\抉择，抉择.png`。
- 决赛准备：已有源图 `F:\st2\美工\事件\美美得吃.png`。

两张事件图已处理为 `1000x760 PNG` 并接入各自的 `CustomInitialPortraitPath`。

### 时间线图标

已有源图：`F:\st2\美工\时间线\宣讲会.png`。剩余 Epoch 均已从现有卡图派生：

| Epoch | 主选卡图 | 备选 | 对应程度 |
|---|---|---|---|
| 宣讲会 | 已有独立源图 | 招新展示 | 已完成源图 |
| 报名·联调 | 赛前联调 | 招新展示 | 明显对应 |
| 没那么美好 | 清退通知 | 熬夜形态 / 散伙饭 | 明显对应 |
| 战队管理层 | 时间管理大师 | 外援教练 | 明显对应 |
| 车都被占用 | 嵌入式比赛 | 精准兑换 | 明显对应 |
| 曼巴出去·冲击UL | 冲击UL | 曼巴出去！ / 冠军巡游 | 明显对应 |
| 剑指春茧 | 剑指春茧 | 冲刺UC | 标题直接对应 |

7 个 Epoch 已通过 RitsuLib `0.109.0` 的 `CustomPackedPortraitPath` / `CustomBigPortraitPath` 接入；6 个 Era 顶部图标已从主线 Epoch 中心裁切并通过仅作用于 2727..2732 的 Harmony 补丁接入。era=2730 的列同时包含“战队管理层”和“车都被占用”，顶部图标使用“时间管理大师”。

### Beta 卡图

当前 `beta` 目录不存在，缺 92 张。正常游戏会回退到普通卡图，不影响视觉完整度。

建议最后再做，或者长期不做。
