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
    private Action[] currentCallbacks;
    private int currentCallbackIndex;

    public void Play(string animation, params Action[] callbacks)
    {
        if (AnimationPlayer.IsPlaying())
        {
            AnimationPlayer.Advance(AnimationPlayer.CurrentAnimationLength - AnimationPlayer.CurrentAnimationPosition);
        }
        currentCallbacks = callbacks;
        currentCallbackIndex = 0;
        AnimationPlayer.Play(animation);
    }

    public void Trigger()
    {
        currentCallbacks[currentCallbackIndex]();
        currentCallbackIndex++;
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
