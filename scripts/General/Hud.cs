using Godot;
using System;

public partial class Hud : Control
{
	private HSlider HealthBar;
	private HSlider ChargeBar;
	private HSlider BoostSlider;
	private ColorRect SpeedMeter;
	private Label ScoreLabel;
	private Label TimerLabel;

	private float elapsedTime = 0f;

	private float targetBoostValue = 100f;
	private float targetCharge = 0f;
	float currentAngle;
	public override void _Ready()
	{
		HealthBar = GetNode<HSlider>("BottomContainer/HBoxContainer/BottomLeftStatus/Box/HealthContainer/HealthBar");
		ChargeBar = GetNode<HSlider>("BottomContainer/HBoxContainer/BottomLeftStatus/Box/ChargedShootContainer/CannonCharge");
		BoostSlider = GetNode<HSlider>("BottomContainer/HBoxContainer/BottomLeftStatus/Box/EngineBoostContainer/EngineBoost");
		SpeedMeter = GetNode<ColorRect>("BottomContainer/HBoxContainer/BottomCenterStatus/SpeedMeter");
		ScoreLabel = GetNode<Label>("MarginContainer/HBoxContainer/ScoreLabel");
		TimerLabel = GetNode<Label>("MarginContainer/HBoxContainer2/TimerLabel");
		GameManager.Instance.ScoreChanged += OnScoreUpdated;

		var players = GetTree().GetNodesInGroup("PlayerObject");

		if (players.Count > 0)
		{
			var player = players[0];

			var control = player.GetNode<Controls>("ControlNode");
			var health = player.GetNodeOrNull<HealthIndicator>("HealthIndicator");

			if (health != null)
			{
				health.PlayerTookDamage += OnHealthChanged;
				OnHealthChanged(health.CurrentHealth, health.MaxHealth);
			}

			control.ChargeUpdated += OnChargeUpdated;
			control.BoostChanged += OnBoostChanged;
			control.SpeedChanged += OnSpeedChanged;
		}
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;


		ChargeBar.Value = Mathf.Lerp(
			(float)ChargeBar.Value,
			targetCharge,
			150f * dt
		);

		BoostSlider.Value = Mathf.MoveToward(
			(float)BoostSlider.Value,
			targetBoostValue,
			80f * dt
		);
		elapsedTime += dt;

		int minutes = (int)(elapsedTime / 60);
		int seconds = (int)(elapsedTime % 60);

		TimerLabel.Text = $"{minutes:00}:{seconds:00}";
	}

	private void OnHealthChanged(float current, float max)
	{
		HealthBar.MaxValue = max;
		HealthBar.Value = current;
	}

	private void OnChargeUpdated(float chargePercent)
	{
		targetCharge = chargePercent;
	}

	private void OnBoostChanged(float value, bool isCoolingDown)
	{
		targetBoostValue = Mathf.Clamp(value, 0f, 1f) * 100f;

		BoostSlider.Modulate = isCoolingDown
			? new Color(1f, 0.5f, 0.5f)
			: new Color(0.5f, 1f, 1f);
	}

	private void OnScoreUpdated(int score)
	{
		ScoreLabel.Text = score.ToString();
	}

	public override void _ExitTree()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.ScoreChanged -= OnScoreUpdated;

		}
	}
	public void OnSpeedChanged(float speedRatio)
	{
		(SpeedMeter.Material as ShaderMaterial).SetShaderParameter("value", speedRatio);

	
	}
}