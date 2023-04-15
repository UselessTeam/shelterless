using Godot;
using System;
using System.Collections.Generic;

public partial class SkillComponent : Component
{
    [Export(PropertyHint.Range, "1,50,or_greater")]
    public int Damage = 20;

    [Export]
    public Skill[] Skills;
}