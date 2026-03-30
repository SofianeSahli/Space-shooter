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
	private AnimatedSprite2D AnimatedSprite;

	private CollisionPolygon2D CollisionPolygon;
	public override void _Ready()
	{
		base._Ready();
		PlayerSprite = GetNode<Sprite2D>("Sprite2D");
		HealthIndicator healthIndicator = GetNode<HealthIndicator>("HealthIndicator");
		healthIndicator.PlayerTookDamage += HealthUpdated;
		healthIndicator.ShouldDie += PlayDeathAnimation;
		AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimatedSprite.AnimationFinished += OnAnimationFinished;
		CollisionPolygon = GetNode<CollisionPolygon2D>("CollisionPolygon2D");
	}

	public void HealthUpdated(float current, float max)
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

	public void PlayDeathAnimation()
	{
		PlayerSprite.Visible = false;
		AnimatedSprite.Visible = true;
		AnimatedSprite.Play("Explosion");
		CollisionPolygon.SetDeferred("disabled", true); ;
	}
	public void OnAnimationFinished()
	{
		GameManager.Instance.ChangeScene("GameOver");

	}
}
