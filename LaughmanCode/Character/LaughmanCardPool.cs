using BaseLib.Abstracts;
using Godot;

namespace Laughman.LaughmanCode.Character;

public class LaughmanCardPool : CustomCardPoolModel
{
    public override string Title => Laughman.CharacterId;

    // Custom energy icons are not shipped yet; use a stock icon path to avoid repeated missing-resource loads.
    public override string EnergyColorName => "ironclad";

    public override float H => 0.12f;
    public override float S => 1f;
    public override float V => 1f;
    
    public override Color DeckEntryCardColor => new("ffcc00");
    
    public override bool IsColorless => false;
}
