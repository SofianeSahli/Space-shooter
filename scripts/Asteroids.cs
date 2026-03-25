using Godot;
using System;

public partial class Asteroids : Area2D
{
	[Export]
	float MaxSpeed = 200f;
	[Export]
	float MinSpeed = 200f;
	float Speed = 0;
	[Export]
	float RotationMaxAngle;
	[Export]
	float RotationMinAngle;
	float RotationAngle = 0;
	RandomNumberGenerator Randomizer = new RandomNumberGenerator();
	float MouvementX;
	// Called when the node enters the scene tree for the first time.
	[Export]
	float ImpactDamage;
	public override void _Ready()
	{
		var width = GetViewport().GetVisibleRect().Size.X;
		var randomX = Randomizer.RandiRange(0, (int)width - 200);
		var randomY = Randomizer.RandiRange(-150, -50);
		Position = new Vector2(randomX, randomY);
		float randomScale = Randomizer.RandfRange(0.09f, 0.4f);
		Scale = new Vector2(randomScale, randomScale);
		float t = Mathf.InverseLerp(0.09f, 0.4f, randomScale);
		Speed = Mathf.Lerp(MaxSpeed, MinSpeed, t);
		ImpactDamage = Mathf.Lerp(20f, 50f, t);
		RotationAngle = Mathf.Lerp(RotationMinAngle, RotationAngle,t); 
		MouvementX = Randomizer.RandfRange(-0.8f, 0.8f);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += new Vector2(MouvementX, 1f) * Speed * (float)delta;
		Rotate(RotationAngle);
	}

	public void CollisionDetected(Node2D body)
	{
		HealthIndicator Health = body.GetNodeOrNull<HealthIndicator>("HealthIndicator");
		if (Health == null) return;
		Health.UpdateHealth(-ImpactDamage);
		Destroy();
	}

	public void Destroy()
	{
		QueueFree();
	}
}
