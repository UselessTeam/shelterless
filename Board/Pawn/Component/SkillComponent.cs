using Godot;
using System;
using System.Collections.Generic;

public partial class SkillComponent : Component
{
    [Export(PropertyHint.Range, "1,50,or_greater")]
    public int Damage = 20;

    [Export] 
    public bool DoubleAttack;

    [Export(PropertyHint.Range, "1,4")]
    public int PushStrength = 3;

    [Export(PropertyHint.Range, "0,2")]
    public int Weight;
    // Reduces the distance this unit is pushed
}