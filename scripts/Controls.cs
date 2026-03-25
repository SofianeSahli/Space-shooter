using Godot;
using System;
using System.Collections.Generic;

public partial class Controls : CharacterBody2D
{

	// Mouvement Variables
	[Export]
	public float Speed;
	[Export] public float Acceleration;
	[Export] public float Friction;
	// Fire variables 
	Engine engine;
	private Marker2D Canon;
	[Signal]
	public delegate void ShootSignalEventHandler(Vector2 Position, PackedScene AmmoScene);
	public Dictionary<string, PackedScene> Ammos = new Dictionary<string, PackedScene>()
	{
		{ "Laser", GD.Load<PackedScene>("res://Scenes/Bullets/BulletRed.tscn") }
	};
	private bool CanShoot = true;
	Timer BaseShotTimer;
	public override void _Ready()
	{
		Canon = GetNode<Marker2D>("Canon");
		BaseShotTimer = GetNode<Timer>("BaseShotTimer");
		engine = GetNode<Marker2D>("EngineMarker").FindChild("*", true, false) as Engine;
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
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		if (direction != Vector2.Zero)
		{
			Velocity = Velocity.MoveToward(direction * Speed, Acceleration * (float)delta);
		}
		else
		{
			Velocity = Velocity.MoveToward(Vector2.Zero, Friction * (float)delta);
		}
		float targetRotation = 0f;
		if (Mathf.Abs(Velocity.X) > 5f)
		{
			targetRotation = Mathf.Clamp(Velocity.X / Speed, -1f, 1f) * 0.3f;
		}
		Rotation = Mathf.Lerp(Rotation, targetRotation, 5f * (float)delta);
		engine?.ChangeAnimation(Velocity);
		MoveAndSlide();
	}

	//For clarification purpose
	public override void _UnhandledInput(InputEvent @event)
	{

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
