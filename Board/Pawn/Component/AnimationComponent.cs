using Godot;
using System;

public partial class AnimationComponent : Component
{
    private AnimationPlayer AnimationPlayer
    {
        get
        {
            animationPlayer ??= GetNode<AnimationPlayer>("AnimationPlayer");
            return animationPlayer;
        }
    }
    private AnimationPlayer animationPlayer = null;

    public void Play(string animation)
    {
        if (AnimationPlayer.IsPlaying())
        {
            AnimationPlayer.Advance(AnimationPlayer.CurrentAnimationLength - AnimationPlayer.CurrentAnimationPosition);
        }
        AnimationPlayer.Play(animation);
    }

    public void OnAnimationFinished(AnimationPlayer.AnimationFinishedEventHandler action)
    {
        AnimationPlayer.AnimationFinished += action;
    }

    public void LookLeft()
    {
        GetNode<Node2D>("../Sprites").Scale = new Vector2(1, 1);
    }
    public void LookRight()
    {
        GetNode<Node2D>("../Sprites").Scale = new Vector2(-1, 1);
    }
}
