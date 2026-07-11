using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.HoverTips;

namespace Laughman.LaughmanCode.Mechs;

public static class MechHoverTipProvider
{
    public static IHoverTip GetActionTip(MechModel mech)
    {
        string entry = mech switch
        {
            InfantryMech => "LAUGHMAN-INFANTRY_MECH.action",
            SentinelMech => "LAUGHMAN-SENTINEL_MECH.action",
            HeroMech hero when hero.LobFireMode => "LAUGHMAN-HERO_MECH.lobAction",
            HeroMech => "LAUGHMAN-HERO_MECH.action",
            EngineerMech engineer when engineer.MaxActions != int.MaxValue => "LAUGHMAN-ENGINEER_MECH.upgradedAction",
            EngineerMech => "LAUGHMAN-ENGINEER_MECH.action",
            CovenantDroneMech => "LAUGHMAN-COVENANT_DRONE_MECH.action",
            DroneMech => "LAUGHMAN-DRONE_MECH.action",
            DartBotMech => "LAUGHMAN-DART_BOT_MECH.action",
            _ => "LAUGHMAN-MECH.action"
        };
        return new HoverTip(new LocString("monsters", entry + ".title"), new LocString("monsters", entry + ".description"));
    }
}
