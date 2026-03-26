using Godot;
using System;

public partial class HealthIndicator : Node2D
{

    [Export]
    public float CurrentHealth = 100f;

    [Export]
    public float MaxHealth = 100f;

    [Export] public bool IsPlayer;

    [Signal]
    public delegate void ShouldDieEventHandler();
    [Signal]
    public delegate void TookDamageEventHandler();
    public override void _Ready()
    {
        UpdateHealth(0);
    }

    public void UpdateHealth(float damage)
    {
        CurrentHealth += damage;
        if (IsPlayer)
        {
            GameManager.Instance.OnHealthUpdate(MaxHealth, CurrentHealth);
        }
        else
        {
            if (CurrentHealth < 0)
            {
                EmitSignal(SignalName.ShouldDie);
            }
            else
            {
                EmitSignal(SignalName.TookDamage);

            }
        }
    }

}
