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
        "INFANTRY_SUMMON" => new[] { LaughmanHoverTips.Term("INFANTRY_MECH"), LaughmanHoverTips.Term("GUARD") },
        "INFANTRY_NO4" => new[] { LaughmanHoverTips.Term("INFANTRY_MECH"), LaughmanHoverTips.Term("GUARD") },
        "SENTINEL_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("SENTINEL_MECH"), LaughmanHoverTips.Term("GUARD") },
        "HERO_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("HERO_MECH"), LaughmanHoverTips.Term("STRAIGHT_FIRE") },
        "ENGINEER_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("ENGINEER_MECH"), LaughmanHoverTips.Term("ACTION_LIMIT") },
        "DRONE_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("DRONE_MECH"), LaughmanHoverTips.Term("FLYING"), LaughmanHoverTips.Term("UPKEEP") },
        "SWARM_DEPLOYMENT" => new[] { LaughmanHoverTips.Term("DART_MECH"), LaughmanHoverTips.Term("SLEEP") },
        "ARMOR_DEPLOY" => new[] { LaughmanHoverTips.Term("GUARD"), LaughmanHoverTips.Term("PLATED_ARMOR") },
        "MECHANICAL_TRYOUT" => new[] { LaughmanHoverTips.Term("MECHANICAL_MEMBER") },
        "ELECTRONICS_TRYOUT" => new[] { LaughmanHoverTips.Term("ELECTRICAL_MEMBER"), LaughmanHoverTips.Term("PLATED_ARMOR") },
        "VISION_TRYOUT" => new[] { LaughmanHoverTips.Term("VISION_MEMBER") },
        "HARDWARE_TRYOUT" => new[] { LaughmanHoverTips.Term("HARDWARE_MEMBER") },
        "MECHANICAL_TRAINING" => new[] { LaughmanHoverTips.Term("MECHANICAL_MEMBER") },
        "ELECTRICAL_TRAINING" => new[] { LaughmanHoverTips.Term("ELECTRICAL_MEMBER") },
        "VISION_TRAINING" => new[] { LaughmanHoverTips.Term("VISION_MEMBER") },
        "HARDWARE_TRAINING" => new[] { LaughmanHoverTips.Term("HARDWARE_MEMBER") },
        "CRASH_COURSE" => TeamMemberHoverTips,
        "SOFTWARE_TRAINING_CHOICE" => new[] { LaughmanHoverTips.Term("VISION_MEMBER"), LaughmanHoverTips.Term("HARDWARE_MEMBER") },
        "CLEAR_ROLES" => TeamMemberHoverTips,
        "PRE_MATCH_CALIBRATION" => TeamMemberHoverTips,
        "SYNC_CALIBRATION" => TeamMemberHoverTips,
        "WEAKPOINT_MARKING" => new[] { LaughmanHoverTips.Term("VISION_MEMBER") },
        "PREP_REVIEW" => TeamMemberHoverTips,
        "ANNUAL_RECRUITMENT" => TeamMemberHoverTips,
        "CHAMPION_FORM" => new[]
        {
            LaughmanHoverTips.Term("ROBOT"),
            LaughmanHoverTips.Term("MECHANICAL_MEMBER"),
            LaughmanHoverTips.Term("ELECTRICAL_MEMBER"),
            LaughmanHoverTips.Term("VISION_MEMBER"),
            LaughmanHoverTips.Term("HARDWARE_MEMBER")
        },
        "DISMISSAL_NOTICE" => TeamMemberHoverTips,
        "RAMP_JUMP" => new[] { LaughmanHoverTips.Term("RAMP_JUMP") },
        "LOB_FIRE_COMMAND" => new[] { LaughmanHoverTips.Term("LOB_FIRE") },
        "RADAR_LOCK" => new[] { LaughmanHoverTips.Term("RADAR") },
        "MECHA_PROTOCOL" => new[] { LaughmanHoverTips.Term("MECHA_PROTOCOL") },
        "AI_SENTINEL" => new[] { LaughmanHoverTips.Term("AI_SENTINEL"), LaughmanHoverTips.Term("GYRO_SPIN") },
        "PRECISION_GUIDANCE" => new[] { LaughmanHoverTips.Term("PRECISION_GUIDANCE") },
        "INSOMNIA_FORM" => new[] { LaughmanHoverTips.Term("INSOMNIA_MODE") },
        "PRECISION_EXCHANGE" => new[] { LaughmanHoverTips.Term("EXCHANGE_MINING") },
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
        "ALL_NIGHTER_TUNING" => new[] { LaughmanHoverTips.Term("ALL_NIGHTER_TUNING"), LaughmanHoverTips.Term("BORROW") },
        "TACTICAL_TIMEOUT" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("PLATED_ARMOR") },
        "RULE_OVERHAUL" => new[] { LaughmanHoverTips.Term("RULE_OVERHAUL"), LaughmanHoverTips.Term("BORROW"), LaughmanHoverTips.Term("RETURN") },
        "MVP_OF_THE_MATCH" => new[] { LaughmanHoverTips.Term("BORROW") },
        "HIGH_SPEED_FLANK" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GYRO_SPIN") },
        "ON_SITE_DEFENSE" => new[] { LaughmanHoverTips.Term("BORROW") },
        "RECALL_NOTICE" => new[] { LaughmanHoverTips.Term("RETURN") },
        "EMERGENCY_RECALL" => new[] { LaughmanHoverTips.Term("RETURN"), LaughmanHoverTips.Term("GYRO_SPIN") },
        "PRE_MATCH_DISPATCH_MEETING" => new[] { LaughmanHoverTips.Term("RETURN") },
        "RECRUITMENT_SHOWCASE" => new[]
        {
            LaughmanHoverTips.Term("BORROW"),
            LaughmanHoverTips.Term("MECHANICAL_MEMBER"),
            LaughmanHoverTips.Term("ELECTRICAL_MEMBER"),
            LaughmanHoverTips.Term("VISION_MEMBER"),
            LaughmanHoverTips.Term("HARDWARE_MEMBER")
        },
        "MECH_COOLDOWN" => new[] { LaughmanHoverTips.Term("BORROW") },
        "COLLECTIVE_FIRE" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("FOCUS_FIRE") },
        "STALLED_SENTINEL" => new[] { LaughmanHoverTips.Term("SENTINEL_MECH"), LaughmanHoverTips.Term("GUARD") },
        "I_AM_THE_WAVE" => new[] { LaughmanHoverTips.Term("HERO_MECH"), LaughmanHoverTips.Term("TIDAL_HERO"), LaughmanHoverTips.Term("BORROW") },
        "NEW_COVENANT_DRONE" => new[] { LaughmanHoverTips.Term("COVENANT_DRONE"), LaughmanHoverTips.Term("FLYING") },
        "BEACON_DART" => new[] { LaughmanHoverTips.Term("DART_MECH"), LaughmanHoverTips.Term("BEACON_DART"), LaughmanHoverTips.Term("FOCUS_FIRE") },
        "PRECISION_ENGINEERING" => new[] { LaughmanHoverTips.Term("PRECISION_ENGINEER"), LaughmanHoverTips.Term("ACTION_LIMIT") },
        "SKY_EYE_RADAR" => new[] { LaughmanHoverTips.Term("SHACKLES"), LaughmanHoverTips.Term("FLANKING") },
        "IMPACT_U_L" => new[] { LaughmanHoverTips.Term("ROBOT"), LaughmanHoverTips.Term("GUARD") },
        _ => Array.Empty<IHoverTip>()
    };

    private static IHoverTip[] TeamMemberHoverTips =>
    [
        LaughmanHoverTips.Term("MECHANICAL_MEMBER"),
        LaughmanHoverTips.Term("ELECTRICAL_MEMBER"),
        LaughmanHoverTips.Term("VISION_MEMBER"),
        LaughmanHoverTips.Term("HARDWARE_MEMBER")
    ];
}
