using Godot;
using System;

public partial class AnimationComponent : Component
{
    public AnimationPlayer AnimationPlayer => animationPlayer;

    private AnimationPlayer animationPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
}
