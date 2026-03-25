using Godot;
using System;
using System.Collections.Generic;

public partial class Level : Node2D
{
    [Export]
    PackedScene[] AsteroidBluePrint = [
        GD.Load<PackedScene>("res://Scenes/Ennemies/AsteroidSmall.tscn"),
        GD.Load<PackedScene>("res://Scenes/Ennemies/AsteroidMedium.tscn"),
        GD.Load<PackedScene>("res://Scenes/Ennemies/AsteroidLarge.tscn")
     ];
    [Export]
    private Node2D AsteroidManager;

    public override void _Ready()
    {
        AsteroidManager = GetNode<Node2D>("AsteroidManager");
        SelectStarsParentAndLayout("Foreground/RedStars");
        SelectStarsParentAndLayout("Foreground/GreenStars");
    }

    public void TimerTimeOut()
    {
        RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
        var random = randomNumberGenerator.RandiRange(0, 2);
        var Asteroid = AsteroidBluePrint[random].Instantiate();
        AsteroidManager.AddChild(Asteroid);
    }
    public void OnShootSignal(Vector2 Position, PackedScene Ammo)
    {
        Node2D SingleShot = (Node2D)Ammo.Instantiate();
        SingleShot.Position = Position;
        AddChild(SingleShot);
    }

    private void SelectStarsParentAndLayout(string ParentName)
    {
        var StarsLayer = GetNodeOrNull<Node2D>(ParentName);

        if (StarsLayer != null)
        {
            var childs = StarsLayer.GetChildren();
            LayoutStars(childs);
        }
    }

    private void LayoutStars(Godot.Collections.Array<Node> Stars)
    {
        if (Stars != null && Stars.Count > 0)
        {
            RandomNumberGenerator Randomizer = new RandomNumberGenerator();
            Randomizer.Randomize();
            Vector2 ViewSize = GetViewport().GetVisibleRect().Size;

            foreach (Node star in Stars)
            {
                if (star is Node2D starNode)
                {
                    float x = Randomizer.RandfRange(0, ViewSize.X);
                    float y = Randomizer.RandfRange(0, ViewSize.Y);

                    starNode.Position = new Vector2(x, y);
                    float scale = Randomizer.RandfRange(0.4f, 1f);
                    starNode.Scale = new Vector2(scale, scale);
                    var anim = starNode.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
                    if (anim != null)
                    {
                        anim.SpeedScale = Randomizer.RandfRange(0.5f, 1.2f);
                    }
                }
            }
        }
    }
}
