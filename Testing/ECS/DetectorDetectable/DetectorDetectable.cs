
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;
using System;
using System.Diagnostics;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Testing:
/// 1. Initialization for detector and detectable works as expected
/// 2. Detector is able to detect valid detectable scene
///    - Signals work as expected
/// 3. Detector is unable to detect invalid detectable scene
/// 4. Public functions work as expected?
/// </summary>
public partial class DetectorDetectable : Node2D
{
	// [Export] private Node2D _detectorObject, _detectableObject;
	// [Export] private DetectorComponent _detector;
	// [Export] private DetectableComponent _detectable;
	[Export] private DetectorParent _detectorParent;
	[Export] private DetectableParent _detectableParent;

	public override void _Ready()
	{
		GD.Print("~~~DetectorDetectable Testing Start~~~");

		Debug.Assert(IsInstanceValid(_detectorParent));
		Debug.Assert(IsInstanceValid(_detectableParent));

		var detector = GetComponentInChildrenOrNull<DetectorComponent>(_detectorParent);
		var detectable = GetComponentInChildrenOrNull<DetectableComponent>(_detectableParent);
		Debug.Assert(IsInstanceValid(detector));
		Debug.Assert(IsInstanceValid(detectable));

		_detectorParent.GlobalPosition = new(0, 0);
		_detectableParent.GlobalPosition = new(20, 20);
		detector.ModifyDetectorRadius(1);
		detectable.ModifyDetectableRadius(1);

		// Initialization Tests
		{
			GD.Print("Initialization test");
			detector.Initialize(new SceneFilePathRes[]{new SceneFilePathRes(_detectableParent)});
			detectable.Initialize(new SceneFilePathRes[]{new SceneFilePathRes(_detectorParent)});

			// Check Entity parent scenes for components
			var currentScene = detector.GetEntityScene().ScenePath;
			var expectedScene = new SceneFilePathRes(_detectorParent).ScenePath;
			var assert = currentScene == expectedScene;
			GD.Print($"\tDetector current scene '{currentScene}' vs. expected scene '{expectedScene}'");
			Debug.Assert(assert);
			if (!assert) {return;}

			currentScene = detectable.GetEntityScene().ScenePath;
			expectedScene = new SceneFilePathRes(_detectableParent).ScenePath;
			assert = currentScene == expectedScene;
			GD.Print($"\tDetectable current scene '{currentScene}' vs. expected scene '{expectedScene}'");
			Debug.Assert(assert);
			if (!assert) {return;}

			// Check detectable/detector scenes
			var detectableScenes = detector.GetDetectableScenes();
			GD.Print($"\nCurrent detector 'detectable scenes': {SceneFilePathRes.ScenesToString(detectableScenes)}");
			assert = detectableScenes.Length == 1 && detectableScenes[0].ScenePath == new SceneFilePathRes(_detectableParent).ScenePath;
			Debug.Assert(assert);
			if (!assert) {return;}

			var detectorScenes = detectable.GetDetectorScenes();
			GD.Print($"Current detectable 'detector scenes': {SceneFilePathRes.ScenesToString(detectorScenes)}");
			assert = detectorScenes.Length == 1 && detectorScenes[0].ScenePath == new SceneFilePathRes(_detectorParent).ScenePath;
			Debug.Assert(assert);
			if (!assert) {return;}
		}

		// Valid Detection
		{
			detector.Initialize(new SceneFilePathRes[]{new SceneFilePathRes(_detectableParent)});
			detectable.Initialize(new SceneFilePathRes[]{new SceneFilePathRes(_detectorParent)});
			
			// Todo: Need to intercept detector OnEnterDetector and OnExitDetector
			// Idea: create 

			// var readyToTest
			// detector.OnEnterDetector += (area) =>
			// {
				
			// };
			// detector.OnExitDetector += (area) =>
			// {
				
			// };

		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

}
