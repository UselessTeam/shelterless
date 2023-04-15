using Godot;
using System;

public partial class HealthComponent : Component
{
    [Export(PropertyHint.Range, "1,1000,or_greater")]
    public int CurrentHealth { get; private set; } = 100;

    [Export(PropertyHint.Range, "1,1000,or_greater")]
    public int MaxHealth { get; private set; } = 100;
    public float healthRatio { get => MaxHealth <= 0 ? 0f : (float)CurrentHealth / MaxHealth; }

    [Signal]
    public delegate void HealthChangedEventHandler(int oldValue, int newValue);
    [Signal]
    public delegate void DeathEventHandler();

    public void ChangeHealth(int value)
    {
        SetHealth(CurrentHealth + value);
    }

    public void SetHealth(int value)
    {
        if (value > MaxHealth)
        {
            value = MaxHealth;
        }
        else if (value < 0)
        {
            value = 0;
        }
        if (value != CurrentHealth)
        {
            EmitSignal(SignalName.HealthChanged, CurrentHealth, value);
            if (value == 0)
            {
                Die();
            }
            CurrentHealth = value;
        }
    }

    private void Die()
    {
        EmitSignal(SignalName.Death);
        Pawn.QueueFree();
    }
}