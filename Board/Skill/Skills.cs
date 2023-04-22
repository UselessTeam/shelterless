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


    public static Skill WindGrenade = new Skill
    {
        Name = "WindGrenade",
        Target = Skill.TargetType.TILE,
        MinTargetRange = 2,
        MaxTargetRange = 4,
        Effect = Effects.ThrowProjectile.WithSelect(
            (Context context) => new Effects.ThrowProjectileContext(
                Attacker: context.SourcePawn,
                Receiver: context.CoordsTarget           
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