using Godot;
using System;
using System.Threading.Tasks;

public partial class GUI : CanvasLayer
{
    [Export]
    public string[] AvailableSkills;

    [Export]
    Label DebugText;

    [Signal]
    public delegate void FinishControlEventHandler();

    private Pawn controlledPawn;
    public async Task ControlPawn(Pawn pawn)
    {
        controlledPawn = pawn;
        if(_prepareSkill!= null)
            SelectSkill(_prepareSkill);
        Context context = (Context)(await ToSignal(SkillTargeting, SkillTargetingGUI.SignalName.SkillReady))[0];
        if (controlledPawn != context.SourcePawn)
        {
            GD.PrintErr("Wrong pawn in context");
            return;
        }
        controlledPawn = null;
        await context.Run();
        EmitSignal(SignalName.FinishControl);
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

    public SkillTargetingGUI SkillTargeting;

    public override void _Ready()
    {
        base._Ready();
        SkillTargeting = GetNode<SkillTargetingGUI>("SkillTargeting");
    }

    string _prepareSkill = null;
    
    public void SelectSkill(string skillName)
    {
        if (controlledPawn is null)
        {
            _prepareSkill = skillName;
            GD.Print("Will prepare");
            return;
        }
        Skill skill = skillName switch
        {
            "attack" => SkillList.SingleAttack,
            "push" => SkillList.Push,
            "wind-grenade" => SkillList.WindGrenade,
            "fire-grenade" => SkillList.FireGrenade,
            "move" => SkillList.Move,
            _ => null,
        };
        if (skill is null)
        {
            return;
        }
        SkillTargeting.LoadSkill(skill, controlledPawn);
    }

    public void MouseOnTile(Board board, Vector2I coords, Vector2 position, bool action)
    {
        if (controlledPawn is null)
        {
            return;
        }
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
