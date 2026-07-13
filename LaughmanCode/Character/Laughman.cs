using BaseLib.Abstracts;
using BaseLib.Extensions;
using Laughman.LaughmanCode.Cards;
using Laughman.LaughmanCode.Extensions;
using Laughman.LaughmanCode.Relics;
using Laughman.LaughmanCode.Timeline;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Laughman.LaughmanCode.Character;

// 时间线解锁触发（RitsuLib 扫描角色类上的特性注册）：渐进式，打得越多解锁越多。
//  - 完成任意一局 → 宣讲会（根，解锁角色）
//  - 胜利一次     → 报名·联调
//  - 累计击杀 3 精英 → 没那么美好
//  - 累计击杀 5 精英 → 战队管理层
//  - 累计击杀 3 Boss → 曼巴出去·冲击UL
//  - A1 胜利      → 熬夜的滋味（药水）
[UnlockEpochAfterRunAs(typeof(LaughmanCharacterEpoch))]
[UnlockEpochAfterWinAs(typeof(LaughmanCardEpoch))]
[UnlockEpochAfterEliteVictories(typeof(LaughmanHardshipEpoch), 3)]
[UnlockEpochAfterEliteVictories(typeof(LaughmanManagementEpoch), 5)]
[UnlockEpochAfterBossVictories(typeof(LaughmanLeagueEpoch), 3)]
[UnlockEpochAfterAscensionOneWin(typeof(LaughmanPotionEpoch))]
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
