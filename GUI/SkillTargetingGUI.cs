

using Godot;

public partial class SkillTargetingGUI : Control
{
    [Signal]
    public delegate void SkillReadyEventHandler(Context context);
    private Board focusedBoard;
    private Context context;
    const int GUI_TILE_LAYER = 1;
    const int GUI_TILE_SET = 128;
    enum GUITile : int
    {
        BLUE = 0,
        RED = 1,
        GREEN = 2,
    }
    private void Clear()
    {
        focusedBoard?.ClearLayer(GUI_TILE_LAYER);
        context = null;
    }
    public void Cancel()
    {
        Clear();
    }
    public void Undo()
    {
        Clear();
    }
    public void LoadSkill(Skill skill, Pawn player)
    {
        context = new Context { SourceSkill = skill, SourcePawn = player };
        GUI.Main.SetDebugText($"Skill {skill.Name} loaded");
    }

    private bool CanTarget(Vector2I coords, Vector2 position)
    {
        int distance = context.SourcePawn?.Coords.Distance(coords) ?? -1;
        if (distance < context.SourceSkill.MinTargetRange)
        {
            return false;
        }
        if (distance > context.SourceSkill.MaxTargetRange)
        {
            return false;
        }
        if (context.SourceSkill.Target == Skill.TargetType.ENTITY)
        {
            if (context.SourcePawn.Board.GetFirstPawnAt(coords) is null)
            {
                return false;
            }
        }
        return true;
    }

    public void Hover(Board board, Vector2I coords, Vector2 position)
    {
        if (context is null)
        {
            return;
        }
        GUI.Main.SetDebugText($"Coords {coords}");
        board.ClearLayer(GUI_TILE_LAYER);
        if (coords.IsValid())
        {
            focusedBoard = board;
            board.SetCell(
                GUI_TILE_LAYER,
                coords,
                GUI_TILE_SET,
                atlasCoords: new Vector2I(0, 0),
                alternativeTile: (int)(CanTarget(coords, position) ? GUITile.GREEN : GUITile.RED)
            );
        }
    }
    public void Click(Board board, Vector2I coords, Vector2 position)
    {
        if (context is null)
        {
            return;
        }
        focusedBoard = board;
        if (CanTarget(coords, position))
        {
            switch (context.SourceSkill.Target)
            {
                case (Skill.TargetType.TILE):
                    context.CoordsTarget = coords;
                    break;
                case (Skill.TargetType.ENTITY):
                    context.PawnTarget = board.GetFirstPawnAt(coords);
                    break;
                default:
                    GD.PrintErr("TODO: Different target type");
                    return;
            }
            EmitSignal(SignalName.SkillReady, context);
            Clear();
        }
    }
}