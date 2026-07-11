using BaseLib.Abstracts;
using BaseLib.Extensions;
using Laughman.LaughmanCode.Cards;
using Laughman.LaughmanCode.Extensions;
using Laughman.LaughmanCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Laughman.LaughmanCode.Character;

public class Laughman : PlaceholderCharacterModel
{
    public const string CharacterId = "Laughman";
    
    public static readonly Color Color = new("ffcc00");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;
    
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<EngineerStrike>(),
        ModelDb.Card<EngineerStrike>(),
        ModelDb.Card<EngineerStrike>(),
        ModelDb.Card<EngineerStrike>(),
        ModelDb.Card<EngineerDefend>(),
        ModelDb.Card<EngineerDefend>(),
        ModelDb.Card<EngineerDefend>(),
        ModelDb.Card<EngineerDefend>(),
        ModelDb.Card<InfantrySummon>(),
        ModelDb.Card<BaseSupply>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<LaughingThunder>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<LaughmanCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<LaughmanRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<LaughmanPotionPool>();
    
    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override string CustomIconTexturePath => "character_icon_laughman.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_laughman.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_laughman_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_laughman.png".CharacterUiPath();
}
