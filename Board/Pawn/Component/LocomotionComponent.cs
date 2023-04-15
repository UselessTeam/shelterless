using Godot;
using System;

public partial class LocomotionComponent : Component
{
    [Export] public bool IsRunning { private get; set; }
    [Export(PropertyHint.Range, "0,1")] float MovementProgress;


    public void RunMovement(Vector2 movement)
    {
        if (IsRunning)
            throw (new Exception("LocomotionComponent is already running"));
        PositionStart = Pawn.Position;
        Movement = movement;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("jump");
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

}