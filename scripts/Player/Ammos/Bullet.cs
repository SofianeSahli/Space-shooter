using Godot;
using System;

public partial class Bullet : Area2D
{

	public float Speed { get; set; }
	public float ImpactDamage { get; set; }
	public Vector2 DesiredScale { get; set; }
	public override void _Ready()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", DesiredScale, 0.5f)
				 .SetTrans(Tween.TransitionType.Bounce)
				 .SetEase(Tween.EaseType.Out);

	}

	public override void _Process(double delta)
	{
		Vector2 forward = -Transform.Y;
		Position += forward * Speed * (float)delta;
	}
	public void CollisionDetected(Node2D body)
	{
		HealthIndicator Health = body.GetNodeOrNull<HealthIndicator>("HealthIndicator");
		if (Health == null) return;
		Health.UpdateHealth(-ImpactDamage);
		QueueFree();
	}
	public void Destroy()
	{
		QueueFree();
	}
}
