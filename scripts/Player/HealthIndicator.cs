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
    [Signal]
    public delegate void PlayerTookDamageEventHandler(float CurrentHeal, float MaxHealth);
    public override void _Ready()
    {
        UpdateHealth(0);
    }

    public void UpdateHealth(float damage)
    {
        CurrentHealth += damage;
        if (IsPlayer)
        {
            EmitSignal(SignalName.PlayerTookDamage, CurrentHealth, MaxHealth);
            if (CurrentHealth < 0)
            {
                EmitSignal(SignalName.ShouldDie);
            
            }
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
