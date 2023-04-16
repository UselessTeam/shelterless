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


    Vector2 topLeftLimit;
    Vector2 bottomRightLimit;
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

        topLeftLimit = GetNode<Node2D>("../TopLeftLimit").Position;
        bottomRightLimit = GetNode<Node2D>("../BottomRightLimit").Position;

        LimitTop = (int)topLeftLimit.Y;
        LimitLeft = (int)topLeftLimit.X;
        LimitRight = (int)bottomRightLimit.X;
        LimitBottom = (int)bottomRightLimit.Y;
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
            ClampPosition();
        }
        if (CurrentZoomTicks > MinZoomTicks && @event.IsActionPressed("ZoomOut"))
        {
            CurrentZoomTicks--;
            Zoom = ZoomValues[CurrentZoomTicks];
            GetViewport().SetInputAsHandled();
            ClampPosition();
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
            ClampPosition();
        }
    }

    void ClampPosition()
    {

        Position = Position.Clamp(topLeftLimit + GetViewportRect().Size, bottomRightLimit - GetViewportRect().Size);
    }


}
