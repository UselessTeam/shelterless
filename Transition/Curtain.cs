using Godot;

public partial class Curtain : ColorRect
{
	[Export]
	public PackedScene NextScene;
	
	public override void _Ready(){
		OpenCurtain();
	}

	// Called when the node enters the scene tree for the first time.
	public void CloseCurtainAndChangeScene()
	{
		var curtainAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
		curtainAnimation.Play("close");
		curtainAnimation.AnimationFinished += _ =>
				GetTree().ChangeSceneToPacked(NextScene);
	}

	
	// Called when the node enters the scene tree for the first time.
	public void OpenCurtain()
	{
		var curtainAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
		curtainAnimation.Play("open");
	}
}
