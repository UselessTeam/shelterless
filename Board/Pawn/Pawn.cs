using Godot;
using System;
using System.Collections.Generic;

public partial class Pawn : Node2D
{
    private Dictionary<Type, Component> Components;

    public void Register(Component component)
    {
        Components.Add(component.GetType(), component);
    }

    public void Unregister(Component component)
    {
        Components.Remove(component.GetType());
    }

    public T Get<T>() where T : Component
    {
        Component component = Components.GetValueOrDefault(typeof(T));
        if (component is null)
        {
            return null;
        }
        return (T)component;
    }
}
