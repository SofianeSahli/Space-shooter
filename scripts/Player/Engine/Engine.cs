using Godot;
using System;

public partial class Engine : Node2D
{
    [Export] public EngineData Stats;

    private AnimatedSprite2D Animator;
    
    public bool IsBoosting { get; set; } = false;
    public bool IsMoving { get; set; } = false;

    private string currentAnimation = "";

    public override void _Ready()
    {
        Animator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }


    public void UpdateAnimation(string AnimationName)
    {
    /*    if (string.IsNullOrEmpty(AnimationName))
        {
            Animator.Visible = false;
        }
        else
        {

            Animator.Visible = true;
            Animator.Play(AnimationName);
        }*/
    }

    public void ChangeAnimation(Vector2 velocity)
    {
        IsMoving = velocity.Length() > 5f;
    }
}