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
        if (existing != null && !existing.IsDead)
        {
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
            if (pet.Monster is MechModel mech && !pet.IsDead)
            {
                mech.RefreshIntent(owner, combatState);
                var node = NCombatRoom.Instance?.GetCreatureNode(pet);
                if (node == null)
                {
                    continue;
                }
                if (playerNode != null)
                {
                    node.Position = playerNode.Position + GetMechOffset(mech);
                }
                node.ToggleIsInteractable(true);
            }
        }
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
