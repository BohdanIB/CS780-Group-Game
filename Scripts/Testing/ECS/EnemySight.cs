
using CS780GroupProject.Scripts.Utils;
using Godot;

public partial class EnemySight : Node2D
{
	[Export] private Enemy _enemy;
	[Export] private Turret _turret;

	[Export] private Godot.Collections.Array<PackedScene> scenes;

	public override void _Ready()
	{
		// GD.Print($"Types: ");
		// foreach (var t in types)
		// {
		// 	GD.Print($"{t}");
		// }
		// GD.Print($"Turret script: {_turret.GetScript()}");

		// GD.Print($"Resources: ");
		// foreach (var r in resources)
		// {
		// 	GD.Print($"{r.ResourcePath}");
		// }
		// // GD.Print($"Turret script: {_turret.GetScript()}");
		// GD.Print($"Turret resource: {_turret.SceneFilePath}");

		foreach (var s in scenes)
		{
			if (SceneType.SameType(_turret, s))
			{
				GD.Print($"EnemySight contains type of {_turret.Name} and should do stuff because of that.");
			}
		}

	}

	public override void _Process(double delta)
	{
		// const float MOVEMENT_SPEED = 200f;
		// if (IsInstanceValid(_enemy))
		// {
		// 	_enemy.Position += new Vector2(-MOVEMENT_SPEED * (float)delta, 0);
		// }
	}
}
