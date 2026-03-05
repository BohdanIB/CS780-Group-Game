using Godot;
using System.Collections.Generic;

public partial class PathFollower : Area2D
{

	private const float DISTANCE_THRESHOLD = 0.01f;

	[Export] private float _health = 100.0f;
	[Export] public float movementSpeed = 50.0f;

	private List<Vector2> path;
	private int currentPathIndex;

    public override void _PhysicsProcess(double delta)
    {
		if (path == null) return;
		if (Position.DistanceTo(path[currentPathIndex]) < DISTANCE_THRESHOLD)
		{
			currentPathIndex++;
			if (currentPathIndex >= path.Count)
			{
				path = null;
				GD.Print($"PathFollower {Name} reached end of path.");
			}
			return;
		}

		Position = Position.MoveToward(path[currentPathIndex], (float) delta * movementSpeed);
	}

	public void SetPath(List<Vector2> newPath)
	{
		path = newPath;
		currentPathIndex = 0;
	}

	public void ChangeHealth(float healthChangeValue)
	{
		GD.Print($"HealthChange for PathFollower {Name} - CurrentHealth {_health} -> NewHealth {_health - healthChangeValue}");
		_health -= healthChangeValue;
		if (_health <= 0.0f)
		{
			GD.Print($"PathFollower {Name} died.");
			QueueFree(); // TODO
		}
	}

}
