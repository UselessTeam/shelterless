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

    public SkillTargetingGUI SkillTargeting;

    public override void _Ready()
    {
        base._Ready();
        SkillTargeting = GetNode<SkillTargetingGUI>("SkillTargeting");
    }

    public void SelectSkill(string skillName)
    {
        Skill skill = skillName switch
        {
            "attack" => SkillList.Attack,
            "move" => SkillList.Move,
            _ => null,
        };
        SkillTargeting.LoadSkill(skill, PlayerComponent.Main.Pawn);
    }

    public void MouseOnTile(Board board, Vector2I coords, Vector2 position, bool action)
    {
        //TODO: Dispatch to current GUI state
        if (!action)
        {
            SkillTargeting.Hover(board, coords, position);
        }
        if (action)
        {
            SkillTargeting.Click(board, coords, position);
        }
    }

    public void LooseFocus()
    {
        SkillTargeting.Cancel();
    }

    public void SetDebugText(string text)
    {
        if (DebugText is not null)
        {
            DebugText.Text = text;
        }
    }
}
