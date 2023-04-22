using Godot;
using System;
using System.Threading.Tasks;

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


    ProgressBar _healthBar;

    public override void _Ready()
    {
        base._Ready();
        _healthBar = GetNode<ProgressBar>("ProgressBar");
        _healthBar.MaxValue = MaxHealth;
        _healthBar.Value = CurrentHealth;
    }

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
            CurrentHealth = value;
        }
        _healthBar.Value = CurrentHealth;
    }

    public async Task Die()
    {
        EmitSignal(SignalName.Death);
        await ToSignal(GetTree().CreateTimer(0.3), "timeout");
        Pawn.QueueFree();
    }
}