using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class LocomotionComponent : Component
{
    [Export(PropertyHint.Range, "0,1")] float MovementProgress;

    public async Task MoveTo(Vector2I target)
    {
        Vector2 targetPosition = Pawn.Board.MapToLocal(target);
        isRunning = true;
        positionStart = Pawn.Position;
        positionEnd = targetPosition;
        MovementProgress = 0f;
        await animationComponent.Play("move", Pawn.Coords.SideTowards(target));
        Pawn.SetCoords(target);
        isRunning = false;
    }

    public async Task PushTowards(VectorUtils.Direction direction, int intensity)
    {
        int pushed = 0;
        int pressed = 0;
        while (intensity > 0)
        {
            if (!Pawn.Board.Walkable(Pawn.Coords + direction.ToVector2I() * (pushed + 1)))
            {
                pressed = intensity;
                break;
            }
            pushed += 1;
            intensity -= 1;
        }
        await MoveTo(Pawn.Coords + direction.ToVector2I() * pushed);
        if (pressed > 0)
        {
            GD.Print($"TODO: monster 'pressed' by {pressed} tiles");
        }
    }

    private bool isRunning;

    Vector2 positionStart;
    Vector2 positionEnd;

    AnimationComponent animationComponent => Pawn.Get<AnimationComponent>();

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (isRunning)
        {
            Pawn.Position = positionStart * (1 - MovementProgress) + positionEnd * MovementProgress;
        }
    }
    override protected IEnumerable<Type> GetDependencies()
    {
        yield return typeof(AnimationComponent);
    }

}