using HarmonyLib;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Patches;

// Flutter's vanilla transition does not re-check death after the hit. A mech can remove the last
// Flutter stack and kill the Hopper in the same segment, leaving the game waiting on its stun move.
[HarmonyPatch(typeof(FlutterPower), nameof(FlutterPower.AfterDamageReceived))]
internal static class ThievingHopperFlutterPatch
{
    private static bool Prefix(
        FlutterPower __instance,
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        ref Task __result)
    {
        if (dealer?.Monster is not MechModel)
        {
            return true;
        }
        __result = ResolveMechHit(__instance, choiceContext, target, result, props);
        return false;
    }

    private static async Task ResolveMechHit(
        FlutterPower power,
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props)
    {
        if (target != power.Owner || result.UnblockedDamage == 0 || !props.IsPoweredAttack())
        {
            return;
        }

        await PowerCmd.Decrement(power);
        if (power.Amount > 0 || power.Owner.IsDead)
        {
            return;
        }

        await CreatureCmd.TriggerAnim(power.Owner, ThievingHopper.stunTrigger, 0.6f);
        if (power.Owner.IsDead)
        {
            return;
        }

        MonsterModel monster = power.Owner.Monster!;
        string nextState = monster.MoveStateMachine!.StateLog.Last()
            .GetNextState(power.Owner, monster.RunRng.MonsterAi);
        await CreatureCmd.Stun(power.Owner, _ => Task.CompletedTask, nextState);
        ((ThievingHopper)monster).IsHovering = false;
        SfxCmd.StopLoop(ThievingHopper.hoverLoop);
        await Cmd.Wait(0.25f);
    }
}
