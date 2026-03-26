using Godot;
using System;

public partial class Bullet : Area2D
{

	public float Speed { get; set; }
	public float ImpactDamage { get; set; }
	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{
		Position += Vector2.Up * Speed * (float)delta;
	}
	public void CollisionDetected(Node2D body)
	{
		HealthIndicator Health = body.GetNodeOrNull<HealthIndicator>("HealthIndicator");
		if (Health == null) return;
		Health.UpdateHealth(-ImpactDamage);
		QueueFree();
	}

}
