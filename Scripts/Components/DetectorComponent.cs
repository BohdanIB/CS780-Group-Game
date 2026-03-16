
using System.Collections.Generic;
using Godot;

public partial class DetectorComponent : Area2D
{
	[Signal] public delegate void OnEnterDetectorEventHandler(Node DetectedOwnerNode, Area2D DetectedArea, Godot.Collections.Array<Area2D> AreasInDetectorRadius);
	[Signal] public delegate void OnExitDetectorEventHandler(Node DetectedOwnerNode, Area2D DetectedArea, Godot.Collections.Array<Area2D> AreasInDetectorRadius);

	[Export] private SceneFilePathRes[] _detectableScenes = [];

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _detectorCollisionShape2D;

	private Godot.Collections.Array<Area2D> _areasInDetectorRadius = [];

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (IsValidDetection(area) is var detectableComponent && detectableComponent != null)
			{
				Node entity = area.Owner;
				GD.Print($"DetectorComponent for {this.Owner} made contact with {entity.Name}. Emitting OnEnterDetector signal.");
				detectableComponent.Detect(this.Owner, this);
				_areasInDetectorRadius.Add(area);
				EmitSignal(SignalName.OnEnterDetector, entity, area, _areasInDetectorRadius);
			}
		};
		AreaExited += (area) =>
		{
			if (_areasInDetectorRadius.Contains(area) && area is DetectableComponent detectableComponent && detectableComponent != null)
			{
				Node entity = area.Owner;
				GD.Print($"DetectorComponent for {this.Owner} lost detection of {entity.Name}. Emitting OnExitDetector signal.");
				detectableComponent.UnDetect(this.Owner, this);
				_areasInDetectorRadius.Remove(area);
				EmitSignal(SignalName.OnExitDetector, entity, area, _areasInDetectorRadius);
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
			var entity = detectable.Owner;
			// Check if the owner scene for area is detectable by this component.
			if (SceneFilePathRes.EntitySharesScenePath(entity, _detectableScenes))
			{
				return detectable;
			}
		}
		return null;
	}
}
