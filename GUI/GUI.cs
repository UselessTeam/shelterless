using Godot;
using System;

public partial class GUI : CanvasLayer
{
    [Export]
    Label DebugText;

    public const int GUI_TILE_LAYER = 1;
    public const int GUI_TILE_SET = 128;
    enum GUITile : int
    {
        BLUE = 0,
        RED = 1,
        GREEN = 2,
    }

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

    public void MouseOnTile(Board board, Vector2I coords, Vector2 position, bool action)
    {
        //TODO: Dispatch to current GUI state
        SetDebugText($"Coords: {coords} ({coords.Magnitude()}) / Action: {action}");
        if (!action)
        {
            TileHover(board, coords);
        }
        if (action)
        {
            TileClick(board, coords);
        }
    }

    private void TileHover(Board board, Vector2I coords)
    {
        board.ClearLayer(GUI_TILE_LAYER);
        if (coords.IsValid())
        {
            board.SetCell(
                GUI_TILE_LAYER,
                coords,
                GUI_TILE_SET,
                atlasCoords: new Vector2I(0, 0),
                alternativeTile: (int)(board.Player.Coords.Distance(coords) switch
                {
                    0 => GUITile.BLUE,
                    1 => GUITile.GREEN,
                    _ => GUITile.RED,
                })
            );
        }
    }


    private void TileClick(Board board, Vector2I coords)
    {
        board.ClearLayer(GUI_TILE_LAYER);
        board.Player.Get<LocomotionComponent>().RunToCoords(coords);
        board.Player.SetCoords(coords);
    }

    public void SetDebugText(String text)
    {
        if (DebugText is not null)
        {
            DebugText.Text = text;
        }
    }
}
