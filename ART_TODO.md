# RM工程师美工待办

> 当前状态基于 `F:\st2\Laughman` 工作树。游戏资源源文件均使用 PNG；Godot 构建时会自动导入到 PCK。

## 总览

| 优先级 | 类别 | 当前状态 | 是否只补文件即可 |
|---|---|---|---|
| 1 | 卡牌插画 | 92/92，普通图与大图均已齐全 | 已完成 |
| 2 | 8 类机甲战斗图 | 全部共用占位图 | 否，需要统一代码接入 |
| 3 | 高频 Power 图标 | 22/22 共用占位图 | 否，需要统一代码接入 |
| 4 | 角色战斗形象 | 使用 `PlaceholderCharacterModel` | 否，需要角色视觉接入 |
| 5 | 遗物普通图 | 10/10 实质占位 | 是 |
| 6 | 剩余药水图 | 3/5，缺 2 张普通图；5/5 缺 outline | 是 |
| 7 | Mod 图标 | `mod_image.png` 仍是占位图 | 是 |
| 8 | 遗物 outline / big | 0/10 outline，0/10 big | 是 |
| 9 | 能量图标 | 使用 Ironclad 能量图标 | 否，需要代码接入 |
| 10 | 事件/时间线 | 使用默认视觉 | 否，需要代码接入 |
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

### 缺失药水普通图

规格：`64x64 PNG`，透明背景。

| 药水 | ID | 文件 |
|---|---|---|
| 应急焊料 | `EMERGENCY_SOLDER` | `Laughman/images/potions/emergency_solder.png` |
| 过载注入剂 | `OVERCLOCK_INJECTOR` | `Laughman/images/potions/overclock_injector.png` |

已有但缺 outline：

- `spare_battery_outline.png`
- `coolant_flask_outline.png`
- `calibration_fluid_outline.png`
- `emergency_solder_outline.png`
- `overclock_injector_outline.png`

outline 规格同普通图：`64x64 PNG`，透明背景，与普通图严格对齐。

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

第一轮建议只补普通图，outline 与 big 后补。

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

### 8 类机甲战斗静态图

当前所有机甲共用 `card_portraits/placeholder.png`，战斗中辨识度最低。

建议先做静态透明图：

- 标准：`512x512 PNG`，透明背景
- 偏高机体：`512x768 PNG`，透明背景
- 统一脚底基线和视觉高度

优先级：

| 优先级 | 机甲 |
|---:|---|
| 1 | 3号步兵 |
| 2 | 哨兵 |
| 3 | 英雄 |
| 4 | 工程 |
| 5 | 无人机 |
| 6 | 飞镖机器人 |
| 7 | 4号步兵 |
| 8 | 新约无人机 |

需要代码：给 `MechModel` 增加按机甲 ID 自动读取视觉文件，或每个机甲覆写文件名。

### 高频 Power 图标

规格：`64x64 PNG`，透明背景。大图 `256x256 PNG` 可后补。

当前 22 个 Power 全部共用遗物占位图，且 `images/powers` 尚未接入代码。

第一批先画这 10 个：

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

后续再补：视觉开源、招新预告、AI哨兵、精确制导、兑矿、熬夜形态、冠军形态、熬夜调车、时间管理大师、夹击、精密工程、天眼雷达。

需要代码：统一 `LaughmanPower` 或路径解析，让 Power 读取 `Laughman/images/powers/<id>.png`。

### 角色战斗形象

当前角色战斗视觉仍由 `PlaceholderCharacterModel` 提供。

建议先做静态立绘：

- `1024x1536 PNG`
- 透明背景
- 全身或大半身
- 脚底基线清晰

需要代码/场景：接入自定义 Creature Visual 或 Godot `.tscn`。

## 第三批：界面完整度

### 遗物 outline 和大图

每件遗物最终建议有三件套：

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

- 抉择……抉择……
- 决赛准备

当前无事件背景或场景。单放 PNG 不会生效，需要事件场景/路径接入。

### 时间线图标

当前时间线文本和解锁功能正常，但 6 个自定义 era 顶部图标缺失，7 个 epoch 没有专属插画。

需要研究 RitsuLib 的图标注册方式后接入。

### Beta 卡图

当前 `beta` 目录不存在，缺 92 张。正常游戏会回退到普通卡图，不影响视觉完整度。

建议最后再做，或者长期不做。
