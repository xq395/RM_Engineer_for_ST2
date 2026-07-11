using BaseLib.Abstracts;
using BaseLib.Utils;
using System.Collections.Generic;
using Laughman.LaughmanCode.Character;
using Laughman.LaughmanCode.Mechs;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Laughman.LaughmanCode.Relics;

// 租借合同（普通）：回合开始时，若场上存在借用中的机器人，则获得 3 金币。每场战斗至多 24。
[Pool(typeof(LaughmanRelicPool))]
public class LeaseContract : LaughmanRelic
{
    private const int GoldPerTurn = 3;
    private const int MaxPerCombat = 24;

    public override RelicRarity Rarity => RelicRarity.Common;

    // 本场战斗已发放的金币（每场重置）。
    private int _gainedThisCombat;

    public override Task BeforeCombatStart()
    {
        _gainedThisCombat = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || _gainedThisCombat >= MaxPerCombat)
        {
            return;
        }
        // 场上是否存在借用中的机器人。
        bool anyBorrowed = BorrowUtils.FindBorrowed(player) != null;
        if (!anyBorrowed)
        {
            return;
        }
        int amount = Math.Min(GoldPerTurn, MaxPerCombat - _gainedThisCombat);
        _gainedThisCombat += amount;
        Flash();
        await PlayerCmd.GainGold(amount, player);
    }
}
