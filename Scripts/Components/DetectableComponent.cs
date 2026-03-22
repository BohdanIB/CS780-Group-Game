
using Godot;

public partial class DetectableComponent : Area2D
{
	[Signal] public delegate void OnDetectedEventHandler(Area2D DetectorArea);
	[Signal] public delegate void OnUnDetectedEventHandler(Area2D DetectorArea);

	private SceneFilePathRes _entityScene; // Scene being detected.
	[Export] private SceneFilePathRes[] _detectorScenes = []; // Valid scenes for this component to be detected by.

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _detectableCollisionShape2D;

	public void Initialize(SceneFilePathRes[] allowedToDetectScenes = null)
	{
		if (GetParent() is var parent && IsInstanceValid(parent))
		{
			_entityScene = new SceneFilePathRes(parent);
		}
		else
		{
			_entityScene = new SceneFilePathRes(this);
		}

		if (allowedToDetectScenes != null)
		{
			_detectorScenes = allowedToDetectScenes;
		}
		if (_detectorScenes.Length <= 0)
		{
			GD.Print($"WARNING - DetectableComponent for {Owner.Name}: No detectable scenes assigned!");
		}
	}

	/// <summary>
	/// DetectorComponents can detect detectable components.
	/// </summary>
	/// <param name="detectorOwner"></param>
	public void Detect(Area2D detector, SceneFilePathRes detectorScene)
	{
		if (CanBeDetectedBy(detectorScene))
		{
			GD.Print($"DetectableComponent for {Owner} was detected by {detector.Owner.Name}. Emitting OnDetected signal.");
			EmitSignal(SignalName.OnDetected, detector);
		}
	}

	public void UnDetect(Area2D detector, SceneFilePathRes detectorScene)
	{
		if (CanBeDetectedBy(detectorScene))
		{
			GD.Print($"DetectableComponent for {Owner} lost the detection of {detector.Owner.Name}. Emitting OnUnDetected signal.");
			EmitSignal(SignalName.OnUnDetected, detector);
		}
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyDetectableRadius(float newRadius)
	{
		((CircleShape2D)_detectableCollisionShape2D.Shape).Radius = newRadius;
	}

	private bool CanBeDetectedBy(SceneFilePathRes scene)
	{
		return SceneFilePathRes.SceneSharesScenePath(scene, _detectorScenes);
	}



	public SceneFilePathRes GetEntityScene()
	{
		return _entityScene;
	}
	public SceneFilePathRes[] GetDetectorScenes()
	{
		return _detectorScenes;
	}
}
