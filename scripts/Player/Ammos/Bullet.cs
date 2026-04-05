using Godot;
using System;

public partial class Bullet : Area2D
{
	private PackedScene ImpactScene = GD.Load<PackedScene>("res://Scenes/Bullets/BulletImpact.tscn");

	public float Speed { get; set; }
	public float ImpactDamage { get; set; }
	public Vector2 DesiredScale { get; set; }
	public Vector2 Direction { get; set; } = Vector2.Zero;
	public override void _Ready()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", DesiredScale, 0.5f)
				 .SetTrans(Tween.TransitionType.Bounce)
				 .SetEase(Tween.EaseType.Out);

	}

	public override void _Process(double delta)
	{

		Vector2 moveDir;
		if (Direction != Vector2.Zero)
			moveDir = Direction;
		else
			moveDir = -Transform.Y; // fallback (player shooting)
		Position += moveDir * Speed * (float)delta;
	}
	public void CollisionDetected(Node2D body)
	{
		HealthIndicator Health = body.GetNodeOrNull<HealthIndicator>("HealthIndicator");
		if (Health == null) return;
		Health.UpdateHealth(-ImpactDamage);
		var impact = ImpactScene.Instantiate<Node2D>();
		GD.Print("Spawn");
		GetTree().CurrentScene.AddChild(impact);

		impact.GlobalPosition = GlobalPosition;
		QueueFree();
	}
	public void Destroy()
	{
		QueueFree();
	}
}
