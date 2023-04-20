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
    [Signal]
    public delegate void DoneEventHandler();
    private TaskSequence taskSequence;

    public override void _Ready()
    {
        base._Ready();
        AnimationPlayer.AnimationFinished += OnAnimationFinish;
    }
    public void StartPlay(string animation, Sign sign = Sign.NEUTRAL, TaskSequence taskSequence = null)
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
            // TODO: Yield
        }
        this.taskSequence = taskSequence;
        taskSequence?.StartNext();
        AnimationPlayer.Play(animation);
    }

    public async Task Play(string animation, Sign sign = Sign.NEUTRAL, TaskSequence taskSequence = null)
    {
        StartPlay(animation, sign, taskSequence);
        await ToSignal(this, SignalName.Done);
        await taskSequence.Join();
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
        try
        {
            taskSequence?.StartNext();
        }
        catch (IndexOutOfRangeException exception)
        {
            GD.PrintErr($"Error during {AnimationPlayer.CurrentAnimation}");
            GD.PushError(exception);
        }
    }

    public void OnAnimationFinish(StringName _)
    {
        if ((taskSequence?.GetRemainingCount() ?? 0) >= 2)
        {
            GD.PrintErr($"Weird amount of final trigger: {taskSequence.GetRemainingCount()}");
        }
        taskSequence.StartRemaining();
        EmitSignal(SignalName.Done);
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
