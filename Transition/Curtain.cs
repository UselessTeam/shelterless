using Godot;

public partial class Curtain : ColorRect
{
    [Export(PropertyHint.File, "*.tscn")]
    public string NextScene;
    AnimationPlayer CurtainAnimation;
    bool ShouldReload;
    bool Working = false;

    public override void _EnterTree()
    {
        OpenCurtain();
        CurtainAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
        CurtainAnimation.AnimationFinished += AnimationFinished;
    }

    public void AnimationFinished(StringName name)
    {
        if (Working)
        {
            if (ShouldReload)
            {
                GetTree().ReloadCurrentScene();
            }
            else
            {
                GetTree().ChangeSceneToFile(NextScene);
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public void CloseCurtainAndChangeScene()
    {
        if (NextScene == "quit")
        {
            GetTree().Quit();
            return;
        }
        Working = true;
        ShouldReload = false;
        CurtainAnimation.Play("close");
    }

    public void CloseCurtainAndRestartScene()
    {
        Working = true;
        ShouldReload = true;
        CurtainAnimation.Play("close");
    }

    // Called when the node enters the scene tree for the first time.
    public void OpenCurtain()
    {
        var curtainAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
        curtainAnimation.Play("open");
    }
}
