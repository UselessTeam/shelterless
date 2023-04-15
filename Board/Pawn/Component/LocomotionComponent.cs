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
        RunMovement(target - Pawn.Position);
    }

    private bool isRunning;

    public void RunMovement(Vector2 movement)
    {
        if (isRunning)
            throw (new Exception("LocomotionComponent is already running"));
        isRunning = true;
        PositionStart = Pawn.Position;
        Movement = movement;
        var animation = Pawn.Get<AnimationComponent>();
        animation.AnimationPlayer.Play("move");
        if (Movement.X > 0)
            animation.LookRight();
        else if (Movement.X < 0)
            animation.LookLeft();
        animation.AnimationPlayer.AnimationFinished +=
            (_ =>
                {
                    isRunning = false;
                    Pawn.Position = PositionStart + Movement;
                    EndMovementEvent?.Invoke();
                }
            );
    }

    public event Action EndMovementEvent;

    Vector2 PositionStart;
    Vector2 Movement;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (isRunning)
        {
            Pawn.Position = PositionStart + Movement * MovementProgress;
        }
    }
    override protected IEnumerable<Type> GetDependencies()
    {
        yield return typeof(AnimationComponent);
    }

}