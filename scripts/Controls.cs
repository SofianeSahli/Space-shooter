using Godot;
using System;
using System.Collections.Generic;

public partial class Controls : Node2D
{
	// Reference to the player (CharacterBody2D)
	private CharacterBody2D Player;

	// Movement variables
	[Export] public float Speed;
	[Export] public float Acceleration;
	[Export] public float Friction;

	// Fire variables
	private Marker2D Canon;

	[Signal]
	public delegate void ShootSignalEventHandler(Vector2 Position, PackedScene AmmoScene);

	public Dictionary<string, PackedScene> Ammos = new Dictionary<string, PackedScene>()
	{
		{ "Laser", GD.Load<PackedScene>("res://Scenes/Bullets/BulletRed.tscn") }
	};

	private bool CanShoot = true;
	private Timer BaseShotTimer;

	private Engine engine;

	public override void _Ready()
	{
		// Get the parent (Player)
		Player = GetParent<CharacterBody2D>();

		Canon = Player.GetNode<Marker2D>("Canon");
		BaseShotTimer = Player.GetNode<Timer>("BaseShotTimer");

		engine = Player.GetNode<Marker2D>("EngineMarker").FindChild("*", true, false) as Engine;

		if (engine != null)
		{
			Speed = engine.Stats.Speed;
			Friction = engine.Stats.Friction;
			Acceleration = engine.Stats.Acceleration;
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ui_accept") && CanShoot)
		{
			FireGun();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Player == null) return;
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector2 velocity = Player.Velocity;
		if (direction != Vector2.Zero)
		{
			velocity = velocity.MoveToward(direction * Speed, Acceleration * (float)delta);
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, Friction * (float)delta);
		}

		Player.Velocity = velocity;
		float targetRotation = 0f;
		if (Mathf.Abs(velocity.X) > 5f)
		{
			targetRotation = Mathf.Clamp(velocity.X / Speed, -1f, 1f) * 0.3f;
		}

		Player.Rotation = Mathf.Lerp(Player.Rotation, targetRotation, 5f * (float)delta);

		engine?.ChangeAnimation(velocity);

		Player.MoveAndSlide();
	}

	private void FireGun()
	{
		EmitSignal(SignalName.ShootSignal, Canon.GlobalPosition, Ammos["Laser"]);

		CanShoot = false;
		BaseShotTimer.Start();
	}

	public void OnBaseShotTimeOut()
	{
		CanShoot = true;
	}
}
