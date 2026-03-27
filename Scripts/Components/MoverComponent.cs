
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
	private List<Vector2> _moverPath = new();
	private int _currentPathIndex = START_PATH_INDEX;

	public void Initialize(float speed, Node2D parent, bool start = false, List<Vector2> moverPath = null)
	{
		Speed = speed;
		ParentNode = parent;
		if (start) {Start();} else {Stop();}
		SetMoverPath(moverPath);
	}

	public override void _Process(double delta)
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

		float distance = Position.DistanceTo(_moverPath[_currentPathIndex]);
		for (int i = _currentPathIndex+1; i < _moverPath.Count; i++)
		{
			distance += _moverPath[i-1].DistanceTo(_moverPath[i]);
		}
		// GD.Print($"MoverComponent for {Owner.Name} path length from current position: {distance}");
		return distance;
	}

	public bool PathCompleted()
	{
		return _moverPath == null || _currentPathIndex >= _moverPath.Count;
	}

	public List<Vector2> GetMoverPath()
	{
		return _moverPath;
	}
	public void SetMoverPath(List<Vector2> path)
	{
		_moverPath = path;
		_currentPathIndex = START_PATH_INDEX;
	}

}
