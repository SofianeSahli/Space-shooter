using Godot;
using System;

public partial class Controls : Node2D
{
	private PlayerObject Player;

	[Signal] public delegate void ShootSignalEventHandler(float ChargePercent);
	[Signal] public delegate void ChargeUpdatedEventHandler(float chargePercent);
	[Signal] public delegate void BoostChangedEventHandler(float value, bool isCoolingDown);

	[Signal]
	public delegate void SpeedChangedEventHandler(float Spped);
	private float currentCharge = 0f;
	private float maxChargeTime = 2f;
	private bool isCharging = false;

	public bool IsDying { get; set; } = false;
	private Engine engine;
	private float lastBoostValue = -1f;
	private bool lastCoolingState = false;


	public override void _Ready()
	{
		Player = GetParent<PlayerObject>();

		engine = Player.GetNode<Marker2D>("EngineMarker")
					   .FindChild("*", true, false) as Engine;

		Player.GetNode<HealthIndicator>("HealthIndicator").ShouldDie += () =>
		{
			IsDying = true;
		};


	}

	private void ReleaseAttack()
	{
		if (!isCharging) return;

		isCharging = false;
		float chargePercent = currentCharge / maxChargeTime;
		EmitSignal(SignalName.ShootSignal, chargePercent);
		currentCharge = 0f;
	}

	// ------------------- PHYSICS -------------------

	public override void _PhysicsProcess(double delta)
	{
		if (Player == null || IsDying) return;

		float dt = (float)delta;
		HandleBoost(dt);
		PhysicsHandling(dt);
		float speedRatio = Player.Velocity.Length() / engine.Stats.Speed;

		EmitSignal(SignalName.SpeedChanged, Math.Abs(speedRatio));

	}
	// ------------------- CHARGE SYSTEM -------------------

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
	// ------------------- BOOST SYSTEM (HOLD BASED) -------------------

	private void HandleBoost(float dt)
	{
		bool boostHeld = Input.IsActionPressed("Boost");

		// Cooldown takes priority
		if (engine.Stats.CooldownTimer > 0)
		{
			engine.Stats.CooldownTimer -= dt;
			engine.Stats.IsBoosting = false;
			return;
		}
		// 🔥 While boosting
		if (boostHeld && engine.Stats.BoostEnergy > 0f)
		{
			engine.Stats.IsBoosting = true;
			engine.Stats.BoostEnergy -= engine.Stats.DepletionRate * dt;
			engine.Stats.RechargeDelayTimer = engine.Stats.RechargeDelay;

			if (engine.Stats.BoostEnergy <= 0f)
			{
				engine.Stats.BoostEnergy = 0f;
				engine.Stats.IsBoosting = false;
				engine.Stats.CooldownTimer = engine.Stats.BoostCooldown;
			}
		}
		else
		{
			engine.Stats.IsBoosting = false;

			if (engine.Stats.RechargeDelayTimer > 0f)
			{
				engine.Stats.RechargeDelayTimer -= dt;
			}
			else
			{
				if (engine.Stats.BoostEnergy < engine.Stats.MaxBoostEnergy)
				{
					engine.Stats.BoostEnergy += engine.Stats.RechargeRate * dt;
					engine.Stats.BoostEnergy = Mathf.Min(
						engine.Stats.BoostEnergy,
						engine.Stats.MaxBoostEnergy
					);
				}
			}
		}

		BoostNotifier();
	}
	// ------------------- MOVEMENT -------------------

	private void PhysicsHandling(float dt)
	{
		Vector2 velocity = Player.Velocity;

		float rotationInput = Input.GetAxis("ui_left", "ui_right");
		Player.Rotation += rotationInput * engine.Stats.RotationSpeed * dt;

		float moveInput = Input.GetAxis("ui_down", "ui_up");
		float maxSpeed = engine.Stats.Speed;

		if (moveInput != 0)
		{
			float currentSpeed = engine.Stats.Speed;
			float currentAcceleration = engine.Stats.Acceleration;

			if (moveInput < 0)
			{
				currentSpeed /= 3f;
				maxSpeed /= 3f;
			}

			if (engine.Stats.IsBoosting)
			{
				engine.UpdateAnimation(EngineData.BOOST_ANIMATION);
				currentSpeed *= engine.Stats.BoostMultiplier;
				maxSpeed *= engine.Stats.BoostMultiplier;
				currentAcceleration *= engine.Stats.BoostAccelerationModifier;
			}
			else
			{
				engine.UpdateAnimation(EngineData.MOVING_ANIMATION);
			}

			Vector2 forward = -Player.Transform.Y;

			if (engine.Stats.IsBoosting && moveInput > 0)
			{
				velocity += forward * 200f;
			}

			velocity = velocity.MoveToward(
				forward * moveInput * currentSpeed,
				currentAcceleration * dt
			);
		}
		else
		{
			engine.UpdateAnimation("");
			velocity = velocity.MoveToward(
				Vector2.Zero,
				engine.Stats.Friction * dt
			);
		}
		velocity = velocity.LimitLength(maxSpeed);
		Player.Velocity = velocity;
		engine?.ChangeAnimation(velocity);
		Player.MoveAndSlide();
		ClampPlayerToBounds();
	}
	// ------------------- BOOST UI -------------------

	private void ClampPlayerToBounds()
	{
		Vector2 pos = Player.GlobalPosition;

		var bounds = GameManager.Instance.WorldBounds;

		float padding = 50f;

		float minX = bounds.Position.X + padding;
		float maxX = bounds.Position.X + bounds.Size.X - padding;
		float minY = bounds.Position.Y + padding;
		float maxY = bounds.Position.Y + bounds.Size.Y - padding;

		pos.X = Mathf.Clamp(pos.X, minX, maxX);
		pos.Y = Mathf.Clamp(pos.Y, minY, maxY);

		// Stop velocity when hitting edges
		if (pos.X <= minX || pos.X >= maxX)
		{
			Player.Velocity = new Vector2(Player.Velocity.X, 0);

		}
		if (pos.Y <= minY || pos.Y >= maxY)
		{
			Player.Velocity = new Vector2(0, Player.Velocity.Y);

		}

		Player.GlobalPosition = pos;
	}
	private void BoostNotifier()
	{
		float newValue = engine.Stats.BoostEnergy / engine.Stats.MaxBoostEnergy;
		bool isCooling = engine.Stats.CooldownTimer > 0;
		if (Mathf.IsEqualApprox(newValue, lastBoostValue) && isCooling == lastCoolingState)
			return;

		lastBoostValue = newValue;
		lastCoolingState = isCooling;

		EmitSignal(SignalName.BoostChanged, newValue, isCooling);
	}

}