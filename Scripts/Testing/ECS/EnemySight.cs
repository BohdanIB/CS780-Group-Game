
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
}
