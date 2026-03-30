using Godot;

public partial class GameOver : Node
{
	Button Shop, TryAgain, Menu;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Shop = GetNode<Button>("Shop");
		TryAgain = GetNode<Button>("TryAgain");
		Menu = GetNode<Button>("Menu");
		TryAgain.GrabFocus();
		TryAgain.Pressed += () =>
	{
		GetTree().ChangeSceneToFile("res://Scenes/LevelOne.tscn");
	};
	}


}
