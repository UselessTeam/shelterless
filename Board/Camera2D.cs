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
        if (@event.IsActionPressed("CenterCamera"))
        {
            Position = PlayerComponent.Main.Pawn.Position;
            GUI.Main.LooseFocus();
        }
        if (CurrentZoomTicks < MaxZoomTicks && @event.IsActionPressed("ZoomIn"))
        {
            CurrentZoomTicks++;
            Zoom *= ZoomRatio;
            GetViewport().SetInputAsHandled();
        }
        if (CurrentZoomTicks > MinZoomTicks && @event.IsActionPressed("ZoomOut"))
        {
            CurrentZoomTicks--;
            Zoom /= ZoomRatio;
            GetViewport().SetInputAsHandled();
        }
        if (@event.IsActionPressed("DragCamera"))
        {
            dragging = true;
            GetViewport().SetInputAsHandled();
        }
        if (@event.IsActionReleased("DragCamera"))
        {
            dragging = false;
            GetViewport().SetInputAsHandled();
        }
        if (dragging && @event is InputEventMouseMotion motion)
        {
            Position -= motion.Relative / Zoom;
            GetViewport().SetInputAsHandled();
        }
    }
}
