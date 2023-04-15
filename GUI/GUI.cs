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

    public void MouseOnTile(TileMap map, Vector2I coords, Vector2 position, bool action)
    {
        //TODO: Dispatch to current GUI state
        SetDebugText($"Coords: {coords} / Action: {action}");
        if (!action)
        {
            TileHover(map, coords);
        }
        if (action)
        {
            TileClick(map, coords);
        }
    }

    private void TileHover(TileMap map, Vector2I coords)
    {
        map.ClearLayer(1);
        map.SetCell(1, coords, 100, atlasCoords: new Vector2I(0, 0));
    }


    private void TileClick(TileMap map, Vector2I coords)
    {
        map.ClearLayer(1);
    }

    public void SetDebugText(String text)
    {
        if (DebugText is not null)
        {
            DebugText.Text = text;
        }
    }
}
