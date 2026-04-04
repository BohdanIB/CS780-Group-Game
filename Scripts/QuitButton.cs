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

	//debug method to print the scene tree. Can be deleted later.
	private void PrintTree(Node node, string indent = "")
	{
		GD.Print(indent + node.Name);

		foreach (Node child in node.GetChildren())
			PrintTree(child, indent + "  ");
	}


	private void _on_QuitButton_pressed()
	{

		var dialog = GetNode<ConfirmationDialog>("../../../QuitConfirmDialog");
		//GD.Print("Dialog found: ", dialog);
		//GD.Print("QuitButton path: ", GetPath());
		//PrintTree(GetTree().Root);


		dialog.PopupCentered();

	}

	private void _on_quit_confirm_dialog_confirmed()
	{
		foreach (Node turret in GetTree().GetNodesInGroup("placed_turrets"))
			turret.QueueFree();

		GetTree().ChangeSceneToFile("res://Scenes/start_menu.tscn");
	}

}
