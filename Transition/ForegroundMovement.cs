using Godot;
using System;

public partial class ForegroundMovement : Node
{
	[Export] bool UseMovingForeground = false;
	[Export] Control MovingForeground;
	[Export] Control FixedForeground;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MovingForeground.Visible = UseMovingForeground;
		FixedForeground.Visible = ! UseMovingForeground;
	}
}
