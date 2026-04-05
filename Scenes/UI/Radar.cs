using Godot;
using System.Collections.Generic;

public partial class Radar : Panel
{
	[Export] public Node2D Player;
	[Export] public float RadarRadius = 150f;
	[Export] public float WorldRange = 2000f;

	[Export] public float SweepSpeed = 2.5f;
	[Export] public float SweepWidth = 1f;

	[Export] public float DetectionDuration = 1.0f;
	ColorRect Background;
	private float SweepAngle = 0f;

	private Control Compass;
	private ShaderMaterial RadarMaterial;

	private List<Node2D> targets = new();
	private Dictionary<Node2D, float> detectionTimers = new();

	public override void _Ready()
	{
		if (Player == null)
			Player = GetTree().GetFirstNodeInGroup("PlayerObject") as Node2D;

		Compass = GetNodeOrNull<Control>("Compass");
		Background = GetNode<ColorRect>("Radar");
		RadarMaterial = Background.Material as ShaderMaterial;

		foreach (Node node in GetTree().GetNodesInGroup("RadarDetectable"))
		{
			if (node is Node2D n)
				targets.Add(n);
		}
	}

	public override void _Process(double delta)
	{
		if (Player == null) return;

		float dt = (float)delta;

		// Compass
		if (Compass != null)
		{
			Compass.PivotOffset = Compass.Size / 2f;
			Compass.Rotation = -Player.GlobalRotation;
		}

		// Sweep animation
		SweepAngle += SweepSpeed * dt;
		SweepAngle = Mathf.Wrap(SweepAngle, 0, Mathf.Tau);

		float finalAngle = SweepAngle - Player.GlobalRotation;

		// 🔥 Send to shader
		if (RadarMaterial != null)
		{
			RadarMaterial.SetShaderParameter("sweep_angle", finalAngle);
			RadarMaterial.SetShaderParameter("sweep_width", SweepWidth);
			RadarMaterial.SetShaderParameter("delta", delta);
		}

		// Detection timers
		var keys = new List<Node2D>(detectionTimers.Keys);

		foreach (var key in keys)
		{
			detectionTimers[key] -= dt;

			if (detectionTimers[key] <= 0)
				detectionTimers.Remove(key);
		}

		QueueRedraw();

	}

	private void DrawTriangle(Vector2 position, float rotation, float size, Color color)
	{
		Vector2[] points = new Vector2[]
		{
		new Vector2(0, -size),
		new Vector2(size * 0.6f, size),
		new Vector2(-size * 0.6f, size)
		};

		for (int i = 0; i < points.Length; i++)
			points[i] = points[i].Rotated(rotation) + position;

		DrawPolygon(points, new Color[] { color });
	}
	public override void _Draw()
	{
		if (Player == null) return;

		Vector2 center = Size / 2f;
		float radius = Mathf.Min(Size.X, Size.Y) / 2f;

		foreach (var target in targets)
		{
			if (target == null || !IsInstanceValid(target)) continue;

			Vector2 offset = target.GlobalPosition - Player.GlobalPosition;
			float distance = offset.Length();

			if (distance > WorldRange) continue;

			Vector2 dir = offset.Normalized();
			float scaledDist = Mathf.Min(distance, WorldRange);
			float safeRadius = radius * 0.9f; // 🔥 shrink inside circle
			Vector2 radarPos = dir * (scaledDist / WorldRange) * safeRadius;
			radarPos = radarPos.Rotated(-Player.GlobalRotation);

			Vector2 finalPos = center + radarPos;
			float relativeRotation = target.GlobalRotation - Player.GlobalRotation;

			float angleToTarget = offset.Angle();
			float diff = Mathf.AngleDifference(SweepAngle, angleToTarget);

			bool detectedNow = Mathf.Abs(diff) < 0.2f;


			if (detectedNow)
				detectionTimers[target] = DetectionDuration;

			bool detected = detectionTimers.ContainsKey(target);

			float fade = 1f - (distance / WorldRange);
			fade = Mathf.Clamp(fade, 0.3f, 1f);

			Color baseColor = detected ? Colors.Yellow : Colors.Red;
			if (detected)
				baseColor = new Color(1f, 0.9f, 0.2f); // gold
			else
				baseColor = new Color(1f, 0.3f, 0.3f); // soft red
			DrawTriangle(finalPos, relativeRotation, 6f, baseColor);

			if (detected)
			{
				float t = detectionTimers[target] / DetectionDuration;

				float pulseSize = Mathf.Lerp(12f, 4f, 1f - t);
				Color pulseColor = new Color(1, 1, 0, t);

				DrawArc(finalPos, pulseSize, 0, Mathf.Tau, 16, pulseColor, 2);
			}
		}

		DrawTriangle(center, 0f, 8f, Colors.Green);
	}
}
