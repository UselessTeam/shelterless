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

    public record ChangeTileContext(Board Board, Vector2I Tile);
    private static IEnumerable<ChangeTileContext> SelectArea(Context context)
    {
        Board board = context.SourcePawn.Board;
        yield return new ChangeTileContext(board, context.CoordsTarget);
        foreach (VectorUtils.Direction direction in VectorUtils.Directions)
        {
            Vector2I tile = context.CoordsTarget + direction.ToVector2I();
            if (board.Exists(tile))
            {
                yield return new ChangeTileContext(board, tile);
            }
        }
    }
    public static Skill FireGrenade = new Skill
    {
        Name = "FireGrenade",
        Target = Skill.TargetType.TILE,
        MinTargetRange = 1,
        MaxTargetRange = 3,
        Effect = new MultiEffectRule<Context>().Then(
            (Context context) =>
            {
                Vector2 origin = context.SourcePawn.Board.MapToLocal(context.SourcePawn.Coords) + 30f * Vector2.Up;
                Vector2 destination = context.SourcePawn.Board.MapToLocal(context.CoordsTarget);
                return ProjectileAnimation.CreateAndWait(context.SourcePawn, origin, destination);
            }
        ).Then(
            new ForEachEffectRule<Context, ChangeTileContext>(
                SelectArea,
                new FunctionEffectRule<ChangeTileContext>(
                    (ChangeTileContext context) =>
                    {
                        TileType tileType = context.Board.GetTileType(context.Tile);
                        if (tileType == TileType.Poisoned)
                        {
                            context.Board.LitTile(context.Tile);
                        }
                    }
                ),
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

internal record struct NewStruct(Board board, Vector2I CoordsTarget)
{
    public static implicit operator (Board board, Vector2I CoordsTarget)(NewStruct value)
    {
        return (value.board, value.CoordsTarget);
    }

    public static implicit operator NewStruct((Board board, Vector2I CoordsTarget) value)
    {
        return new NewStruct(value.board, value.CoordsTarget);
    }
}