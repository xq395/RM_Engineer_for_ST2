using BaseLib.Abstracts;
using BaseLib.Extensions;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;

namespace Laughman.LaughmanCode.Cards;

public abstract class LaughmanCard(int cost, CardType type, CardRarity rarity, TargetType target) : 
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
            return ResourceLoader.Exists(path) ? path : PortraitPath;
        }
    }
    
    public override string PortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "placeholder.png".CardImagePath();
        }
    }
    public override string BetaPortraitPath
    {
        get
        {
            var path = $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : PortraitPath;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => Id.Entry.RemovePrefix() switch
    {
        "INFANTRY_SUMMON" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GUARD") },
        "INFANTRY_NO4" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GUARD") },
        "SENTINEL_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GUARD") },
        "HERO_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("STRAIGHT_FIRE") },
        "ENGINEER_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("ACTION_LIMIT") },
        "DRONE_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("FLYING"), LaughmanHoverTips.Term("UPKEEP") },
        "SWARM_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("SLEEP") },
        "ARMOR_DEPLOY" => new[] { LaughmanHoverTips.Term("GUARD"), LaughmanHoverTips.Term("PLATED_ARMOR") },
        "RAMP_JUMP" => new[] { LaughmanHoverTips.Term("RAMP_JUMP") },
        "LOB_FIRE_COMMAND" => new[] { LaughmanHoverTips.Term("LOB_FIRE") },
        "BASE_SUPPLY" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("STUN") },
        "PAY_TO_RESPAWN" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GYRO_SPIN") },
        "GYRO_SPIN_COMMAND" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GYRO_SPIN") },
        "SCHOOL_EXHIBITION" => new[] { LaughmanHoverTips.Term("BORROW") },
        "TRAINING_CAMP" => new[] { LaughmanHoverTips.Term("BORROW") },
        "GUEST_COACH" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("BORROW") },
        "BROADCAST_CUT" => new[] { LaughmanHoverTips.Term("BORROW") },
        "MAMBA_OUT" => new[] { LaughmanHoverTips.Term("ROBOT") },
        "EMBEDDED_CONTEST" => new[] { LaughmanHoverTips.Term("BORROW") },
        "MATH_MODELING" => new[] { LaughmanHoverTips.Term("BORROW") },
        "ACM_REGIONAL" => new[] { LaughmanHoverTips.Term("BORROW") },
        "ROBOTICS_FINALS" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("BORROW") },
        "WORLD_SKILLS" => new[] { LaughmanHoverTips.Term("BORROW") },
        "CHAMPION_PARADE" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("BORROW") },
        "ALL_NIGHTER_TUNING" => new[] { LaughmanHoverTips.Term("BORROW") },
        "TACTICAL_TIMEOUT" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("PLATED_ARMOR") },
        "RULE_OVERHAUL" => new[] { LaughmanHoverTips.Term("BORROW") },
        "MVP_OF_THE_MATCH" => new[] { LaughmanHoverTips.Term("BORROW") },
        "HIGH_SPEED_FLANK" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GYRO_SPIN") },
        "ON_SITE_DEFENSE" => new[] { LaughmanHoverTips.Term("BORROW") },
        "RECALL_NOTICE" => new[] { LaughmanHoverTips.Term("RETURN") },
        "EMERGENCY_RECALL" => new[] { LaughmanHoverTips.Term("RETURN"), LaughmanHoverTips.Term("GYRO_SPIN") },
        "PRE_MATCH_DISPATCH_MEETING" => new[] { LaughmanHoverTips.Term("RETURN") },
        "RECRUITMENT_SHOWCASE" => new[] { LaughmanHoverTips.Term("BORROW") },
        "MECH_COOLDOWN" => new[] { LaughmanHoverTips.Term("BORROW") },
        "COLLECTIVE_FIRE" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("FOCUS_FIRE") },
        "STALLED_SENTINEL" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GUARD") },
        "I_AM_THE_WAVE" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("BORROW") },
        "NEW_COVENANT_DRONE" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("FLYING") },
        "BEACON_DART" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("FOCUS_FIRE") },
        "PRECISION_ENGINEERING" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("ACTION_LIMIT") },
        "SKY_EYE_RADAR" => new[] { LaughmanHoverTips.Term("SHACKLES"), LaughmanHoverTips.Term("FLANKING") },
        "IMPACT_U_L" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GUARD") },
        _ => Array.Empty<IHoverTip>()
    };
}
