using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerComponent : Component
{
    public static PlayerComponent Main;
    public override void _EnterTree()
    {
        base._EnterTree();
        Main = this;

    }
    override protected IEnumerable<Type> GetDependencies()
    {
        yield return typeof(HealthComponent);
        yield return typeof(LocomotionComponent);
    }
}