using Godot;
using System;

public partial class Asteroids : Area2D
{
	[Export]
	int MaxSpeed = 200;
	[Export]
	int MinSpeed = 200;
	int Speed = 0;
	[Export]
	float RotationMaxAngle = 0.5f;
	[Export]
	float RotationMinAngle = 0.1f;
	float RotationAngle = 0;
	RandomNumberGenerator Randomizer = new RandomNumberGenerator();
	float MouvementX;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var Width = GetViewport().GetVisibleRect().Size[0];
		var RandomLocationX = Randomizer.RandiRange(0, (int)Width - 200);
		var RandomLocationY = Randomizer.RandiRange(-150, -50);
		Position = new Vector2((float)RandomLocationX, (float)RandomLocationY);
		Speed = Randomizer.RandiRange(MinSpeed, MaxSpeed);
		RotationAngle = Randomizer.RandfRange(RotationMinAngle, RotationAngle);
		MouvementX = Randomizer.RandfRange(-0.8f, 0.8f);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += new Vector2(MouvementX, 1f) * Speed * (float)delta;
		Rotate(RotationAngle);
	}

	public void BodyEntred(Node2D body)
	{
	}
}
