using Godot;

public partial class Curtain : ColorRect
{
	[Export(PropertyHint.File, "*.tscn")]
	public string NextScene;
	
	public override void _EnterTree(){
		OpenCurtain();
	}

	// Called when the node enters the scene tree for the first time.
	public void CloseCurtainAndChangeScene()
	{
		var curtainAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
		curtainAnimation.Play("close");
		curtainAnimation.AnimationFinished += _ =>
				GetTree().ChangeSceneToFile(NextScene);
	}

	
	// Called when the node enters the scene tree for the first time.
	public void OpenCurtain()
	{
		var curtainAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
		curtainAnimation.Play("open");
	}
}
