using Godot;
using System;
using System.Collections.Generic;

public partial class Camera2D : Godot.Camera2D
{
    [Export] int MinZoomTicks;
    [Export] int MaxZoomTicks;

    [Export] float ZoomRatio;

    [Export] int CurrentZoomTicks;

    bool dragging = false;

    Dictionary<int, Vector2> ZoomValues = new();

    public override void _Ready()
    {
        base._Ready();
        if (MinZoomTicks > 0 || MaxZoomTicks < 0)
            throw new Exception("ZoomTicks are wrongly set");
        if (MinZoomTicks > CurrentZoomTicks || MaxZoomTicks < CurrentZoomTicks)
            throw new Exception("CurrentZoomTicks is wrongly set");
        var zoomIn = Zoom;
        for (int i = 0; i <= MaxZoomTicks; i++)
        {
            ZoomValues[i] = zoomIn;
            zoomIn *= ZoomRatio;
        }
        var zoomOut = Zoom;
        for (int i = -1; i >= MinZoomTicks; i--)
        {
            zoomOut /= ZoomRatio;
            ZoomValues[i] = zoomOut;
        }

        Zoom = ZoomValues[CurrentZoomTicks];

        var topLeftLimit = GetNode<Node2D>("../TopLeftLimit");
        var bottomRightLimit = GetNode<Node2D>("../BottomRightLimit");

        LimitTop = (int) topLeftLimit.Position.Y;
        LimitLeft = (int) topLeftLimit.Position.X;
        LimitRight = (int) bottomRightLimit.Position.X;
        LimitBottom = (int) bottomRightLimit.Position.Y;
    }

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
            Zoom = ZoomValues[CurrentZoomTicks];
            GetViewport().SetInputAsHandled();
        }
        if (CurrentZoomTicks > MinZoomTicks && @event.IsActionPressed("ZoomOut"))
        {
            CurrentZoomTicks--;
            Zoom = ZoomValues[CurrentZoomTicks];
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
