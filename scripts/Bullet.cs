using Godot;
using System;

public partial class Bullet : Area2D
{
	[Export]
	float Speed;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite2D Sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (Sprite != null)
		{
			Tween Tween = CreateTween();
			Tween.TweenProperty(Sprite, "scale", new Vector2(1, 1.5f), 0.2).From(new Vector2(0,0));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += Vector2.Up * Speed * (float)delta;
	}
	public void BodyEntred(Node2D body)
	{
	}

}
