using Godot;
using System;

public partial class ConstructionButton : Button
{
	[Signal] public delegate void ConstructionButtonPressedEventHandler(ConstructionInformation constructionInformation);
	[Export] public ConstructionInformation constructionInformation;

    public override void _Ready()
    {
        base._Ready();
		Pressed += OnButtonPressed;
    }


	public void OnButtonPressed()
	{
		EmitSignal(SignalName.ConstructionButtonPressed, constructionInformation);
	}
}
