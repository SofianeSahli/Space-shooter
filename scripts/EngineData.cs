using Godot;
using System;
[GlobalClass]
public partial class EngineData : Resource
{
    [Export]
    public float Speed ;
    [Export] public float Acceleration ;
    [Export] public float Friction ;

}
