using Godot;
using System;
using System.Collections.Generic;

public partial class Controls : Node2D
{
	private CharacterBody2D Player;
	[Export] public float Speed;
	[Export] public float Acceleration;
	[Export] public float Friction;
	private Marker2D Canon;
	[Signal]
	public delegate void ShootSignalEventHandler(float ChargePercent);
	private float currentCharge = 0f;
	private float maxChargeTime = 2f;
	private bool isCharging = false;

	private Engine engine;

	public override void _Ready()
	{
		Player = GetParent<CharacterBody2D>();
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
		if (Input.IsActionPressed("ui_accept"))
		{
			isCharging = true;
			currentCharge += (float)delta;
			currentCharge = Mathf.Min(currentCharge, maxChargeTime);
		}
		if (Input.IsActionJustReleased("ui_accept"))
		{
			ReleaseAttack();
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

	private void ReleaseAttack()
	{
		if (!isCharging) return;

		isCharging = false;

		float chargePercent = currentCharge / maxChargeTime;

		EmitSignal(SignalName.ShootSignal, chargePercent);

		currentCharge = 0f;
	}


}
