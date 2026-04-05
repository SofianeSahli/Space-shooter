using Godot;
using System;

public partial class Hunter : CharacterBody2D
{
	[Export] public float Speed = 1500f;
	[Export] public float Acceleration = 500f;
	[Export] public float RotationSpeed = 2.5f;

	[Export] public float PreferredDistance = 400f;
	[Export] public float DistanceTolerance = 40f;

	[Export] public float ShootRange = 500f;
	[Export] public float ShootCooldown = 1.2f;

	private float shootTimer = 0f;
	private CharacterBody2D player;
	private int strafeDirection = 1;
	float slowRadius = 700f;
	PackedScene Bullet;
	Marker2D Cannon;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Ready()
	{
		rng.Randomize();
		Bullet = GD.Load<PackedScene>("res://Scenes/Ennemies/EnnemiesBullet.tscn");
		Cannon = GetNode<Marker2D>("Cannon");
		var hitbox = GetNode<Area2D>("Hitbox");
		hitbox.BodyEntered += OnBodyEntered;
		var players = GetTree().GetNodesInGroup("PlayerObject");
		if (players.Count > 0)
		{
			player = players[0] as CharacterBody2D;
		}
		strafeDirection = rng.Randf() > 0.5f ? 1 : -1;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null) return;

		float dt = (float)delta;
		shootTimer -= dt;
		Vector2 toPlayer = player.GlobalPosition - GlobalPosition;
		float distance = toPlayer.Length();
		Vector2 direction = toPlayer / distance;
		Rotation = direction.Angle() + Mathf.Pi / 2f;
		Vector2 targetVelocity = direction * Speed;

		if (distance < slowRadius)
		{
			float t = Mathf.Clamp(distance / slowRadius, 0f, 1f);
			t = t * t;
			targetVelocity *= t;
		}

		if (distance < PreferredDistance)
		{
			if (Velocity.Length() < 10f)
			{
				Velocity = Vector2.Zero;
			}
		}
		Velocity = Velocity.MoveToward(targetVelocity, Acceleration * dt);
		Vector2 separation = Vector2.Zero;
		var hunters = GetTree().GetNodesInGroup("Hunntersa");

		foreach (Node node in hunters)
		{
			if (node == this) continue;

			Hunter other = node as Hunter;
			if (other == null) continue;

			float dist = GlobalPosition.DistanceTo(other.GlobalPosition);

			if (dist < 150f)
			{
				Vector2 away = (GlobalPosition - other.GlobalPosition).Normalized();
				separation += away * (1f / dist);
			}
		}
		Velocity += separation * 300f;
		MoveAndSlide();
		TryShoot(distance);
	}
	private void TryShoot(float distance)
	{
		if (distance > ShootRange) return;

		if (shootTimer <= 0f)
		{
			Shoot();
			shootTimer = ShootCooldown;
		}
	}

	private void Shoot()
	{
		if (player == null) return;

		var shot = Bullet.Instantiate<Bullet>();

		float damage = 25f;
		float speed = 800f;
		float scale = 0.5f;

		shot.ImpactDamage = damage;
		shot.Speed = speed;
		shot.DesiredScale = Vector2.One * scale;
		Vector2 toPlayer = player.GlobalPosition - Cannon.GlobalPosition;
		float distance = toPlayer.Length();
		float travelTime = distance / speed;

		float leadFactor = Mathf.Clamp(distance / 500f, 1f, 2.5f);
		Vector2 predictedPosition = player.GlobalPosition + player.Velocity * travelTime * leadFactor;
		Vector2 shootDirection = (predictedPosition - Cannon.GlobalPosition).Normalized();
		shot.GlobalPosition = Cannon.GlobalPosition;
		shot.Rotation = shootDirection.Angle() + Mathf.Pi / 2f;
		shot.Direction = shootDirection;

		GetTree().CurrentScene.AddChild(shot);
	}
	private void OnBodyEntered(Node body)
	{

		if (body.IsInGroup("Player"))
		{
		}
	}
}
