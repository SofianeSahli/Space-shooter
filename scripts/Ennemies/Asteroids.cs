using Godot;
using System;

public partial class Asteroids : Area2D
{

	float Score = 50f;
	[Export] float MaxSpeed = 750f;
	[Export] float MinSpeed = 500f;
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
		var camera = GetViewport().GetCamera2D();
		if (camera == null)
			return;

		var screenSize = GetViewport().GetVisibleRect().Size;
		var center = camera.GlobalPosition;
		var halfSize = screenSize * 0.5f * camera.Zoom;
		var left = center.X - halfSize.X;
		var right = center.X + halfSize.X;
		var top = center.Y - halfSize.Y;
		var bottom = center.Y + halfSize.Y;
		var randomX = Randomizer.RandfRange(left, right);
		var randomY = Randomizer.RandfRange(top - 150, top - 50);
		Position = new Vector2(randomX, randomY);
		float randomScale = Randomizer.RandfRange(0.09f, 0.4f);
		Scale = new Vector2(randomScale, randomScale);
		float t = Mathf.InverseLerp(0.09f, 0.4f, randomScale);
		t = Mathf.Clamp(t, 0f, 1f);
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
		if (isDying) return;
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
		GameManager.Instance.AddScore((int)Score);
	}

	private void OnAnimationFinished()
	{
		GD.Print("da");
		QueueFree();
	}
	public void TookDamage()
	{
		if (Animator == null)
			return;
		Animator.Frame = 2;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Animator.AnimationFinished -= OnAnimationFinished;
	}

}