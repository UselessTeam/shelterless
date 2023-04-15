using Godot;
using System;

public partial class Component : Node
{
    public override void _EnterTree()
    {
        base._EnterTree();
        Pawn = FindPawnParent();
        Pawn?.Register(this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Pawn?.Unregister(this);
        Pawn = null;
    }

    public Pawn Pawn { get; private set; }
    private Pawn FindPawnParent()
    {
        Node parent = GetParent();
        while (parent is not null)
        {
            if (parent is Pawn pawn)
            {
                return pawn;
            }
            parent = parent.GetParent();
        }
        return null;
    }
}
