using Godot;
using System;

public partial class PlayerObject : CharacterBody2D
{
	[Export]
	public Texture2D Damaged;
	[Export]
	public Texture2D SlightDamage;
	[Export]
	public Texture2D VeryDamaged;
	[Export]
	public Texture2D FullHealth;
	public Sprite2D PlayerSprite;

	public override void _Ready()
	{
		base._Ready();
		GameManager.Instance.PlayerHealthChanged += HealthUpdated;
		PlayerSprite = GetNode<Sprite2D>("Sprite2D");
	}

	public void HealthUpdated(float max, float current)
	{
		float healthPercent = current / max;
		if (healthPercent > 0.75f)
		{
			PlayerSprite.Texture = FullHealth;
		}
		else if (healthPercent > 0.5f)
		{
			PlayerSprite.Texture = SlightDamage;
		}
		else if (healthPercent > 0.25f)
		{
			PlayerSprite.Texture = Damaged;
		}
		else
		{
			PlayerSprite.Texture = VeryDamaged;
		}
	}
}
