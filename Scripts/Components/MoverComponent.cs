
using System.Collections.Generic;
using Godot;

public partial class MoverComponent : Node2D
{
	private const int START_PATH_INDEX = 0;

	// [Signal] public delegate void OnPathPointReachedEventHandler(); // todo
	[Signal] public delegate void OnPathCompletedEventHandler(); // todo

	[Export] public float Speed = 20f;
	[Export] public Node2D ParentNode;
	public bool CurrentlyMoving { get; private set; } = false;
	private Vector2[] _moverPath = [];
	private int _currentPathIndex = START_PATH_INDEX;

	public void Initialize(float speed, Node2D parent, bool start = false, Vector2[] moverPath = null)
	{
		Speed = speed;
		ParentNode = parent;
		if (start) {Start();} else {Stop();}
		if (moverPath != null) { SetMoverPath(moverPath); }
	}

	public override void _PhysicsProcess(double delta)
	{
		if (PathCompleted() || !CurrentlyMoving) return;

		if (!IsInstanceValid(ParentNode))
		{
			// Mover cannot move parent
			return;
		}

		var targetPosition = _moverPath[_currentPathIndex];
		var distanceToTarget = ParentNode.Position.DistanceTo(targetPosition);
		var totalMovement = Speed*delta;
		while (totalMovement >= distanceToTarget)
		{
			ParentNode.Position = targetPosition;
			_currentPathIndex++;
			if (PathCompleted())
			{
				EmitSignal(SignalName.OnPathCompleted);
				return;
			}
			totalMovement -= distanceToTarget;
			targetPosition = _moverPath[_currentPathIndex];
			distanceToTarget = ParentNode.Position.DistanceTo(targetPosition);
		}
		ParentNode.Position = ParentNode.Position.MoveToward(targetPosition, (float)totalMovement);
	}

	public void Start()
	{
		CurrentlyMoving = true;
	}
	public void Stop()
	{
		CurrentlyMoving = false;
	}

	public float GetPathLengthFromCurrentPosition()
	{
		if (PathCompleted())
		{
			return 0.0f;
		}

		float distance = GlobalPosition.DistanceTo(_moverPath[_currentPathIndex]);
		for (int i = _currentPathIndex+1; i < _moverPath.Length; i++)
		{
			distance += _moverPath[i-1].DistanceTo(_moverPath[i]);
		}
		// GD.Print($"MoverComponent for {Owner.Name} path length from current position: {distance}");
		return distance;
	}

	public bool PathCompleted()
	{
		return _moverPath == null || _currentPathIndex >= _moverPath.Length;
	}

	public Vector2[] GetMoverPath()
	{
		return _moverPath;
	}
	public void SetMoverPath(Vector2[] path)
	{
		_moverPath = path;
		_currentPathIndex = START_PATH_INDEX;
	}

}
