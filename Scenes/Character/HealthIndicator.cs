using Godot;
using System;

public partial class HealthIndicator : Node2D
{
    
    public float Health = 100f;

    public void UpdateHealth(float damage)
    {
        Health += damage;
        GD.Print("New Health" + Health);
    }
}
