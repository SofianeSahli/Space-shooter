using Godot;
using System;
[GlobalClass]
public partial class EngineData : Resource
{
    [Export]
    public float Speed;
    [Export] public float Acceleration;
    [Export] public float Friction ;
    [Export] public float RotationSpeed; 
    [Export] public float BoostMultiplier  = 2.5f;
    [Export] public float BoostDuration  = 0.5f;
    [Export] public float BoostCooldown  = 1.5f;
    [Export]
    public float BoostAccelerationModifier  = 1.5f;
    public bool IsBoosting  = false;
    public float BoostTimer  = 0f;
    public float CooldownTimer  = 0f;
    public float NormalizedValue  = 0f;
    public bool IsCooling  = false;
    public float BoostEnergy  = 2f;        // current energy
    public float MaxBoostEnergy  = 2f;     // max energy

    public float RechargeRate  = 1f;       // energy per second
    public float DepletionRate  = 1f;      // energy drain per second
    public float RechargeDelay  = 0.5f;
    public float RechargeDelayTimer  = 0f;

    public static string MOVING_ANIMATION = "default";
    public static string BOOST_ANIMATION = "Boost";

}
