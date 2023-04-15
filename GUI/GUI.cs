using Godot;
using System;

public partial class GUI : CanvasLayer
{
	[Export]
	Label DebugText;

	public static GUI Main
	{
		get
		{
			if (main is null && !printedWarning)
			{
				GD.PrintErr("No GUI has been set");
				printedWarning = true;
			}
			return main;
		}
		private set
		{
			main = value;
		}
	}
	private static bool printedWarning = false;
	private static GUI main;

	public override void _EnterTree()
	{
		base._EnterTree();
		Main = this;
	}

	public void MouseOnTile(TileMap map, Vector2I coords, Vector2 position, InputEventMouse inputEvent)
	{
		//TODO: Dispatch to current GUI state
		SetDebugText($"Coords: {coords}");

	}

	public void SetDebugText(String text)
	{
		if (DebugText is not null)
		{
			DebugText.Text = text;
		}
	}
}
