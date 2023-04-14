using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerComponent : Component
{
    override protected IEnumerable<Type> GetDependencies()
    {
        yield return typeof(HealthComponent);
        yield return typeof(LocomotionComponent);
    }
}