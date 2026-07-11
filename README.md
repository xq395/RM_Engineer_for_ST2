# RM工程师 / RM Engineer

中文昵称：**萝马人**

[English](#english) | [中文](#中文)

## 中文

### 关于 RoboMaster

RoboMaster 是一项以机器人对抗为核心的工程竞技赛事。参赛队伍需要让机械、电控、视觉、硬件等不同方向的成员协作，完成机器人的设计、制造、调试和临场决策，并操控步兵、英雄、哨兵、工程、无人机等不同定位的机器人共同作战。

本 Mod 以 RoboMaster 参赛队的工程师为原型，将备赛、招新、联调、赛场战术和机器人协同转化为《杀戮尖塔 2》的牌组构筑与战斗机制。这是一项由爱好者制作的非官方同人项目，与 RoboMaster 官方及《杀戮尖塔 2》开发团队无关。

### 角色机制

**机器人**是 RM工程师的核心。你可以召唤不同定位的机器人作为友方单位；它们拥有生命值、力量、敏捷、格挡和行动意图，并在每回合执行各自的行动。不同机型负责输出、防护、支援或资源生产，部分机型还具有屏卫、飞行、运维成本、休眠、复活等特性。

**编队与战术**要求玩家同时管理角色和机器人。机器人能够替角色承受攻击、获得强化、响应集火标记，或通过特定卡牌立即行动。部分战术会改变机器人的攻击模式和行动逻辑，因此构筑重点不仅是打牌，也包括部署顺序、机型搭配和行动时机。

**队员**代表机械、电控、视觉和硬件方向的成员，主要为已部署的机器人提供强化、维护和额外效果。积累不同方向的队员并让他们与合适的机型配合，是机器人持续成长的重要来源。

**借用**模拟 RM 队伍中的机器人被借去参加其他比赛，或因场地、设备和测试安排而需要排队协调的情况。部分卡牌会暂时借出队员或机器人，以换取更强的即时收益；被借用的机器人会暂时离开当前编队，不行动，也不参与承伤，之后再归队。玩家需要在短期收益和编队完整性之间取舍。

**先古成长**与初始遗物相连。造访先古之民会影响角色的长期成长，并允许基础牌逐步转化为角色卡；部分特殊牌、事件和遗物也围绕整局游戏中的队伍发展展开。

### 特殊术语

- **屏卫**：某些机甲特有的词条。带有屏卫的单位会替你承担未被你的格挡抵消的攻击伤害，并先消耗该单位自己的格挡，再损失生命值，类似原版的“为你而死”机制。同时存在多个屏卫单位时，最早部署的单位优先承担伤害；若其被击破，溢出的伤害会继续由下一个屏卫承担。
- **普通机甲承伤**：不具有屏卫的机甲不会替你挡下伤害。当屏卫未能抵消全部伤害，或场上没有可用的屏卫时，你与所有未被借用的普通机甲会同时受到剩余攻击伤害，采用原版多人游戏的共同受击逻辑。
- **飞行**：飞行机甲按照上述共同受击逻辑承受伤害时，受到的伤害减半。
- **运维成本**：机器人行动前需要支付指定数量的金币；金币不足时，该机器人本回合晕眩并跳过行动。
- **休眠**：机器人暂时不会执行正常行动，通常需要等待指定回合或满足对应条件。
- **借用 / 归队**：借用中的机器人暂时不行动、不触发屏卫，也不参与共同受击；借用结束或被提前归还后重新加入编队。

### Demo 内容

当前 Demo 已完成：

- 1 名可游玩的自定义角色，以及完整的初始牌组、卡池和升级逻辑
- 80+ 张已实现卡牌，包含常规卡牌、衍生牌、特殊牌和事件相关卡牌
- 8 类机器人单位及其行动、意图、受伤、强化和协同逻辑
- 10 件遗物，包含初始遗物、角色遗物及事件遗物
- 2 个自定义事件，以及与先古之民交互的特殊内容
- 角色专属能力、关键词提示和中文文本
- 一套围绕召唤、编队、借用、队员培养和机器人战术构筑的完整可玩框架

### Demo 局限

这是一个用于公开测试核心玩法的早期 Demo，并非完成版本：

- 暂无正式美术资源。角色、卡牌、机器人、遗物和事件目前大量使用占位图，也没有定制动画和完整视觉表现。
- 仍可能存在程序错误、显示问题、流程卡死、存档兼容问题，或与其他 Mod 冲突的情况。
- 数值平衡尚未经过足够规模的测试。部分构筑在成长后可能达到远高于原版角色的强度；建议追求挑战的玩家搭配提高游戏难度的 Mod，或主动采用更高难度规则。
- 卡牌、遗物、事件出现率及不同流派之间的强度差异仍可能大幅调整，后续版本不保证与旧存档兼容。
- 当前主要提供中文文本，其他语言支持尚未完成。
- 本 Demo 依赖 **BaseLib**，请确保安装并启用与当前游戏版本兼容的 BaseLib。

欢迎反馈可复现的 Bug、异常日志、数值体验和机制建议。反馈时请尽量附上游戏版本、BaseLib 版本、已启用 Mod 列表及复现步骤。

---

## English

### About RoboMaster

RoboMaster is an engineering competition centered on robot combat. Teams bring together members specializing in mechanics, control systems, computer vision, hardware, and other disciplines to design, build, tune, and operate a lineup of robots. Infantry, Hero, Sentry, Engineer, and Aerial units fill different roles and must work together on the field.

This mod is inspired by the experience of a RoboMaster team engineer. Preparation, recruitment, system integration, match tactics, and robot coordination are translated into deck-building and combat mechanics for *Slay the Spire 2*. This is an unofficial fan project and is not affiliated with RoboMaster or the developers of *Slay the Spire 2*.

### Character Mechanics

**Robots** are the core of the RM Engineer's kit. Different robot types can be summoned as allied units with their own HP, Strength, Dexterity, Block, and intents. They act each turn and specialize in damage, protection, support, or resource generation. Some also use mechanics such as Guard, Flying, upkeep costs, Sleep, and revival.

**Formation and tactics** require you to manage both the character and the deployed robots. Robots can intercept attacks, receive upgrades, follow focus-fire marks, or be ordered to act immediately. Certain cards alter their attack modes or behavior, making deployment order, lineup composition, and timing as important as the cards in your hand.

**Team members** represent the Mechanical, Control, Vision, and Hardware groups. Their main role is to upgrade, maintain, and provide additional effects to deployed robots. Building the right mix of disciplines is an important source of long-term robot scaling.

**Borrowing** represents a robot being loaned out for another competition, or waiting its turn because field space, equipment, and testing schedules must be coordinated across the team. Some cards temporarily Borrow a team member or robot for a stronger immediate payoff. A Borrowed robot leaves the active formation, does not act, and does not share incoming damage until it Returns, creating a tradeoff between short-term value and formation stability.

**Ancient progression** is tied to the starting relic. Visiting Ancients contributes to long-term character growth and allows basic cards to be transformed into character cards. Several special cards, events, and relics also reflect the team's development across an entire run.

### Special Terms

- **Guard**: A keyword exclusive to certain robots. After the player's Block is exhausted, Guard units intercept the remaining attack damage. Their own Block is consumed before their HP, similar to the base game's “Die for You” mechanic. If multiple Guard units are present, the earliest deployed unit takes damage first; overflow continues to the next Guard.
- **Damage to non-Guard robots**: A robot without Guard does not intercept damage for you. If no active Guard remains, the player and every non-Borrowed, non-Guard robot take the remaining attack damage simultaneously, following the base game's multiplayer shared-hit logic.
- **Flying**: When a Flying robot takes damage through the shared-hit rule above, that damage is halved.
- **Upkeep**: The listed amount of Gold must be paid before the robot acts. If you cannot pay, that robot is Stunned and skips its action for the turn.
- **Sleep**: The robot temporarily performs no normal action, usually until enough turns pass or a specified condition is met.
- **Borrow / Return**: A Borrowed robot does not act, use Guard, or participate in shared incoming damage. It rejoins the formation when the Borrow duration ends or an effect Returns it early.

### Demo Content

The current Demo includes:

- 1 playable custom character with a complete starting deck, card pool, and card upgrades
- 80+ implemented cards, including collectible, generated, special, and event-related cards
- 8 robot unit types with action, intent, damage, upgrade, and coordination logic
- 10 relics, including starting, character-specific, and event relics
- 2 custom events plus special interactions with Ancients
- Character-specific powers, keyword tooltips, and complete Chinese text
- A playable framework built around summoning, formation management, Borrowing, team development, and robot tactics

### Demo Limitations

This is an early public Demo intended to test the core gameplay, not a finished release:

- Final art is not available yet. The character, cards, robots, relics, and events currently rely heavily on placeholder images, with no custom animation or complete visual presentation.
- Bugs, display issues, progression blockers, save compatibility problems, and conflicts with other mods may still occur.
- Balance has not been tested at sufficient scale. Some builds can reach power levels far above the base-game characters; players looking for a challenge may want to use a difficulty-increasing mod or impose higher-difficulty rules.
- Card and relic balance, event frequency, and archetype strength may change substantially. Future versions are not guaranteed to remain compatible with old saves.
- The current release primarily supports Chinese. Additional localization is not yet complete.
- This Demo requires **BaseLib**. Install and enable a BaseLib version compatible with your current game version.

Reproducible bug reports, logs, balance feedback, and mechanic suggestions are welcome. When reporting an issue, please include the game version, BaseLib version, enabled mod list, and reproduction steps whenever possible.
