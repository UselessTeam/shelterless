using Godot;
using System;

public partial class Camera2D : Godot.Camera2D
{
    [Export] int MaxZoomTicks;
    [Export] int MinZoomTicks;

    [Export] float ZoomRatio;

    int CurrentZoomTicks = 0;

    bool dragging = false;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (CurrentZoomTicks < MaxZoomTicks && @event.IsActionPressed("ZoomIn"))
        {
            CurrentZoomTicks++;
            Zoom *= ZoomRatio;
        }
        if (CurrentZoomTicks > MinZoomTicks && @event.IsActionPressed("ZoomOut")){
            CurrentZoomTicks --;
            Zoom /= ZoomRatio;
        }
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Right)
            dragging = !dragging;
        if (dragging && @event is InputEventMouseMotion motion)
            Position -= motion.Relative / Zoom;
    }
}
