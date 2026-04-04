using Godot;
using System;

public partial class QuitButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += _on_QuitButton_pressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_QuitButton_pressed()
	{
		// Free all placed turrets
		foreach (Node turret in GetTree().GetNodesInGroup("placed_turrets"))
			turret.QueueFree();

		GD.Print(GetPath()); 
		GetTree().ChangeSceneToFile("res://Scenes/start_menu.tscn");
	}
}
