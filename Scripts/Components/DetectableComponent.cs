
using CS780GroupProject.Scripts.Utils;
using Godot;

public partial class DetectableComponent : Area2D
{
	[Signal] public delegate void OnDetectedEventHandler(DetectorComponent Detector);
	[Signal] public delegate void OnLostDetectionEventHandler(DetectorComponent Detector);

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes;
	[Export] private Groups.GroupTypes _validDetectorTypes;

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _detectableCollisionShape2D;

	public void Initialize(float detectableRadius, Groups.GroupTypes entityTypes = Groups.GroupTypes.None, Groups.GroupTypes validDetectorTypes = Groups.GroupTypes.None)
	{
		ModifyDetectableRadius(detectableRadius);
		Initialize(entityTypes, validDetectorTypes);
	}

	public void Initialize(Groups.GroupTypes entityTypes = Groups.GroupTypes.None, Groups.GroupTypes validDetectorTypes = Groups.GroupTypes.None)
	{
		_thisEntityTypes = entityTypes;
		_validDetectorTypes = validDetectorTypes;
	}

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (area is DetectorComponent detector && detector != null)
			{
				if (SpottedBy(detector))
				{
					EmitSignal(SignalName.OnDetected, detector);
				}
			}
		};
		AreaExited += (area) =>
		{
			if (area is DetectorComponent detector && detector != null)
			{
				if (ConcealedFrom(detector))
				{
					EmitSignal(SignalName.OnLostDetection, detector);
				}
			}
		};
	}

	private bool SpottedBy(DetectorComponent detector)
	{
		return CanBeDetectedBy(detector) && detector.CanDetect(this);
	}
	private bool ConcealedFrom(DetectorComponent detector)
	{
		return CanBeDetectedBy(detector) && detector.CanDetect(this);
	}

	public bool CanBeDetectedBy(DetectorComponent detector)
	{
		return CanBeDetectedBy(detector.GetEntityTypes());
	}
	private bool CanBeDetectedBy(Groups.GroupTypes detectorGroupTypes)
	{
		return (detectorGroupTypes & _validDetectorTypes) != Groups.GroupTypes.None;
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyDetectableRadius(float newRadius)
	{
		((CircleShape2D)_detectableCollisionShape2D.Shape).Radius = newRadius;
	}

	public float GetDetectableRadius()
	{
		return ((CircleShape2D)_detectableCollisionShape2D.Shape).Radius;
	}
    public Groups.GroupTypes GetEntityTypes()
	{
		return _thisEntityTypes;
	}
	public Groups.GroupTypes GetValidDetectorTypes()
	{
		return _validDetectorTypes;
	}
}
