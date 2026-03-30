using Godot;
using System;

public partial class Hud : Control
{

	private HSlider HealthBar;
	private HSlider ChargeBar;
	private Label ScoreLabel;
	private Label TimerLabel;
	private float elapsedTime = 0f;

	private float targetCharge = 0f;
	public override void _Ready()
	{
		HealthBar = GetNode<HSlider>("BottomLeftStatus/VBoxContainer/HealthBar");
		ChargeBar = GetNode<HSlider>("BottomCenterStatus/VBoxContainer/CannonCharge");
		ScoreLabel = GetNode<Label>("MarginContainer/HBoxContainer/ScoreLabel");
		TimerLabel = GetNode<Label>("MarginContainer/HBoxContainer2/TimerLabel");
		GameManager.Instance.ScoreChanged += OnScoreUpdated;
		var players = GetTree().GetNodesInGroup("PlayerObject");

		if (players.Count > 0)
		{
			var player = players[0];
			Controls PlayerControl = player.GetNode<Controls>("ControlNode");
			HealthIndicator healthIndicator = player.GetNodeOrNull<HealthIndicator>("HealthIndicator");
			if (healthIndicator != null)
			{
				healthIndicator.PlayerTookDamage += OnHealthChanged;
				OnHealthChanged(healthIndicator.CurrentHealth, healthIndicator.MaxHealth);
			}
			PlayerControl.ChargeUpdated += OnChargeUpdated;
		}
	}
	public override void _Process(double delta)
	{
		ChargeBar.Value = Mathf.Lerp(
			(float)ChargeBar.Value,
			targetCharge,
			75f * (float)delta
		);
		elapsedTime += (float)delta;

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

	public void OnScoreUpdated(int Score)
	{
		ScoreLabel.Text = Score.ToString();
	}
	public override void _ExitTree()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.ScoreChanged -= OnScoreUpdated;
		}
	}
}
