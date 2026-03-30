using Godot;
using System;

public partial class Canon : Marker2D
{
	public PackedScene Laser;
	private bool CanShoot = true;
	Timer ShootTimer;
	public override void _Ready()
	{
		Laser = GD.Load<PackedScene>("res://Scenes/Bullets/BulletRed.tscn");
		ShootTimer = GetNode<Timer>("BaseShotTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void OnBaseShoot(float chargePercent)
	{
		if (!CanShoot) return;

		var projectile = Laser.Instantiate<Bullet>();
		float damage = Mathf.Lerp(10f, 100f, chargePercent);
		float speed = Mathf.Lerp(1800f, 3600f, chargePercent);
		float scale = Mathf.Lerp(0.5f, 2.0f, chargePercent);
		projectile.ImpactDamage = damage;
		projectile.Speed = speed;
		projectile.DesiredScale = Vector2.One * scale;
		projectile.GlobalPosition = GlobalPosition;

		projectile.GlobalPosition = GetParent<PlayerObject>().GlobalPosition;
		projectile.Rotation = GetParent<PlayerObject>().Rotation;
		GetTree().CurrentScene.AddChild(projectile);

		ShootTimer.Start();
	}

	public void CansShootAgain()
	{
		CanShoot = true;
	}
}
