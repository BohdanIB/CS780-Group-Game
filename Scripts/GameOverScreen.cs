using Godot;
using System;

public partial class GameOverScreen : Control
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		GetNode<Button>("VBoxContainer/RetryButton").Pressed += OnRetryPressed;
		GetNode<Button>("VBoxContainer/MainMenuButton").Pressed += OnMainMenuPressed;
		
		Visible = false;
		//ZIndex = 999;
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void onGameOver()
	{
		GD.Print("GameOverScreen: onGameOver() called");
		Visible = true;
		//MoveToFront();
	}

	public void OnRetryPressed()
	{
		
		GetTree().ChangeSceneToFile("Scenes/main.tscn");
	}

	public void OnMainMenuPressed()
	{
		GetTree().ChangeSceneToFile("Scenes/start_menu.tscn");
	}
}
