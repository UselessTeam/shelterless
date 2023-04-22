using Godot;
using System;

public partial class TransitionAnimation : AnimationPlayer
{
	[Export] string playOnLoad = "walk";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Play(playOnLoad);
	}
}
