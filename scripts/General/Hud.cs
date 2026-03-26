using Godot;
using System;

public partial class Hud : Control
{

	HSlider HealthBar;
	public override void _Ready()
	{
		HealthBar = GetNode<HSlider>("BottomLeftStatus/VBoxContainer/HealthBar");
		GameManager.Instance.PlayerHealthChanged += OnHealthChanged;
	}

	private void OnHealthChanged(float max, float current)
	{
		HealthBar.MaxValue = max;
		HealthBar.Value = current;
	}
}
