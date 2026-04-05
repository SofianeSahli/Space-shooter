using Godot;
using System;

public partial class LaserImpact : GpuParticles2D
{
	public override void _Ready()
	{
		GD.Print("Spawned");
		Restart();
		Emitting = true;
		OneShot = true;
		GetTree().CreateTimer(Lifetime).Timeout += QueueFree;
	}

	public override void _Process(double delta)
	{
	}
}
