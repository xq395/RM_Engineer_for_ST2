using BaseLib.Utils;
using Laughman.LaughmanCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Laughman.LaughmanCode.Relics;

// 涂满的笔记本（事件遗物，事件1「抉择……抉择……」选项二奖励）：
// 获得时，将一张 [神化]（Apotheosis，原版先古通用卡）和一张 [悔恨]（Regret，原版诅咒）加入牌组。
[Pool(typeof(LaughmanRelicPool))]
public class ScribbledNotebook : LaughmanRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task AfterObtained()
    {
        var apotheosis = Owner.RunState.CreateCard<Apotheosis>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(apotheosis, PileType.Deck), 2f);

        var regret = Owner.RunState.CreateCard<Regret>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(regret, PileType.Deck), 2f);
    }
}
