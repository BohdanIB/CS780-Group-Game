
using Godot;

public partial class MoverComponent : Node2D
{
	private const int START_PATH_INDEX = 0;

	// [Signal] public delegate void OnMoveEventHandler(); // todo
	[Signal] public delegate void OnPathCompletedEventHandler(); // todo

	[Export] public float Speed = 20f;
	[Export] private Vector2[] _path;
	private int _currentPathIndex = START_PATH_INDEX;
	private bool _isMoving = false;

	public override void _Process(double delta)
	{
		if (_path == null) return;
		if (Owner is Node2D entity)
		{
			var targetPosition = _path[_currentPathIndex];
			var distanceToTarget = entity.Position.DistanceTo(targetPosition);
			var totalMovement = distanceToTarget*Speed*delta;
			while (totalMovement >= distanceToTarget)
			{
				entity.Position = targetPosition;
				_currentPathIndex++;
				if (PathCompleted())
				{
					EmitSignal(SignalName.OnPathCompleted);
					return;
				}
				totalMovement -= distanceToTarget;
				targetPosition = _path[_currentPathIndex];
				distanceToTarget = entity.Position.DistanceTo(targetPosition);
			}
			entity.Position = entity.Position.MoveToward(targetPosition, (float)totalMovement);
		}
		else
		{
			GD.Print($"Warning - MoverComponent's owner '{Owner.Name}' is not a Node2D, and cannot be moved in 2D space...");
		}
	}

	public void SetPath(Vector2[] newPath)
	{
		_path = newPath;
		_currentPathIndex = START_PATH_INDEX;
		_isMoving = false;
	}
	public void SetPathAndMove(Vector2[] newPath)
	{
		_path = newPath;
		_currentPathIndex = START_PATH_INDEX;
		_isMoving = (_path != null && _path.Length > 0);
	}
	public void Start()
	{
		_isMoving = true;
	}
	public void Stop()
	{
		_isMoving = false;
	}

	public float GetPathLengthFromCurrentPosition()
	{
		if (_path == null || _currentPathIndex >= _path.Length)
		{
			return 0.0f;
		}

		float distance = Position.DistanceTo(_path[_currentPathIndex]);
		for (int i = _currentPathIndex+1; i < _path.Length; i++)
		{
			distance += _path[i-1].DistanceTo(_path[i]);
		}
		// GD.Print($"MoverComponent for {Owner.Name} path length from current position: {distance}");
		return distance;
	}

	public bool PathCompleted()
	{
		if (_path == null) { return true; }

		if (_currentPathIndex >= _path.Length)
		{
			_isMoving = false;
			EmitSignal(SignalName.OnPathCompleted);
			return true;
		}
		return false;
	}

}
