using Godot;

public static partial class Effects
{
    public record MoveContext(Pawn Pawn, Vector2I Target);
    public static readonly EffectRule<MoveContext> Move = new AnimationEffectRule<MoveContext>(
        (MoveContext context) => new AnimationContext
        {
            Component = context.Pawn.Get<AnimationComponent>(),
            Name = "move",
            Side = context.Pawn.Coords.SideTowards(context.Target),
        }
    ).Then((MoveContext context) =>
        {
            context.Pawn.Get<LocomotionComponent>()?.StartMovement(context.Target);
        }
    ).Then((MoveContext context) =>
        {
            context.Pawn.Get<LocomotionComponent>()?.EndMovement(context.Target);
        }
    );

    public record TakeDamageContext(Pawn Pawn, int Damage);
    public static readonly EffectRule<TakeDamageContext> TakeDamage = new MultiEffectRule<TakeDamageContext>(
    ).Then(
        (TakeDamageContext context) => GD.Print(context.Pawn)
    ).Then(
        (TakeDamageContext context) => context.Pawn.Get<AnimationComponent>()?.PlayText($"-{context.Damage}", Colors.Red)
    ).Then(
        (TakeDamageContext context) => context.Pawn.Get<HealthComponent>()?.ChangeHealth(-context.Damage)
    ).ThenIf(
        (TakeDamageContext context) => context.Pawn.Get<HealthComponent>()?.CurrentHealth <= 0,
        (TakeDamageContext context) => context.Pawn.Get<HealthComponent>().Die()
    );

    public record AttackContext(Pawn Attacker, Pawn Receiver, int Damage);
    public static readonly MultiEffectRule<AttackContext> Attack = new AnimationEffectRule<AttackContext>(
        (AttackContext context) => new AnimationContext
        {
            Component = context.Attacker.Get<AnimationComponent>(),
            Name = "attack",
            Side = context.Attacker.Coords.SideTowards(context.Receiver.Coords),
        }
    ).Skip(
    ).Then(
        TakeDamage.WithSelect((AttackContext context) => new TakeDamageContext(context.Receiver, context.Damage))
    );

    public record PushContext(Pawn Pawn, VectorUtils.Direction Direction, int Strength);
    public record PushAugmentedContext(Pawn Pawn, VectorUtils.Direction Direction, Vector2I Destination, int Crush);
    public static readonly EffectRule<PushContext> Push = new AnimationEffectRule<PushAugmentedContext>(
        (PushAugmentedContext context) =>
        {
            return new AnimationContext
            {
                Component = context.Pawn.Get<AnimationComponent>(),
                Name = "pushed",
                Side = context.Direction.ToSide().Opposite(),
            };
        }
    ).Then((PushAugmentedContext context) =>
        {
            context.Pawn.Get<LocomotionComponent>()?.StartMovement(context.Destination);
        }
    ).Then((PushAugmentedContext context) =>
        {
            context.Pawn.Get<LocomotionComponent>()?.EndMovement(context.Destination);
            if (context.Crush > 0)
            {
                GD.Print($"TODO: monster 'crushed' by {context.Crush} tiles");
            }
        }
    ).WithSelect((PushContext context) =>
        {
            int pushed = 0;
            int crushed = 0;
            int strength = context.Strength;
            while (strength > 0)
            {
                if (!context.Pawn.Board.Walkable(context.Pawn.Coords + context.Direction.ToVector2I() * (pushed + 1)))
                {
                    crushed = strength;
                    break;
                }
                pushed += 1;
                strength -= 1;
            }
            return new PushAugmentedContext(
                Pawn: context.Pawn,
                Direction: context.Direction,
                Destination: context.Pawn.Coords + context.Direction.ToVector2I() * pushed,
                Crush: crushed
            );
        }
    );

    public record PushAttackContext(Pawn Attacker, Pawn Receiver, VectorUtils.Direction Direction, int Strength);
    public static readonly EffectRule<PushAttackContext> PushAttack = new AnimationEffectRule<PushAttackContext>(
        (PushAttackContext context) => new AnimationContext
        {
            Component = context.Attacker.Get<AnimationComponent>(),
            Name = "attack",
            Side = context.Direction.ToVector2I().ToSide(),
        }
    ).Skip(
    ).ThenIf(
        (PushAttackContext context) => (context.Receiver is not null),
        Push.WithSelect((PushAttackContext context) => new PushContext(context.Receiver, context.Direction, context.Strength))
    );
}