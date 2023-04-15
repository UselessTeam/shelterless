using Godot;
using System;
using System.Collections.Generic;

public partial class LocomotionComponent : Component
{
    [Export] public bool IsRunning { private get; set; } = false;
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
        if (IsRunning)
            throw (new Exception("LocomotionComponent is already running"));
        PositionStart = Pawn.Position;
        Movement = movement;
        var animation = Pawn.Get<AnimationComponent>();
        animation.AnimationPlayer.Play("move");
        if (Movement.X > 0)
            animation.LookRight();
        else if (Movement.X < 0)
            animation.LookLeft();
    }

    public event Action EndMovementEvent;

    private void NotifyEndMovement()
    {
        EndMovementEvent?.Invoke();
    }

    Vector2 PositionStart;
    Vector2 Movement;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (IsRunning)
        {
            Pawn.Position = PositionStart + Movement * MovementProgress;
        }
    }
    override protected IEnumerable<Type> GetDependencies()
    {
        yield return typeof(AnimationComponent);
    }

}