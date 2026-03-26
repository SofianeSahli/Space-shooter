using Godot;
using System;

public partial class Asteroids : Area2D
{
	[Export] float MaxSpeed = 200f;
	[Export] float MinSpeed = 200f;
	[Export] float RotationMaxAngle;
	[Export] float RotationMinAngle;
	[Export] float ImpactDamage;
	private float Speed = 0;
	private float RotationAngle = 0;
	private float MouvementX;
	private RandomNumberGenerator Randomizer = new RandomNumberGenerator();
	private AnimatedSprite2D Animator;
	private CollisionPolygon2D Collider;
	private bool isDying = false;

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
		RotationAngle = Mathf.Lerp(RotationMinAngle, RotationMaxAngle, t);
		MouvementX = Randomizer.RandfRange(-0.8f, 0.8f);
		Animator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Collider = GetNode<CollisionPolygon2D>("CollisionPolygon2D");
		Animator.AnimationFinished += OnAnimationFinished;
		Animator.Play("Explosion");
		Animator.Stop();
	}

	public override void _Process(double delta)
	{
		if (isDying) return; // ✅ stop movement when dying

		Position += new Vector2(MouvementX, 1f) * Speed * (float)delta;
		Rotate(RotationAngle);
	}

	public void CollisionDetected(Node2D body)
	{
		if (isDying) return;

		HealthIndicator health = body.GetNodeOrNull<HealthIndicator>("HealthIndicator");
		if (health == null) return;
		health.UpdateHealth(-ImpactDamage);
		ShouldDie();
	}

	public void ShouldDie()
	{
		if (isDying) return;
		isDying = true;
		if (Collider != null)
			Collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		Animator.Play("Explosion");
	}

	private void OnAnimationFinished()
	{
		QueueFree();
	}
	public void TookDamage()
	{ 
		if(Animator == null)
		return;
		Animator.Frame = 2;
	}
}