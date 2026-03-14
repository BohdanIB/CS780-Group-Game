
using CS780GroupProject.Scripts.Utils;
using Godot;

public partial class DetectionComponent : Area2D
{
	[Signal] public delegate void OnDetectEventHandler(Area2D area);

	[Export] private Godot.Collections.Array<PackedScene> _detectableScenes;

	[Export] private CollisionShape2D _detectionCollisionShape2D;

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (IsValidDetection(area))
			{
				EmitSignal(SignalName.OnDetect, area);
			}
		};
	}

	// public override void _Process(double delta)
	// {
	// }

	// Todo: Handle shapes other than circles in future?
	public void ModifyDetectionRadius(float newRadius)
	{
		((CircleShape2D)_detectionCollisionShape2D.Shape).Radius = newRadius;
	}

	public bool IsValidDetection(Area2D area)
	{
		return SceneType.NodeSharesSceneType(area, _detectableScenes);
	}
}
