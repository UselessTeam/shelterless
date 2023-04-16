using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class TextAnimation : Node2D
{
    public static TextAnimation Create(string text)
    {
        return Create(text, Colors.White);
    }
    public static TextAnimation Create(string text, Color color)
    {
        TextAnimation obj = ResourceLoader.Load<PackedScene>("res://GUI/TextAnimation/text_animation.tscn").Instantiate<TextAnimation>();
        obj.Text = text;
        obj.Color = color;
        return obj;
    }
    string Text = "DEFAULT";
    float MovementMagnitude = 300f;
    float MovementOffset = 100f;
    float Duration = 1f;
    float ProgressDuration = 0f;
    float ProgressRatio => ProgressDuration / Duration;
    Color Color;
    Label Label;
    public override void _Ready()
    {
        base._Ready();
        Label = GetNode<Label>("Label");
        Label.Text = Text;
        Label.Modulate = Color;
        Tween tween = CreateTween().SetParallel(true);
        tween.TweenProperty(this, "position:y", -MovementMagnitude, Duration).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "position:x", GD.RandRange(-MovementOffset, MovementOffset), Duration).SetTrans(Tween.TransitionType.Linear);
        tween.TweenProperty(this, "modulate", Colors.Transparent, Duration).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.In);
        tween.TweenMethod(Callable.From((float scale) => this.FixZoom(scale)), 1f, 1f, Duration);
        tween.Chain().TweenCallback(Callable.From(QueueFree));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        this.FixZoom();
    }
}