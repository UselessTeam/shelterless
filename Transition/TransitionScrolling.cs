using Godot;
using System;

public partial class TransitionScrolling : TileMap
{
    Node2D LeftLimit;
    Node2D RightLimit;
    public override void _Ready()
    {
        base._Ready();
        LeftLimit = GetNode<Node2D>("LeftLimit");
        RightLimit = GetNode<Node2D>("RightLimit");
    }

    [Export] float scrollingSpeed;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Position += scrollingSpeed * (float) delta * Vector2.Left;
        if(RightLimit.GlobalPosition.X < 0)
            Position += (RightLimit.Position.X - LeftLimit.Position.X) * Vector2.Right; 
    }
}
