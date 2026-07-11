using System.Linq;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Laughman.LaughmanCode.Mechs;

// 原版 AttackIntent 会用“本地玩家”作为目标重新计算意图伤害。
// 玩家侧宠物的意图因此会错误吃玩家身上的易伤/减伤。这里固定基础数值，
// 但会按“机器人自身的力量 + 飞坡×3 倍率”动态修正显示，与实际结算一致。
public class FixedAttackIntent(int baseDamage, int repeat = 1) : AttackIntent
{
    protected override LocString IntentLabelFormat => new("intents", repeat > 1 ? "FORMAT_DAMAGE_MULTI" : "FORMAT_DAMAGE_SINGLE");

    public override int Repeats => repeat;

    // 单次攻击的显示伤害：基础 + 力量，再乘飞坡倍率。
    private int PerHitDamage(Creature owner)
    {
        decimal damage = baseDamage + owner.GetPowerAmount<StrengthPower>();
        if (owner.HasPower<RampJumpPower>())
        {
            damage *= 3m;
        }
        if (damage < 0m)
        {
            damage = 0m;
        }
        return (int)damage;
    }

    public override int GetTotalDamage(IEnumerable<Creature> targets, Creature owner)
    {
        return PerHitDamage(owner) * repeat;
    }

    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        var label = IntentLabelFormat;
        label.Add("Damage", PerHitDamage(owner));
        label.Add("Repeat", repeat);
        return label;
    }

    protected override LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)
    {
        var description = new LocString("intents", "ATTACK.description");
        description.Add("Damage", PerHitDamage(owner));
        description.Add("Repeat", repeat);
        description.Add("IsMultiplayer", false);
        return description;
    }
}
