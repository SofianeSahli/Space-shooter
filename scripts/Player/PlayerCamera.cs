using Godot;
using System;


public partial class PlayerCamera : Camera2D
{
	private PlayerObject Player;

	public override void _Ready()
	{
		Enabled = true;
		PositionSmoothingEnabled = true;
		PositionSmoothingSpeed = 5f;
		MakeCurrent();

		Player = GetTree().GetFirstNodeInGroup("PlayerObject") as PlayerObject;

		if (Player == null)
		{
			GD.PrintErr("Player not found in group!");
			return;
		}

		GlobalPosition = Player.GlobalPosition;

		CallDeferred(nameof(SetupCameraLimits));
	}



	private void SetupCameraLimits()
	{
		LimitLeft = (int)GameManager.Instance.WorldBounds.Position.X;
		LimitTop = (int)GameManager.Instance.WorldBounds.Position.Y;
		LimitRight = (int)(GameManager.Instance.WorldBounds.Position.X + GameManager.Instance.WorldBounds.Size.X);
		LimitBottom = (int)(GameManager.Instance.WorldBounds.Position.Y + GameManager.Instance.WorldBounds.Size.Y);
	}
}

