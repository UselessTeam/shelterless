using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class LocomotionComponent : Component
{
    [Export(PropertyHint.Range, "0,1")] float MovementProgress;

    private bool isRunning;

    Vector2 positionStart;
    Vector2 positionEnd;

    AnimationComponent animationComponent => Pawn.Get<AnimationComponent>();

    public void StartMovement(Vector2I target)
    {
        isRunning = true;
        positionStart = Pawn.Position;
        positionEnd = Pawn.Board.MapToLocal(target);
        MovementProgress = 0f;
    }

    public void EndMovement(Vector2I target)
    {
        isRunning = false;
        Pawn.SetCoords(target);
    }

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