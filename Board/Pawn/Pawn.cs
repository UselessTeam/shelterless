using Godot;
using System;
using System.Collections.Generic;

public partial class Pawn : Node2D
{
    public Vector2I Coords { get; private set; } = global::Board.INVALID_COORDS;
    private Dictionary<Type, Component> Components = new Dictionary<Type, Component>();

    public override void _EnterTree()
    {
        base._EnterTree();
        Board = this.GetAncestor<Board>();
        if (Board is null)
        {
            GD.PrintErr($"Could not find Board pawn '{Name}'");
        }
        Board?.Register(this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Board?.Unregister(this);
        Board = null;
    }

    public Board Board { get; private set; }

    internal void Register(Component component)
    {
        Components.Add(component.GetType(), component);
    }

    internal void Unregister(Component component)
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

    internal void SetCoords(Vector2I coords)
    {
        // Should only be called by Board
        // Call 'MovePawn' in Board to move pawn
        Coords = coords;
    }
}
