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

    public void RunMovement(Vector2 movement)
    {
        if (isRunning)
            throw (new Exception("LocomotionComponent is already running"));
        isRunning = true;
        positionStart = Pawn.Position;
        this.movement = movement;
        var animation = Pawn.Get<AnimationComponent>();
        animation.AnimationPlayer.Play("move");
        if (this.movement.X > 0)
            animation.LookRight();
        else if (this.movement.X < 0)
            animation.LookLeft();

        var onAnimationFinished = new AnimationPlayer.AnimationFinishedEventHandler(_ =>
            {
                isRunning = false;
                Pawn.Position = positionStart + this.movement;
                EndMovementEvent?.Invoke();
            }
        );
        animation.AnimationPlayer.AnimationFinished += onAnimationFinished;
        EndMovementEvent += () => animation.AnimationPlayer.AnimationFinished -= onAnimationFinished;
    }

    public event Action EndMovementEvent;

    private bool isRunning;

    Vector2 positionStart;
    Vector2 movement;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (isRunning)
        {
            Pawn.Position = positionStart + movement * MovementProgress;
        }
    }
    override protected IEnumerable<Type> GetDependencies()
    {
        yield return typeof(AnimationComponent);
    }

}