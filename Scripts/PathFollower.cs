using Godot;
using System;
using System.Collections.Generic;

public partial class PathFollower : RigidBody2D
{
	private const float DISTANCE_THRESHOLD = 0.01f;
	[Export] public float movementSpeed;

	private List<Vector2> path;
	private int currentPathIndex;

    public override void _PhysicsProcess(double delta)
    {
        if (path == null) return;
		if (Position.DistanceTo(path[currentPathIndex]) < DISTANCE_THRESHOLD)
		{
			currentPathIndex++;
			if (currentPathIndex >= path.Count) path = null;
			return;
		}

		Position = Position.MoveToward(path[currentPathIndex], (float) delta * movementSpeed);
    }

	public void SetPath(List<Vector2> newPath)
	{
		path = newPath;
		currentPathIndex = 0;
	}

}
