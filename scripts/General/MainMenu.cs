using Godot;
using System;

public partial class MainMenu : VBoxContainer
{

	Button StoryMode, SurvivalMode, Params, Credits, Exit;
	public override void _Ready()
	{
		StoryMode = GetNode<Button>("StoryMode");
		SurvivalMode = GetNode<Button>("SurvivalMode");
		Params = GetNode<Button>("Params");
		Credits = GetNode<Button>("Credits");
		Exit = GetNode<Button>("Exit");
		SurvivalMode.GrabFocus();


		SurvivalMode.Pressed += () =>
		{
			GameManager.Instance.ChangeScene("SurvivalMode");

		};
	}

}
