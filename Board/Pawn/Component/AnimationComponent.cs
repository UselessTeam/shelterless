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

    public override void _Ready()
    {
        base._Ready();
        AnimationPlayer.AnimationFinished += OnAnimationFinish;
    }
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
        if (currentCallbackIndex >= currentCallbacks.Length)
        {
            GD.PushError($"Already all triggers (count: {currentCallbacks.Length}) had been consumed for animation {AnimationPlayer.CurrentAnimation}, but requesting a {currentCallbackIndex}th");
        }
        else
        {
            currentCallbacks[currentCallbackIndex]();
        }
        currentCallbackIndex++;
    }

    public void OnAnimationFinish(StringName _)
    {
        if (currentCallbacks.Length - currentCallbackIndex >= 2)
        {
            GD.PrintErr($"Weird amount of final trigger: {currentCallbacks.Length - currentCallbackIndex}");
        }
        while (currentCallbackIndex < currentCallbacks.Length)
        {
            currentCallbacks[currentCallbackIndex]();
            currentCallbackIndex++;
        }
        currentCallbacks = null;
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
