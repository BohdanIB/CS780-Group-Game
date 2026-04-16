
using Godot;

public partial class EnemySight : Node2D
{
	[Export] private Enemy _enemy;
	[Export] private Turret _turret;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}


	/*
	Sight testing:

	Case 1: Turret sees enemy at specific range. Enemy sees turret at specific range.
	Case 2: Turrets do not see each other. Enemies do not see each other
	Case 3: 
	*/
}
