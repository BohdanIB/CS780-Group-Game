
using Godot;

public partial class DetectableComponent : Area2D
{
	[Signal] public delegate void OnDetectedEventHandler(Node DetectorOwnerNode, Area2D DetectorArea);
	[Signal] public delegate void OnUnDetectedEventHandler(Node DetectorOwnerNode, Area2D DetectorArea);

	[Export] private SceneFilePathRes[] _allowedToDetectEntity = []; // Valid scenes for this component to be detected by.

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _detectableCollisionShape2D;

	/// <summary>
	/// DetectorComponents can detect detectable components.
	/// </summary>
	/// <param name="detectorOwner"></param>
	public void Detect(Node detectorOwner, Area2D detector)
	{
		GD.Print($"DetectableComponent was detected by {detectorOwner.Name}. Emitting OnDetected signal.");
		EmitSignal(SignalName.OnDetected, detectorOwner, detector);
	}

	public void UnDetect(Node detectorOwner, Area2D detector)
	{
		GD.Print($"DetectableComponent lost the detection of {detectorOwner.Name}. Emitting OnUnDetected signal.");
		EmitSignal(SignalName.OnUnDetected, detectorOwner, detector);
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyDetectableRadius(float newRadius)
	{
		((CircleShape2D)_detectableCollisionShape2D.Shape).Radius = newRadius;
	}
}
