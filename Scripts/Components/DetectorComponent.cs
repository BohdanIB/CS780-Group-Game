
using Godot;
using CS780GroupProject.Scripts.Utils;

/// <summary>
/// DetectorComponent can detect DetectableComponents as long as the DetectorComponent contains 
/// the Detectable's type, and the Detectable contains the Detector's type.
/// </summary>
public partial class DetectorComponent : Area2D
{
	[Signal] public delegate void OnEnterDetectorEventHandler(DetectableComponent Detectable);
	[Signal] public delegate void OnExitDetectorEventHandler(DetectableComponent Detectable);

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes, _validDetectableTypes;

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _detectorCollisionShape2D;

	public void Initialize(float detectorRadius, Groups.GroupTypes entityTypes, Groups.GroupTypes validDetectableTypes)
	{
		SetRadius(detectorRadius);
		Initialize(entityTypes, validDetectableTypes);
	}

	public void Initialize(Groups.GroupTypes entityTypes, Groups.GroupTypes validDetectableTypes)
	{
		_thisEntityTypes = entityTypes;
		_validDetectableTypes = validDetectableTypes;
	}

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (area is DetectableComponent detectable && detectable != null)
			{
				if (AbleToDetect(detectable))
				{
					EmitSignal(SignalName.OnEnterDetector, area);
				}
			}
		};
		AreaExited += (area) =>
		{
			if (area is DetectableComponent detectable && detectable != null)
			{
				if (AbleToDetect(detectable))
				{
					EmitSignal(SignalName.OnExitDetector, detectable);
				}
			}
		};
	}

	/// <summary>
	/// Can this DetectorComponent detect a given DetectableComponent?
	/// </summary>
	/// <param name="detectable"></param>
	/// <returns></returns>
	private bool AbleToDetect(DetectableComponent detectable)
	{
		return this.CanDetect(detectable) && detectable.CanBeDetectedBy(this);
	}
	public bool CanDetect(DetectableComponent detectable)
	{
		return (detectable.GetEntityTypes() & _validDetectableTypes) != Groups.GroupTypes.None;
	}

	public float GetRadius()
	{
		return ((CircleShape2D)_detectorCollisionShape2D.Shape).Radius;
	}
	public void SetRadius(float newRadius)
	{
		((CircleShape2D)_detectorCollisionShape2D.Shape).Radius = newRadius;
	}
	public Groups.GroupTypes GetEntityTypes()
	{
		return _thisEntityTypes;
	}
	public Groups.GroupTypes GetDetectableTypes()
	{
		return _validDetectableTypes;
	}
	public void SetDetectableTypes(Groups.GroupTypes detectableTypes)
	{
		_validDetectableTypes = detectableTypes;
	}
}