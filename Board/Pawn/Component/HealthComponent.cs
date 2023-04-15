using Godot;
using System;

public partial class HealthComponent : Component
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public float healthRatio { get => MaxHealth <= 0 ? 0f : CurrentHealth / MaxHealth; }

    public override void _Ready()
    {
        base._Ready();
        GD.Print(healthRatio);
    }
}