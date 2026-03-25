
using CS780GroupProject.Scripts.Utils;
using Godot;

/// <summary>
/// DetectableComponent can be detected by DetectorComponents as long as the DetectableComponent 
/// contains the Detector's type, and the Detector contains the Detectable's type.
/// </summary>
public partial class DetectableComponent : Area2D
{
	[Signal] public delegate void OnDetectedEventHandler(DetectorComponent Detector);
	[Signal] public delegate void OnLostDetectionEventHandler(DetectorComponent Detector);

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes, _validDetectorTypes;

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _detectableCollisionShape2D;

	public void Initialize(float detectableRadius, Groups.GroupTypes entityTypes, Groups.GroupTypes validDetectorTypes)
	{
		ModifyDetectableRadius(detectableRadius);
		Initialize(entityTypes, validDetectorTypes);
	}

	public void Initialize(Groups.GroupTypes entityTypes, Groups.GroupTypes validDetectorTypes)
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
				if (DetectableBy(detector))
				{
					EmitSignal(SignalName.OnDetected, detector);
				}
			}
		};
		AreaExited += (area) =>
		{
			if (area is DetectorComponent detector && detector != null)
			{
				if (DetectableBy(detector))
				{
					EmitSignal(SignalName.OnLostDetection, detector);
				}
			}
		};
	}

	/// <summary>
	/// Can this DetectableComponent be detected by a given DetectorComponent?
	/// </summary>
	/// <param name="detector"></param>
	/// <returns></returns>
	private bool DetectableBy(DetectorComponent detector)
	{
		return CanBeDetectedBy(detector) && detector.CanDetect(this);
	}
	public bool CanBeDetectedBy(DetectorComponent detector)
	{
		return (detector.GetEntityTypes() & _validDetectorTypes) != Groups.GroupTypes.None;
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
