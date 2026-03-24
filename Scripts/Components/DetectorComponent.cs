
using Godot;
using CS780GroupProject.Scripts.Utils;

public partial class DetectorComponent : Area2D
{
	[Signal] public delegate void OnEnterDetectorEventHandler(DetectableComponent Detectable);
	[Signal] public delegate void OnExitDetectorEventHandler(DetectableComponent Detectable);

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes;
	[Export] private Groups.GroupTypes _validDetectableTypes;

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _detectorCollisionShape2D;

	public void Initialize(float detectorRadius, Groups.GroupTypes entityTypes = Groups.GroupTypes.None, Groups.GroupTypes validDetectableScenes = Groups.GroupTypes.None)
	{
		ModifyDetectorRadius(detectorRadius);
		Initialize(entityTypes, validDetectableScenes);
	}

	public void Initialize(Groups.GroupTypes entityTypes = Groups.GroupTypes.None, Groups.GroupTypes validDetectableScenes = Groups.GroupTypes.None)
	{
		_thisEntityTypes = entityTypes;
		_validDetectableTypes = validDetectableScenes;
	}

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (area is DetectableComponent detectable && detectable != null)
			{
				if (Detected(detectable))
				{
					EmitSignal(SignalName.OnEnterDetector, area);
				}
			}
		};
		AreaExited += (area) =>
		{
			if (area is DetectableComponent detectable && detectable != null)
			{
				if (LostDetectionOf(detectable))
				{
					EmitSignal(SignalName.OnExitDetector, detectable);
				}
			}
		};
	}

	private bool Detected(DetectableComponent detectable)
	{
		return this.CanDetect(detectable) && detectable.CanBeDetectedBy(this);
	}
	private bool LostDetectionOf(DetectableComponent detectable)
	{
		return this.CanDetect(detectable) && detectable.CanBeDetectedBy(this);
	}

	public bool CanDetect(DetectableComponent detectable)
	{
		return CanDetect(detectable.GetEntityTypes());
	}
	private bool CanDetect(Groups.GroupTypes detectableGroupTypes)
	{
		return (detectableGroupTypes & _validDetectableTypes) != Groups.GroupTypes.None;
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyDetectorRadius(float newRadius)
	{
		((CircleShape2D)_detectorCollisionShape2D.Shape).Radius = newRadius;
	}

	public float GetDetectorRadius()
	{
		return ((CircleShape2D)_detectorCollisionShape2D.Shape).Radius;
	}
	public Groups.GroupTypes GetEntityTypes()
	{
		return _thisEntityTypes;
	}
	public Groups.GroupTypes GetValidDetectableTypes()
	{
		return _validDetectableTypes;
	}
}
