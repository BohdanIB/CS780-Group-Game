using Godot;
using System;

public partial class StartMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("CenterContainer/VBoxContainer/PlayButton").Pressed += OnPlayPressed;
		GetNode<Button>("CenterContainer/VBoxContainer/OptionsButton").Pressed += OnOptionsPressed;
		GetNode<Button>("CenterContainer/VBoxContainer/QuitButton").Pressed += OnQuitPressed;
		
		  // Fade in	
		var fade = GetNode<ColorRect>("FadeRect");
		fade.Modulate = new Color(0, 0, 0, 1);

		var tween = CreateTween();
		tween.TweenProperty(fade, "modulate:a", 0.0f, 10.0f)
		 	.SetTrans(Tween.TransitionType.Cubic)
		 	.SetEase(Tween.EaseType.Out);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnPlayPressed(){
		GetTree().ChangeSceneToFile("res://Scenes/main.tscn");

	}

	private void OnOptionsPressed(){
		GetTree().ChangeSceneToFile("res://Scenes/options_menu.tscn");	

	}

	private void OnQuitPressed(){
		GetTree().Quit();

	}
}
