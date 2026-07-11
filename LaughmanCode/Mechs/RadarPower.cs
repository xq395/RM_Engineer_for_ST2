using BaseLib.Abstracts;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Laughman.LaughmanCode.Extensions;

namespace Laughman.LaughmanCode.Mechs;

// 雷达（能力）：每回合开始给随机一名存活敌人施加易伤（升级后附带虚弱）。无实体、不受伤害。
public class RadarPower : CustomPowerModel
{
    private const int VulnerableStacks = 1;
    private const int WeakStacks = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://" + "laughing_thunder.png".RelicImagePath();

    // 升级版雷达附带虚弱。由 RadarLock 卡在施放后设置。
    public bool AppliesWeak { get; set; }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.IsDead)
        {
            return;
        }
        if (CombatManager.Instance == null || CombatManager.Instance.IsEnding)
        {
            return;
        }

        var enemies = combatState.HittableEnemies.Where(e => !e.IsDead).ToList();
        if (enemies.Count == 0)
        {
            return;
        }

        var owner = Owner.PetOwner ?? (Owner.IsPlayer ? Owner.Player : null);
        int idx = owner != null ? owner.RunState.Rng.MonsterAi.NextInt(enemies.Count) : 0;
        var target = enemies[idx];

        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, VulnerableStacks, Owner, null);
        if (AppliesWeak)
        {
            await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, WeakStacks, Owner, null);
        }
    }
}
