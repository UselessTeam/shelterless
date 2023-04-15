using Godot;
using System;

public partial class Component : Node
{
    public override void _EnterTree()
    {
        base._EnterTree();
        Pawn = this.GetAncestor<Pawn>();
        Pawn?.Register(this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Pawn?.Unregister(this);
        Pawn = null;
    }

    public Pawn Pawn { get; private set; }
}
