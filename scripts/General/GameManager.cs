using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class GameManager : Node
{

	public int Score = 0;
	public Godot.Collections.Dictionary<string, PackedScene> Scenes = new Godot.Collections.Dictionary<string, PackedScene>()
	{
		{ "GameOver", GD.Load<PackedScene>("res://Scenes/UI/GameOver.tscn") },
		{ "SurvivalMode" , GD.Load<PackedScene>("res://Scenes/LevelOne.tscn") }
	};
	public static GameManager Instance { get; private set; }

	[Signal]
	public delegate void ScoreChangedEventHandler(int newScore);

	public override void _Ready()
	{
		Instance = this;
	}

	public void AddScore(int amount)
	{
		Score += amount;
		EmitSignal(SignalName.ScoreChanged, Score);
	}

	public void ChangeScene(string ScenePath)
	{
		if (!Scenes.ContainsKey(ScenePath))
		{
			GD.PrintErr($"Scene '{ScenePath}' not found in Scenes dictionary!");
			return;
		}
		if(ScenePath == "GameOver")
		{
			Score = 0;
		}
		CallDeferred(nameof(_ChangeSceneDeferred), ScenePath);

	}
	private void _ChangeSceneDeferred(string sceneKey)
	{
		GetTree().ChangeSceneToPacked(Scenes[sceneKey]);
	}
}
