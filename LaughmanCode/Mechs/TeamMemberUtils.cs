using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Laughman.LaughmanCode.Mechs;

public enum TeamMemberType
{
    Mechanical,
    Electrical,
    Vision,
    Hardware
}

public static class TeamMemberUtils
{
    public static List<Creature> AliveMechs(Player owner) =>
        owner.Creature.Pets.Where(p => p.Monster is MechModel && !p.IsDead).ToList();

    public static Creature? RandomMech(Player owner)
    {
        var mechs = AliveMechs(owner);
        return mechs.Count == 0 ? null : mechs[owner.RunState.Rng.MonsterAi.NextInt(mechs.Count)];
    }

    public static async Task Trigger(Player owner, TeamMemberType type, int times, PlayerChoiceContext? context = null)
    {
        context ??= new ThrowingPlayerChoiceContext();
        for (int i = 0; i < times; i++)
        {
            var target = RandomMech(owner);
            if (target == null)
            {
                return;
            }

            switch (type)
            {
                case TeamMemberType.Mechanical:
                    await CreatureCmd.GainMaxHp(target, 3m);
                    await CreatureCmd.Heal(target, 3m);
                    break;
                case TeamMemberType.Electrical:
                    await PowerCmd.Apply<PlatingPower>(context, target, 5m, owner.Creature, null);
                    break;
                case TeamMemberType.Vision:
                    await PowerCmd.Apply<StrengthPower>(context, target, 2m, owner.Creature, null);
                    break;
                case TeamMemberType.Hardware:
                    await PowerCmd.Apply<DexterityPower>(context, target, 2m, owner.Creature, null);
                    break;
            }
        }
    }

    public static async Task TriggerAll(Player owner, PlayerChoiceContext? context = null)
    {
        await Trigger(owner, TeamMemberType.Mechanical, Amount<MechanicalMemberPower>(owner), context);
        await Trigger(owner, TeamMemberType.Electrical, Amount<ElectricalMemberPower>(owner), context);
        await Trigger(owner, TeamMemberType.Vision, Amount<VisionMemberPower>(owner), context);
        await Trigger(owner, TeamMemberType.Hardware, Amount<HardwareMemberPower>(owner), context);
    }

    public static Task TriggerRandom(Player owner, PlayerChoiceContext? context = null) =>
        Trigger(owner, (TeamMemberType)owner.RunState.Rng.MonsterAi.NextInt(4), 1, context);

    public static async Task AddRandom(PlayerChoiceContext context, Player owner, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            switch ((TeamMemberType)owner.RunState.Rng.MonsterAi.NextInt(4))
            {
                case TeamMemberType.Mechanical:
                    await PowerCmd.Apply<MechanicalMemberPower>(context, owner.Creature, 1m, owner.Creature, null);
                    break;
                case TeamMemberType.Electrical:
                    await PowerCmd.Apply<ElectricalMemberPower>(context, owner.Creature, 1m, owner.Creature, null);
                    break;
                case TeamMemberType.Vision:
                    await PowerCmd.Apply<VisionMemberPower>(context, owner.Creature, 1m, owner.Creature, null);
                    break;
                case TeamMemberType.Hardware:
                    await PowerCmd.Apply<HardwareMemberPower>(context, owner.Creature, 1m, owner.Creature, null);
                    break;
            }
        }
    }

    public static async Task ReduceHighest(PlayerChoiceContext context, Player owner)
    {
        var powers = new MegaCrit.Sts2.Core.Models.PowerModel?[]
        {
            owner.Creature.GetPower<MechanicalMemberPower>(),
            owner.Creature.GetPower<ElectricalMemberPower>(),
            owner.Creature.GetPower<VisionMemberPower>(),
            owner.Creature.GetPower<HardwareMemberPower>()
        }.Where(p => p != null && p.Amount > 0).ToList();
        if (powers.Count == 0) return;

        int highest = powers.Max(p => p!.Amount);
        var tied = powers.Where(p => p!.Amount == highest).ToList();
        var chosen = tied[owner.RunState.Rng.MonsterAi.NextInt(tied.Count)]!;
        if (chosen.Amount <= 1) await PowerCmd.Remove(chosen);
        else await PowerCmd.ModifyAmount(context, chosen, -1m, null, null);
    }

    public static int Amount<T>(Player owner) where T : MegaCrit.Sts2.Core.Models.PowerModel
    {
        var power = owner.Creature.GetPower<T>();
        return power?.Amount ?? 0;
    }
}
