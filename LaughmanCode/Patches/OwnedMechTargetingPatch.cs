using System.Reflection;
using Godot;
using HarmonyLib;
using Laughman.LaughmanCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Laughman.LaughmanCode.Patches;

internal static class OwnedMechTargetingSession
{
    public static CardModel? Card { get; set; }
    private static readonly List<(Control Hitbox, Control.MouseFilterEnum Mouse, Control.FocusModeEnum Focus)> SavedHitboxes = new();

    public static void Begin(CardModel? card)
    {
        Restore();
        Card = card;
        if (card == null || NCombatRoom.Instance == null) return;
        foreach (var candidate in OwnedMechTargeting.GetCandidates(card).Where(candidate => candidate.IsDead))
        {
            var node = NCombatRoom.Instance.GetCreatureNode(candidate);
            if (node == null) continue;
            SavedHitboxes.Add((node.Hitbox, node.Hitbox.MouseFilter, node.Hitbox.FocusMode));
            node.Hitbox.MouseFilter = Control.MouseFilterEnum.Stop;
            node.Hitbox.FocusMode = Control.FocusModeEnum.All;
        }
    }

    public static void Restore()
    {
        foreach (var (hitbox, mouse, focus) in SavedHitboxes)
        {
            if (!GodotObject.IsInstanceValid(hitbox)) continue;
            hitbox.MouseFilter = mouse;
            hitbox.FocusMode = focus;
        }
        SavedHitboxes.Clear();
        Card = null;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.IsValidTarget))]
internal static class OwnedMechIsValidTargetPatch
{
    private static bool Prefix(CardModel __instance, Creature? target, ref bool __result)
    {
        if (__instance is not IOwnedMechTargetingCard) return true;
        __result = OwnedMechTargeting.IsValidTarget(__instance, target);
        return false;
    }
}

[HarmonyPatch]
internal static class OwnedMechCanPlayPatch
{
    private static MethodBase TargetMethod() => AccessTools.Method(
        typeof(CardModel), nameof(CardModel.CanPlay),
        new[] { typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType() });

    private static void Postfix(CardModel __instance, ref bool __result, ref UnplayableReason reason)
    {
        if (__instance is not IOwnedMechTargetingCard) return;
        if (OwnedMechTargeting.GetCandidates(__instance).Count > 0)
            reason &= ~UnplayableReason.NoLivingAllies;
        else
            reason |= UnplayableReason.NoLivingAllies;
        __result = reason == UnplayableReason.None;
    }
}

[HarmonyPatch]
internal static class OwnedMechStartTargetingPatch
{
    private static MethodBase TargetMethod() => AccessTools.Method(
        typeof(NTargetManager), nameof(NTargetManager.StartTargeting),
        new[] { typeof(TargetType), typeof(Control), typeof(TargetMode), typeof(Func<bool>), typeof(Func<Node, bool>) });

    private static void Prefix(Control control)
    {
        var card = control is NCard { Model: IOwnedMechTargetingCard } cardNode
            ? cardNode.Model
            : null;
        OwnedMechTargetingSession.Begin(card);
    }
}

[HarmonyPatch(typeof(NTargetManager), nameof(NTargetManager.AllowedToTargetNode))]
internal static class OwnedMechAllowedTargetPatch
{
    private static bool Prefix(Node node, ref bool __result)
    {
        var card = OwnedMechTargetingSession.Card;
        if (card == null) return true;
        __result = node is NCreature creatureNode
            && OwnedMechTargeting.IsValidTarget(card, creatureNode.Entity);
        return false;
    }
}

[HarmonyPatch]
internal static class OwnedMechFinishTargetingPatch
{
    private static MethodBase TargetMethod() => AccessTools.Method(
        typeof(NTargetManager), "FinishTargeting", new[] { typeof(bool) });

    private static void Postfix() => OwnedMechTargetingSession.Restore();
}

[HarmonyPatch(typeof(Creature), "get_IsHittable")]
internal static class OwnedMechDeadHittablePatch
{
    private static void Postfix(Creature __instance, ref bool __result)
    {
        var card = OwnedMechTargetingSession.Card;
        if (card != null && OwnedMechTargeting.IsValidTarget(card, __instance))
            __result = true;
    }
}

[HarmonyPatch]
internal static class OwnedMechCardPlayCleanupPatch
{
    private static MethodBase TargetMethod() => AccessTools.Method(typeof(NCardPlay), "Cleanup");

    private static void Postfix() => OwnedMechTargetingSession.Restore();
}

[HarmonyPatch]
internal static class OwnedMechMouseTargetingPatch
{
    private static readonly FieldInfo TargetField = AccessTools.Field(typeof(NMouseCardPlay), "_target");

    private static MethodBase TargetMethod() => AccessTools.Method(
        typeof(NMouseCardPlay), "TargetSelection", new[] { typeof(TargetMode) });

    private static bool Prefix(NMouseCardPlay __instance, ref Task __result)
    {
        var card = __instance.Holder.CardModel;
        if (card is not IOwnedMechTargetingCard) return true;
        var candidates = OwnedMechTargeting.GetCandidates(card);
        if (candidates.Count != 1) return true;
        TargetField.SetValue(__instance, candidates[0]);
        __result = Task.CompletedTask;
        return false;
    }
}

[HarmonyPatch]
internal static class OwnedMechControllerTargetingPatch
{
    private static readonly MethodInfo TryPlayCardMethod = AccessTools.Method(typeof(NCardPlay), "TryPlayCard");

    private static MethodBase TargetMethod() => AccessTools.Method(
        typeof(NControllerCardPlay), "SingleCreatureTargeting", new[] { typeof(TargetType) });

    private static bool Prefix(NControllerCardPlay __instance, ref Task __result)
    {
        var card = __instance.Holder.CardModel;
        if (card is not IOwnedMechTargetingCard) return true;
        __result = Select(__instance, card);
        return false;
    }

    private static async Task Select(NControllerCardPlay cardPlay, CardModel card)
    {
        var candidates = OwnedMechTargeting.GetCandidates(card);
        if (candidates.Count == 0)
        {
            cardPlay.CancelPlayCard();
            return;
        }
        if (candidates.Count == 1)
        {
            TryPlayCardMethod.Invoke(cardPlay, new object?[] { candidates[0] });
            return;
        }

        var room = NCombatRoom.Instance;
        if (room == null)
        {
            cardPlay.CancelPlayCard();
            return;
        }
        OwnedMechTargetingSession.Begin(card);
        var nodes = candidates
            .Select(room.GetCreatureNode)
            .OfType<NCreature>()
            .ToList();
        if (nodes.Count == 0)
        {
            OwnedMechTargetingSession.Restore();
            cardPlay.CancelPlayCard();
            return;
        }
        room.RestrictControllerNavigation(nodes.Select(node => node.Hitbox));
        nodes[0].Hitbox.GrabFocus();

        var manager = NTargetManager.Instance;
        if (cardPlay.Holder.CardNode == null)
        {
            OwnedMechTargetingSession.Restore();
            cardPlay.CancelPlayCard();
            return;
        }
        manager.StartTargeting(TargetType.AnyAlly, cardPlay.Holder.CardNode, TargetMode.Controller,
            () => !GodotObject.IsInstanceValid(cardPlay), null);
        var selected = await manager.SelectionFinished();
        if (GodotObject.IsInstanceValid(cardPlay) && selected is NCreature creatureNode)
            TryPlayCardMethod.Invoke(cardPlay, new object?[] { creatureNode.Entity });
        else if (GodotObject.IsInstanceValid(cardPlay))
            cardPlay.CancelPlayCard();
        OwnedMechTargetingSession.Restore();
    }
}
