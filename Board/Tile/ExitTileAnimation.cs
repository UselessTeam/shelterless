using Godot;
using System;

public partial class ExitTileAnimation : AnimationPlayer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Play("exit");
    }
}
