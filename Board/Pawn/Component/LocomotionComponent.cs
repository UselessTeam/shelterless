using Godot;
using System;
using System.Collections.Generic;

public partial class LocomotionComponent : Component
{
    [Export(PropertyHint.Range, "0,1")] float MovementProgress;

    public void MoveTo(Vector2I target)
    {
        Vector2 targetPosition = Pawn.Board.MapToLocal(target);
        animationComponent.Play("move", Pawn.Coords.SideTowards(target), () =>
            {
                isRunning = false;
                Pawn.SetCoords(target);
            }
        );
        isRunning = true;
        positionStart = Pawn.Position;
        positionEnd = targetPosition;
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