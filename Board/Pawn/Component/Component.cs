using Godot;
using System;
using System.Collections.Generic;

public partial class Component : Node
{
    virtual protected IEnumerable<Type> GetDependencies()
    {
        yield break;
    }
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

    public override void _Ready()
    {
        base._Ready();
        foreach (Type dependency in GetDependencies())
        {
            if (!Pawn.Has(dependency))
            {
                GD.PrintErr($"Missing dependency {dependency} for pawn {Name}");
            }
        }
    }
    public Pawn Pawn { get; private set; }
}
