using Godot;
using System;
using System.Collections.Generic;

public partial class LocomotionComponent : Component
{
    [Export(PropertyHint.Range, "0,1")] float MovementProgress;

    public void RunToCoords(Vector2I target)
    {
        RunToTargetPosition(Pawn.Board.MapToLocal(target));
    }

    public void RunToTargetPosition(Vector2 target)
    {
        animationComponent.Play("move");
        if (target.X - Pawn.Position.X > 0)
            animationComponent.LookRight();
        else if (target.X - Pawn.Position.X < 0)
            animationComponent.LookLeft();
        isRunning = true;
        positionStart = Pawn.Position;
        positionEnd = target;
    }

    public event Action EndMovementEvent;

    private bool isRunning;

    Vector2 positionStart;
    Vector2 positionEnd;

    AnimationComponent animationComponent => Pawn.Get<AnimationComponent>();

    public override void _Ready()
    {
        base._Ready();

        animationComponent.OnAnimationFinished(_ =>
            {
                if (isRunning)
                {
                    isRunning = false;
                    Pawn.Position = this.positionEnd;
                    EndMovementEvent?.Invoke();
                }
            }
        );
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