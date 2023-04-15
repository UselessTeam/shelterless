using Godot;
using System;

public partial class Skill : Resource
{
    public enum TargetType
    {
        TILE,
        ENTITY,
        DIRECTION,
        RADIUS,
    }
    public enum Shape
    {
        STAR,
        ALL,
    }

    [Export]
    public string Name = "";

    [ExportGroup("Target")]
    [Export]
    public TargetType Target = TargetType.ENTITY;
    [Export]
    public Shape TargetShape = Shape.ALL;
    [Export]
    public int MinTargetRange = 0;
    [Export]
    public int MaxTargetRange = 0;
    [Export]
    public bool MustBeEmpty = false;

    [ExportGroup("Effect")]
    [Export]
    public bool Move = false;
    [Export]
    public int Damage = 10;
    [Export]
    public Shape EffectShape = Shape.ALL;
    [Export]
    public int MinEffectRange = 0;
    [Export]
    public int MaxEffectRange = 0;
    // Can be negative for pulling
    [Export(PropertyHint.Range, "-3,3")]
    public int Push = 0;
}