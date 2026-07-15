using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Laughman.LaughmanCode.Cards;

public interface IOwnedMechTargetingCard
{
    OwnedMechTargetMode MechTargetMode => OwnedMechTargetMode.Alive;
    bool IsAllowedMechTarget(Creature target) => true;
}

public enum OwnedMechTargetMode
{
    Alive,
    DeadRevivable,
    DeadElseAlive
}

public static class OwnedMechTargeting
{
    public static List<Creature> GetCandidates(CardModel card)
    {
        if (card is not IOwnedMechTargetingCard targeting)
        {
            return new List<Creature>();
        }
        var mechs = card.Owner.Creature.Pets
            .Where(target => target.Monster is MechModel && targeting.IsAllowedMechTarget(target))
            .ToList();
        return targeting.MechTargetMode switch
        {
            OwnedMechTargetMode.DeadRevivable => mechs
                .Where(target => target.IsDead && target.Monster is IRevivableMech)
                .ToList(),
            OwnedMechTargetMode.DeadElseAlive => mechs.Any(target => target.IsDead && target.Monster is IRevivableMech)
                ? mechs.Where(target => target.IsDead && target.Monster is IRevivableMech).ToList()
                : mechs.Where(target => !target.IsDead).ToList(),
            _ => mechs.Where(target => !target.IsDead).ToList()
        };
    }

    public static bool IsValidTarget(CardModel card, Creature? target) =>
        target != null
        && card is IOwnedMechTargetingCard targeting
        && card.Owner.Creature.Pets.Contains(target)
        && target.PetOwner == card.Owner
        && target.Monster is MechModel
        && targeting.IsAllowedMechTarget(target)
        && GetCandidates(card).Contains(target);
}
