using Godot;
using System;
using System.Collections.Generic;

public partial class LocomotionComponent : Component
{
    [Export(PropertyHint.Range, "0,1")] float MovementProgress;

    public void RunToCoords(Vector2I target)
    {
        Vector2 targetPosition = Pawn.Board.MapToLocal(target);
        isRunning = true;
        if (targetPosition.X - Pawn.Position.X > 0)
            animationComponent.LookRight();
        else if (targetPosition.X - Pawn.Position.X < 0)
            animationComponent.LookLeft();
        positionStart = Pawn.Position;
        positionEnd = targetPosition;
        animationComponent.Play("move", () =>
            {
                isRunning = false;
                Pawn.SetCoords(target);
            }
        );
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