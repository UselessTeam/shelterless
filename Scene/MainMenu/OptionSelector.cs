using Godot;
using System;

public partial class OptionSelector : HBoxContainer
{
    [Export] public int Id;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.GetAncestor<CharacterSelection>().RegisterOptionSelector(this);
        GetNode<Button>("Left").ButtonDown += () => Left?.Invoke();
        GetNode<Button>("Right").ButtonDown += () => Right?.Invoke();
    }

    public event Action Left;
    public event Action Right;
}
