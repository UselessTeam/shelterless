using Godot;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class ProjectileAnimation : Node2D
{
    public static ProjectileAnimation Create(Vector2 start, Vector2 end, bool fire = false)
    {
        ProjectileAnimation obj = ResourceLoader.Load<PackedScene>("res://AnimationEffect/ProjectileAnimation/projectile_animation.tscn").Instantiate<ProjectileAnimation>();
        obj.Start = start;
        obj.End = end;
        obj.Fire = fire;
        return obj;
    }

    public static async Task CreateAndWait(Node2D parent, Vector2 end, bool fire = false)
    {
        ProjectileAnimation obj = Create(parent.GlobalPosition, end, fire);
        parent.AddChild(obj);
        await parent.ToSignal(obj, SignalName.Done);
    }

    [Signal]
    public delegate void DoneEventHandler();
    float Height = 200f;
    Vector2 Start;
    Vector2 End;
    bool Fire;
    float Duration = 0.5f;
    float time = 0f;
    public override void _Ready()
    {
        base._Ready();
        Sprite2D sprite = GetNode<Sprite2D>("Sprite");
        if (Fire)
        {
            sprite.Texture = ResourceLoader.Load<Texture2D>("res://Asset/Sprite/FireGrenade.png");
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        time += (float)delta;
        float alpha = time / Duration;
        if (alpha >= 1)
        {
            alpha = 1;
            EmitSignal(SignalName.Done);
            QueueFree();
        }
        GlobalPosition = (1 - alpha) * Start + alpha * End + alpha * (1 - alpha) * 4 * Vector2.Up * Height;
    }
}