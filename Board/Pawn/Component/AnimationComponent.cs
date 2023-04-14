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
}
