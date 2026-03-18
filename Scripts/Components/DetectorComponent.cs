
using Godot;

public partial class DetectorComponent : Area2D
{
	[Signal] public delegate void OnEnterDetectorEventHandler(Area2D DetectedArea);
	[Signal] public delegate void OnExitDetectorEventHandler(Area2D DetectedArea);

	[Export] private SceneFilePathRes[] _detectableScenes = [];
	[Export] private SceneFilePathRes _detectorScene; // Scene which is doing the detecting

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _detectorCollisionShape2D;

	public void Initialize(SceneFilePathRes detectorScene, SceneFilePathRes[] detectableScenes = null)
	{
		_detectorScene = detectorScene;
		if (detectableScenes != null)
		{
			_detectableScenes = detectableScenes;
		}
		if (_detectableScenes.Length <= 0)
		{
			GD.Print($"WARNING - DetectorComponent for {Owner.Name}: No detectable scenes assigned!");
		}
	}

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (IsValidDetection(area) is var detectableComponent && detectableComponent != null)
			{
				GD.Print($"DetectorComponent for {this.Owner} made contact with {area.Owner.Name}. Emitting OnEnterDetector signal.");
				detectableComponent.Detect(this, _detectorScene);
				EmitSignal(SignalName.OnEnterDetector, area);
			}
		};
		AreaExited += (area) =>
		{
			if (area is DetectableComponent detectableComponent && detectableComponent != null)
			{
				GD.Print($"DetectorComponent for {this.Owner} lost detection of {area.Owner.Name}. Emitting OnExitDetector signal.");
				detectableComponent.UnDetect(this, _detectorScene);
				EmitSignal(SignalName.OnExitDetector, area);
			}
		};
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyDetectorRadius(float newRadius)
	{
		((CircleShape2D)_detectorCollisionShape2D.Shape).Radius = newRadius;
	}

	public DetectableComponent IsValidDetection(Area2D area)
	{
		if (area is DetectableComponent detectable) // Todo: Unfortunate coupling, but can't wizard up a better way without getting godot layers or groups involved.
		{
			// GD.Print("AREA IS DETECTABLE COMPONENT.");
			// Check if the owner scene for area is detectable by this component.
			if (SceneFilePathRes.EntitySharesScenePath(detectable.Owner, _detectableScenes))
			{
				return detectable;
			}
		}
		return null;
	}
}
