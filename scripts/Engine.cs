using Godot;
using System;

public partial class Engine : Node2D
{
    [Export]
    public EngineData Stats;
    AnimatedSprite2D Animator;
    public override void _Ready()
    {
        Animator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    }

    public void ChangeAnimation(Vector2 Velocity)
    {
        float speed = Velocity.Length();

        if (speed > 5f)
        {
            Animator.Play("Boost");
        }
        else
        {
            Animator.Play("default");

        }
    }
}
