using Godot;
using System;

public partial class GameManager : Node
{
	public int Score = 0;
	public PackedScene GameOverScene=  GD.Load<PackedScene>("res://Scenes/GameOver.tscn");
	public static GameManager Instance { get; private set; }

	[Signal]
	public delegate void ScoreChangedEventHandler(int newScore);
	[Signal]
	public delegate void PlayerHealthChangedEventHandler(float max, float current);

	public override void _Ready()
	{
		Instance = this;
	}

	public void AddScore(int amount)
	{
		Score += amount;
		EmitSignal(SignalName.ScoreChanged, Score);
	}

	public void OnHealthUpdate(float maxHealth, float currentHealth)
	{
		GD.Print($"Health: {currentHealth}/{maxHealth}");
		EmitSignal(SignalName.PlayerHealthChanged, maxHealth, currentHealth);
		if (currentHealth <= 0)
		{
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToPacked, GameOverScene);

		}
	}
}
