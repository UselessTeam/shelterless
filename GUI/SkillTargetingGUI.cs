

using System;
using Godot;

public partial class SkillTargetingGUI : Control
{
    [Signal]
    public delegate void SkillReadyEventHandler(Context context);

    //The button passed as an arg will no be cleared 
    event Action<Button> ClearButtons;

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

    public override void _Ready()
    {
        base._Ready();

        var gui = this.GetAncestor<GUI>();
        foreach (var skill in gui.AvailableSkills)
        {
            var buttonNode = new Button()
            {
                Text = skill,
                ToggleMode = true,
            };
            ClearButtons += (button)=> {
                if (buttonNode != button) buttonNode.ButtonPressed = false;
            };
            AddChild(buttonNode);
            buttonNode.Connect(Button.SignalName.Toggled,
                Callable.From<bool>((isPressed) =>
                {
                    if(isPressed){
                        ClearButtons?.Invoke(buttonNode);
                        gui.SelectSkill(skill.ToLower());
                    }
                    else
                        gui.LooseFocus();
                }
            ));
        }
    }
    
    private void Clear()
    {
        focusedBoard?.ClearLayer(GUI_TILE_LAYER);
        context = null;
        ClearButtons(null);
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
        context = new Context { SourceSkill = skill, SourcePawn = player, Origin = player.Coords };
        GUI.Main.SetDebugText($"Skill {skill.Name} loaded");
    }

    private bool CanTarget(Vector2I coords, Vector2 position)
    {
        int distance = context?.Origin.Distance(coords) ?? -1;
        if (context.SourceSkill.Target == Skill.TargetType.DIRECTION)
        {
            return (coords - context.Origin).ToDirection() != VectorUtils.Direction.NONE;
        }
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
                case (Skill.TargetType.DIRECTION):
                    context.DirectionTarget = (coords - context.Origin).ToDirection();
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