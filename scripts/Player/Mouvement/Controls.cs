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
	public bool IsDying { get; set; } = false;
	private Engine engine;
	[Signal]
	public delegate void ChargeUpdatedEventHandler(float chargePercent);
	public override void _Ready()
	{
		Player = GetParent<CharacterBody2D>();
		engine = Player.GetNode<Marker2D>("EngineMarker").FindChild("*", true, false) as Engine;
		Player.GetNode<HealthIndicator>("HealthIndicator").ShouldDie += () =>
		{
			IsDying = true;
		};
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
			float chargePercent = currentCharge / maxChargeTime;
			EmitSignal(SignalName.ChargeUpdated, chargePercent);
		}
		if (Input.IsActionJustReleased("ui_accept"))
		{
			EmitSignal(SignalName.ChargeUpdated, 0f);

			ReleaseAttack();
		}

	}

public override void _PhysicsProcess(double delta)
{
    if (Player == null || IsDying) return;

    float dt = (float)delta;
    Vector2 velocity = Player.Velocity;

    // 🔄 ROTATION (left/right)
    float rotationInput = Input.GetAxis("ui_left", "ui_right");
    Player.Rotation += rotationInput * 0.5f * dt;

    // 🚀 FORWARD / BACKWARD (up/down)
    float moveInput = Input.GetAxis("ui_down", "ui_up");

    float maxSpeed = Speed;

    if (moveInput != 0)
    {
        float currentSpeed = Speed;

        // ⬇️ Slower when going backward
        if (moveInput < 0)
        {
            currentSpeed /= 3f;
            maxSpeed /= 3f;
        }

        // 🧭 Forward direction (based on rotation)
        Vector2 forward = -Player.Transform.Y;

        velocity = velocity.MoveToward(
            forward * moveInput * currentSpeed,
            Acceleration * dt
        );
    }
    else
    {
        // 🧊 Apply friction when no input
        velocity = velocity.MoveToward(Vector2.Zero, Friction * dt);
    }

    // 🚫 Clamp speed so it never exceeds max
    velocity = velocity.LimitLength(maxSpeed);

    Player.Velocity = velocity;

    // 🎮 Optional: update engine animation
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
