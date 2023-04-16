using Godot;
using System;
using System.Threading.Tasks;

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
    public void StartPlay(string animation, Sign sign = Sign.NEUTRAL, params Action[] callbacks)
    {
        switch (sign)
        {
            case Sign.RIGHT:
                LookRight();
                break;
            case Sign.LEFT:
                LookLeft();
                break;
        }
        if (AnimationPlayer.IsPlaying())
        {
            AnimationPlayer.Advance(AnimationPlayer.CurrentAnimationLength - AnimationPlayer.CurrentAnimationPosition);
        }
        currentCallbacks = callbacks;
        currentCallbackIndex = 0;
        AnimationPlayer.Play(animation);
    }

    public async Task Play(string animation, Sign sign = Sign.NEUTRAL, params Action[] callbacks)
    {
        StartPlay(animation, sign, callbacks);
        await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
    }

    public void PlayText(string text)
    {
        PlayText(text, Colors.White);
    }

    public void PlayText(string text, Color color)
    {
        TextAnimation animation = TextAnimation.Create(text, color);
        Pawn.AddChild(animation);
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
