using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class Effects
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

    public record DamageContext(Pawn Attacker, Pawn Receiver, int Damage);
    public static readonly MultiEffectRule<DamageContext> Damage = new AnimationEffectRule<DamageContext>(
        (DamageContext context) => new AnimationContext
        {
            Component = context.Attacker.Get<AnimationComponent>(),
            Name = "attack",
            Side = context.Attacker.Coords.SideTowards(context.Receiver.Coords),
        }
    ).Skip().Then((DamageContext context) =>
        {
            context.Receiver.Get<AnimationComponent>().PlayText($"-{context.Damage}", Colors.Red);
            context.Receiver.Get<HealthComponent>().ChangeHealth(-context.Damage);
        }
    );

    public record PushContext(Pawn Pawn, VectorUtils.Direction Direction, int Strength);
    public record PushAugmentedContext(Pawn Pawn, VectorUtils.Direction Direction, Vector2I Destination, int Crush);
    public static readonly EffectRule<PushContext> Push = new AnimationEffectRule<PushAugmentedContext>(
        (PushAugmentedContext context) =>
        {
            return new AnimationContext
            {
                Component = context.Pawn.Get<AnimationComponent>(),
                Name = "move",
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
            Side = context.Attacker.Coords.SideTowards(context.Receiver.Coords),
        }
    ).Skip().Then(
        Push.WithSelect((PushAttackContext context) => new PushContext(context.Receiver, context.Direction, context.Strength))
    );
}

public static class SkillList
{
    public static Skill Move = new Skill
    {
        Name = "Move",
        Target = Skill.TargetType.TILE,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.Move.WithSelect(
            (Context context) => new Effects.MoveContext(context.SourcePawn, context.CoordsTarget)
        ),
    };

    public static Skill Push = new Skill
    {
        Name = "Push",
        Target = Skill.TargetType.DIRECTION,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.PushAttack.WithSelect(
            (Context context) =>
            {
                Vector2I targetCoord = context.Origin + context.DirectionTarget.ToVector2I();
                Pawn targetPawn = context.SourcePawn.Board.GetFirstPawnAt(targetCoord);
                return new Effects.PushAttackContext(context.SourcePawn, targetPawn, context.DirectionTarget, 1);
            }
        ),
    };
    public static Skill SingleAttack = new Skill
    {
        Name = "SingleAttack",
        Target = Skill.TargetType.ENTITY,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.Damage.WithSelect(
            (Context context) =>
            {
                int damage = context.SourcePawn.Get<SkillComponent>()?.Damage ?? 10;
                return new Effects.DamageContext(context.SourcePawn, context.PawnTarget, damage);
            }
        ),
    };
    public static Skill DoubleAttack = new Skill
    {
        Name = "DoubleAttack",
        Target = Skill.TargetType.ENTITY,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.Damage.Copy().Repeat(2).WithSelect(
            (Context context) =>
            {
                int damage = context.SourcePawn.Get<SkillComponent>()?.Damage ?? 10;
                return new Effects.DamageContext(context.SourcePawn, context.PawnTarget, damage);
            }
        ),
    };
}