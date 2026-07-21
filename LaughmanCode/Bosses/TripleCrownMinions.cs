using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using Laughman.LaughmanCode.Extensions;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laughman.LaughmanCode.Bosses;

public interface ITripleCrownMinion
{
    bool IsGuard { get; }
    bool IsBackline { get; }
    bool CanBuyback { get; }
    string SlotName { get; }
}

public abstract class TripleCrownMinion : CustomMonsterModel, ITripleCrownMinion
{
    protected const float VisualScale = 0.28f;

    public abstract bool IsGuard { get; }
    public bool IsBackline => !IsGuard;
    public virtual bool CanBuyback => false;
    public abstract string SlotName { get; }

    public override float HpBarSizeReduction => 45f;
    public override bool ShouldFadeAfterDeath => false;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Creature, 1m, Creature, null);
        await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), Creature, 2m, Creature, null);
    }

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        var image = $"triple_crown_{SlotName}.png";
        var texture = PreloadManager.Cache.GetTexture2D("res://" + image.MonsterImagePath());
        if (texture == null)
        {
            MainFile.Logger.Info($"[{GetType().Name}] battle sprite not found, falling back to placeholder");
            texture = PreloadManager.Cache.GetTexture2D("res://" + "placeholder.png".CardImagePath());
            if (texture == null)
            {
                return null;
            }
        }

        var visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(texture);
        visuals.DefaultScale = VisualScale;
        visuals.Scale = Vector2.One * VisualScale;
        return visuals;
    }

    public override CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller) => null;

    protected async Task AttackPlayers(int damage, int hits = 1)
    {
        var command = DamageCmd.Attack(damage).FromMonster(this).WithHitFx("vfx/vfx_attack_slash");
        if (hits > 1)
        {
            command.WithHitCount(hits).OnlyPlayAnimOnce();
        }
        await command.Execute(null);
    }

    protected static MonsterMoveStateMachine Loop(MoveState move)
    {
        move.FollowUpState = move;
        return new MonsterMoveStateMachine(new[] { (MonsterState)move }, move);
    }

    public virtual void ResetAfterRevive()
    {
    }
}

public sealed class TripleCrownInfantry3 : TripleCrownMinion
{
    private const int Damage = 8;
    private const int Block = 5;
    public override int MinInitialHp => 60;
    public override int MaxInitialHp => 60;
    public override bool IsGuard => true;
    public override bool CanBuyback => true;
    public override string SlotName => "infantry3";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var move = new MoveState("ATTACK_DEFEND", Perform, new SingleAttackIntent(Damage), new DefendIntent());
        return Loop(move);
    }

    private async Task Perform(IReadOnlyList<Creature> targets)
    {
        await AttackPlayers(Damage);
        await CreatureCmd.GainBlock(Creature, Block, ValueProp.Move, null);
    }
}

public sealed class TripleCrownSentinel : TripleCrownMinion
{
    private const int Damage = 5;
    private const int BossBlock = 8;
    public override int MinInitialHp => 55;
    public override int MaxInitialHp => 55;
    public override bool IsGuard => true;
    public override bool CanBuyback => true;
    public override string SlotName => "sentinel";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var move = new MoveState("ATTACK_DEFEND", Perform, new SingleAttackIntent(Damage), new DefendIntent());
        return Loop(move);
    }

    private async Task Perform(IReadOnlyList<Creature> targets)
    {
        await AttackPlayers(Damage);
        var boss = CombatState.Enemies.FirstOrDefault(enemy => enemy.IsAlive && enemy.Monster is TripleCrownChampion);
        if (boss != null)
        {
            decimal dexterity = Creature.GetPower<DexterityPower>()?.Amount ?? 0m;
            await CreatureCmd.GainBlock(boss, BossBlock + dexterity, ValueProp.Move, null);
        }
    }
}

public sealed class TripleCrownHero : TripleCrownMinion
{
    private const int Damage = 15;
    public override int MinInitialHp => 65;
    public override int MaxInitialHp => 65;
    public override bool IsGuard => false;
    public override bool CanBuyback => true;
    public override string SlotName => "hero";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        return Loop(new MoveState("ATTACK", targets => AttackPlayers(Damage), new SingleAttackIntent(Damage)));
    }
}

public sealed class TripleCrownInfantry4 : TripleCrownMinion
{
    private const int Damage = 8;
    private const int Block = 5;
    public override int MinInitialHp => 60;
    public override int MaxInitialHp => 60;
    public override bool IsGuard => true;
    public override bool CanBuyback => true;
    public override string SlotName => "infantry4";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var move = new MoveState("ATTACK_DEFEND", Perform, new SingleAttackIntent(Damage), new DefendIntent());
        return Loop(move);
    }

    private async Task Perform(IReadOnlyList<Creature> targets)
    {
        await AttackPlayers(Damage);
        await CreatureCmd.GainBlock(Creature, Block, ValueProp.Move, null);
    }
}

public sealed class TripleCrownDrone : TripleCrownMinion
{
    private const int Damage = 3;
    private const int Hits = 3;
    public override int MinInitialHp => 18;
    public override int MaxInitialHp => 18;
    public override bool IsGuard => false;
    public override string SlotName => "drone";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        return Loop(new MoveState("MULTI_ATTACK", targets => AttackPlayers(Damage, Hits), new MultiAttackIntent(Damage, Hits)));
    }
}

public sealed class TripleCrownEngineer : TripleCrownMinion
{
    private const int BossBlock = 10;
    public override int MinInitialHp => 45;
    public override int MaxInitialHp => 45;
    public override bool IsGuard => false;
    public override string SlotName => "engineer";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        return Loop(new MoveState("SUPPORT", Perform, new DefendIntent(), new BuffIntent()));
    }

    private async Task Perform(IReadOnlyList<Creature> targets)
    {
        var boss = CombatState.Enemies.FirstOrDefault(enemy => enemy.IsAlive && enemy.Monster is TripleCrownChampion);
        if (boss != null)
        {
            decimal dexterity = Creature.GetPower<DexterityPower>()?.Amount ?? 0m;
            await CreatureCmd.GainBlock(boss, BossBlock + dexterity, ValueProp.Move, null);
        }

        var allies = CombatState.Enemies
            .Where(enemy => enemy.IsAlive && enemy != Creature && enemy.Monster is ITripleCrownMinion)
            .ToList();
        if (allies.Count > 0)
        {
            var target = allies[RunRng.MonsterAi.NextInt(allies.Count)];
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), target, 1m, Creature, null);
        }
    }
}

public sealed class TripleCrownDart : TripleCrownMinion
{
    private const int Damage = 30;
    private int _charge;
    private MoveState? _firstCharge;
    public override int MinInitialHp => 18;
    public override int MaxInitialHp => 18;
    public override bool IsGuard => false;
    public override string SlotName => "dart";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        _firstCharge = new MoveState("CHARGE_1", _ => Charge(), new BuffIntent());
        var secondCharge = new MoveState("CHARGE_2", _ => Charge(), new BuffIntent());
        var fire = new MoveState("FIRE", _ => Fire(), new SingleAttackIntent(Damage));
        _firstCharge.FollowUpState = secondCharge;
        secondCharge.FollowUpState = fire;
        fire.FollowUpState = _firstCharge;
        return new MonsterMoveStateMachine(new MonsterState[] { _firstCharge, secondCharge, fire }, _firstCharge);
    }

    private Task Charge()
    {
        _charge++;
        return Task.CompletedTask;
    }

    private async Task Fire()
    {
        await AttackPlayers(Damage);
        _charge = 0;
    }

    public override void ResetAfterRevive()
    {
        _charge = 0;
        if (_firstCharge != null)
        {
            SetMoveImmediate(_firstCharge, forceTransition: true);
        }
    }
}
