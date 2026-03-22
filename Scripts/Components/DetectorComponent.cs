
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;

public partial class DetectorComponent : Area2D
{
	[Signal] public delegate void OnEnterDetectorEventHandler(Area2D DetectedArea);
	[Signal] public delegate void OnExitDetectorEventHandler(Area2D DetectedArea);

	private SceneFilePathRes _entityScene; // Scene which is doing the detecting
	[Export] private SceneFilePathRes[] _detectableScenes = [];

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _detectorCollisionShape2D;

	public void Initialize(SceneFilePathRes[] detectableScenes = null)
	{
		if (GetParent() is var parent && IsInstanceValid(parent))
		{
			_entityScene = new SceneFilePathRes(parent);
		}
		else
		{
			_entityScene = new SceneFilePathRes(this);
		}
		
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
			// Todo: Should be a better way of excluding related nodes?
			if (area.GetParent() == this.GetParent())
			{
				return;
			}
			if (GetComponentOrNull<DetectableComponent>(area) is var detectable && IsInstanceValid(detectable))
			{
				// if (area.GetOwnerOrNull<Node>() is var owner && owner != null)
				// {
				// 	GD.Print($"DetectorComponent for {this.Owner.Name} made contact with {owner.Name}. Emitting OnEnterDetector signal.");
				// }
				// else
				// {
				// 	GD.Print($"DetectorComponent for {this.Owner.Name} made contact with {area.Name}. Emitting OnEnterDetector signal.");
				// }
				detectable.Detect(this, _entityScene);
				EmitSignal(SignalName.OnEnterDetector, area);
			}
		};
		AreaExited += (area) =>
		{
			if (GetComponentOrNull<DetectableComponent>(area) is var detectable && IsInstanceValid(detectable))
			{
				// if (area.GetOwnerOrNull<Node>() is var owner && owner != null)
				// {
				// 	GD.Print($"DetectorComponent for {this.Owner.Name} lost detection of {area.Owner.Name}. Emitting OnExitDetector signal.");
				// }
				// else
				// {
				// 	GD.Print($"DetectorComponent for {Owner.Name} lost detection of {area.Name}. Emitting OnExitDetector signal.");
				// }
				detectable.UnDetect(this, _entityScene);
				EmitSignal(SignalName.OnExitDetector, area);
			}
		};
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyDetectorRadius(float newRadius)
	{
		((CircleShape2D)_detectorCollisionShape2D.Shape).Radius = newRadius;
	}

	private DetectableComponent IsValidDetection(Area2D area)
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



	public SceneFilePathRes GetEntityScene()
	{
		return _entityScene;
	}
	public SceneFilePathRes[] GetDetectableScenes()
	{
		return _detectableScenes;
	}
}
