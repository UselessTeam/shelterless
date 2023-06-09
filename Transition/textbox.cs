using Godot;
using System.Collections.Generic;

public partial class textbox : CanvasLayer
{
    [Export] Label _textBox;

    [Export(PropertyHint.MultilineText)] string[] TextList = new[] { "" };

    [Export] bool ChangeSceneAfterDialog = true;

    int currentTextIndex = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _textBox.Text = TextList[0];
        currentTextIndex++;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("TextSkip"))
        {
            GetViewport().SetInputAsHandled();
			if(currentTextIndex < TextList.Length){
				_textBox.Text = TextList[currentTextIndex];
				currentTextIndex++;
			} else if(ChangeSceneAfterDialog) {
				GetNode<Curtain>("Curtain").CloseCurtainAndChangeScene();
			} else {
                QueueFree();
            }
        }
    }
}