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

    public EffectRule<Context> Effect;
}