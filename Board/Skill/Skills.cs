using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                return new Effects.PushAttackContext(
                    context.SourcePawn,
                    targetPawn,
                    context.DirectionTarget,
                    Mathf.Max(1, context.SourcePawn.Get<SkillComponent>()?.PushStrength - targetPawn?.Get<SkillComponent>().Weight ?? 0)
                );
            }
        ),
    };

    private static IEnumerable<Effects.PushContext> PushAround(Context context)
    {
        foreach (VectorUtils.Direction direction in VectorUtils.Directions)
        {
            Pawn pawn = context.SourcePawn.Board.GetFirstPawnAt(context.CoordsTarget + direction.ToVector2I());
            if (pawn is not null)
            {
                yield return new Effects.PushContext(pawn, direction, 1);
            }
        }
    }
    public static Skill WindGrenade = new Skill
    {
        Name = "WindGrenade",
        Target = Skill.TargetType.TILE,
        MinTargetRange = 2,
        MaxTargetRange = 4,
        Effect = new MultiEffectRule<Context>().Then(
            (Context context) =>
            {
                Vector2 origin = context.SourcePawn.Board.MapToLocal(context.SourcePawn.Coords) + 20f * Vector2.Up;
                Vector2 destination = context.SourcePawn.Board.MapToLocal(context.CoordsTarget);
                return ProjectileAnimation.CreateAndWait(context.SourcePawn, origin, destination);
            }
        ).Then(
            new ForEachEffectRule<Context, Effects.PushContext>(
                PushAround,
                Effects.Push,
                parallel: true
            )
        )
    };


    public static Skill SingleAttack = new Skill
    {
        Name = "SingleAttack",
        Target = Skill.TargetType.ENTITY,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.Attack.WithSelect(
            (Context context) =>
            {
                int damage = context.SourcePawn.Get<SkillComponent>()?.Damage ?? 10;
                return new Effects.AttackContext(context.SourcePawn, context.PawnTarget, damage);
            }
        ),
    };
    public static Skill DoubleAttack = new Skill
    {
        Name = "DoubleAttack",
        Target = Skill.TargetType.ENTITY,
        MinTargetRange = 1,
        MaxTargetRange = 1,
        Effect = Effects.Attack.Copy().Repeat(2).WithSelect(
            (Context context) =>
            {
                int damage = context.SourcePawn.Get<SkillComponent>()?.Damage ?? 10;
                return new Effects.AttackContext(context.SourcePawn, context.PawnTarget, damage);
            }
        ),
    };
}