using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Laughman.LaughmanCode.Mechs;

// 机甲召唤工具：由召唤卡直接调用。
// - 首次召唤某类型：加宠物、设血量、显示血条，并确保玩家身上挂有 MechCoordinatorPower。
// - 同名叠加：用 GainMaxHp 提升上限并只补对应血量（保留已损失的血量缺口，参考原版召唤）。
public static class MechManager
{
    public static async Task<Creature?> SummonMech<T>(Player owner, int summonValue)
        where T : MechModel, new()
    {
        if (owner.Creature.CombatState == null)
        {
            return null;
        }

        var existing = FindMech<T>(owner);
        if (existing != null)
        {
            if (existing.IsDead)
            {
                // 可复活机甲死亡后仍保留在 Pets 和场景树中。普通召唤不能绕过买活
                // 规则复活它，也不能再创建同型宠物，否则两个节点会在固定 offset 上重叠。
                await EnsureCoordinator(owner);
                RefreshMechUi(owner);
                return null;
            }

            // 同名叠加：最大生命 +summonValue，仅回复 summonValue 的血（保留血量缺口）。
            await CreatureCmd.GainMaxHp(existing, summonValue);
            await EnsureCoordinator(owner);
            RefreshMechUi(owner);
            await NotifySummonRelics(owner);
            return existing;
        }

        var mech = await PlayerCmd.AddPet<T>(owner);
        await CreatureCmd.SetMaxAndCurrentHp(mech, summonValue);
        await EnsureCoordinator(owner);

        // 原生宠物定位逻辑会把同一玩家的所有宠物 ToggleIsInteractable(false)。
        // 每次召唤后必须把所有机器人重新打开，否则只会显示最新召唤那台的血条/状态栏。
        RefreshMechUi(owner);
        await NotifySummonRelics(owner);

        return mech;
    }

    // 召唤后通知相关遗物（如「量产流水线」第二次召唤抽牌）。
    private static async Task NotifySummonRelics(Player owner)
    {
        var line = owner.GetRelic<Laughman.LaughmanCode.Relics.MassProductionLine>();
        if (line != null)
        {
            await line.OnSummon(owner);
        }
    }

    // 确保玩家身上有且只有一个协调器 Power。
    public static async Task EnsureCoordinator(Player owner)
    {
        if (!owner.Creature.HasPower<MechCoordinatorPower>())
        {
            await PowerCmd.Apply<MechCoordinatorPower>(
                new BlockingPlayerChoiceContext(), owner.Creature, 1m, null, null);
        }
    }

    public static void RefreshMechUi(Player owner)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }
        var playerNode = NCombatRoom.Instance?.GetCreatureNode(owner.Creature);
        foreach (var pet in owner.Creature.Pets)
        {
            if (pet.Monster is not MechModel mech)
            {
                continue;
            }

            var node = NCombatRoom.Instance?.GetCreatureNode(pet);
            if (node == null)
            {
                continue;
            }

            // 原版每次添加宠物都会重排所有 Pets，包括死亡后保留的可复活机甲。
            // 无论死活都恢复固定布局，避免机甲在死亡期间被移走，复活时突然跳位。
            if (playerNode != null)
            {
                node.Position = playerNode.Position + GetMechOffset(mech);
            }

            if (pet.IsDead)
            {
                node.ToggleIsInteractable(false);
            }
            else
            {
                mech.RefreshIntent(owner, combatState);
                node.ToggleIsInteractable(true);
            }
        }
    }

    public static void RestoreRevivedMechUi(Player owner, Creature creature)
    {
        RefreshMechUi(owner);

        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node == null || creature.IsDead)
        {
            return;
        }

        // StartDeathAnim 会关闭控制器焦点，StartReviveAnim 只恢复鼠标交互。
        // 死亡/复活状态栏 tween 也可能短暂竞争，因此显式恢复最终可见状态。
        node.Visible = true;
        node.Modulate = Colors.White;
        node.Visuals.Visible = true;
        node.Visuals.Modulate = Colors.White;
        node.Body.Visible = true;
        node.Body.Modulate = Colors.White;
        var healthBar = node.GetNodeOrNull<Control>("%HealthBar");
        if (healthBar != null)
        {
            healthBar.Visible = true;
            healthBar.Modulate = Colors.White;
        }
        node.Hitbox.FocusMode = Control.FocusModeEnum.All;
        node.ToggleIsInteractable(true);
    }

    private static Vector2 GetMechOffset(MechModel mech)
    {
        // 屏卫在右侧；英雄、工程、飞镖沿飞镖原横轴排成竖列；无人机置于高空。
        return mech switch
        {
            // 右侧上排（4号步兵，需放在 InfantryMech 之前，否则被基类匹配吞掉）
            InfantryNo4Mech => new Vector2(225f, -150f),
            // 右侧下排
            SentinelMech => new Vector2(230f, 30f),
            InfantryMech => new Vector2(380f, 30f),
            // 头顶高空
            DroneMech => new Vector2(30f, -360f),
            // 左侧竖列
            HeroMech => new Vector2(-235f, 60f),
            EngineerMech => new Vector2(-235f, -70f),
            DartBotMech => new Vector2(-235f, -200f),
            _ => new Vector2(150f, 30f)
        };
    }

    // 查找某类型的现有机器人（存活或阵亡都返回，供买活复用）。
    public static Creature? FindMech<T>(Player owner) where T : MechModel
    {
        foreach (var pet in owner.Creature.Pets)
        {
            if (pet.Monster?.GetType() == typeof(T))
            {
                return pet;
            }
        }
        return null;
    }
}
