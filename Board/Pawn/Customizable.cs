using Godot;
using System;

public partial class Customizable : AnimatedSprite2D
{

    [Export] public int Id;

    public int NumberOptions => SpriteFrames.GetFrameCount(Animation);
    public int CurrentOption => Frame;

    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        this.GetAncestor<Pawn>()?.Get<PlayerComponent>()?.RegisterCustomizable(this);
        if (CharacterSelection.ChosenOptions != null)
            SetOption(CharacterSelection.ChosenOptions[Id]);
    }

    public void SetOption(int option)
    {
        while (option < 0) option += NumberOptions;
        Frame = option % NumberOptions;
    }

}
