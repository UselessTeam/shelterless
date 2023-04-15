using Godot;
using System;

public partial class AnimationComponent : Component
{
    public AnimationPlayer AnimationPlayer
    {
        get
        {
            animationPlayer ??= GetNode<AnimationPlayer>("AnimationPlayer");
            return animationPlayer;
        }
    }

    private AnimationPlayer animationPlayer = null;

    public void LookLeft()
    {
        GetNode<Node2D>("../Sprites").Scale = new Vector2(1, 1);
    }
    public void LookRight()
    {
        GetNode<Node2D>("../Sprites").Scale = new Vector2(-1, 1);
    }
}
