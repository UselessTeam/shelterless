using System.Threading.Tasks;
using Godot;

public partial class Context : GodotObject
{
    public interface ITarget { }
    public struct TargetCoords : ITarget
    {
        Vector2I Value;
        public static implicit operator TargetCoords(Vector2I value) => new TargetCoords { Value = value };
        public static implicit operator Vector2I(TargetCoords value) => value.Value;
    }

    public struct TargetPawn : ITarget
    {
        Pawn Value;
        public static implicit operator TargetPawn(Pawn value) => new TargetPawn { Value = value };
        public static implicit operator Pawn(TargetPawn value) => value.Value;
    }

    public struct TargetDirection : ITarget
    {
        VectorUtils.Direction Value;
        public static implicit operator TargetDirection(VectorUtils.Direction value) => new TargetDirection { Value = value };
        public static implicit operator VectorUtils.Direction(TargetDirection value) => value.Value;
    }
    public Pawn SourcePawn;
    public Skill SourceSkill;
    public Vector2I Origin; //TODO: Do the same as Target
    public ITarget Target;
    public Vector2I CoordsTarget
    {
        get
        {
            return Target is TargetCoords coords ? coords : VectorUtils.INVALID_VECTOR2I;
        }
        set
        {
            Target = (TargetCoords)value;
        }
    }
    public Pawn PawnTarget
    {
        get
        {
            return Target is TargetPawn pawn ? pawn : null;
        }
        set
        {
            Target = (TargetPawn)value;
        }
    }
    public VectorUtils.Direction DirectionTarget
    {
        get
        {
            return Target is TargetDirection direction ? direction : VectorUtils.Direction.NONE;
        }
        set
        {
            Target = (TargetDirection)value;
        }
    }

    public async Task Run()
    {
        await SourceSkill.Effect.RunOn(this);
    }
}