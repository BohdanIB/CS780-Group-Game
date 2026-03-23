
using GdUnit4;
using GdUnit4.Asserts;
using static GdUnit4.Assertions;
using System.Threading.Tasks;

namespace TestNS
{
	/// <summary>
	/// Testing:
	/// <list type="bullet">
	/// <item>Initialization</item>
	/// <item>Valid detection basic case</item>
	/// <item>Invalid detection basic case</item>
	/// <item>Valid detection of specific scene, but invalid detectable</item>
	/// <item>Node with detector and detectable works properly (cannot detect itself also important!)</item>
	/// <item>Multiple detection works properly (can detect 2 unique detectables properly)</item>
	/// </list>
	/// </summary>
	[TestSuite]
	public class DetectorDetectable
	{
		private ISceneRunner _runner = null;

		// Scene root
		private DetectorDetectableTestScene _scene;
		// Parent nodes
		private DetectorParent _detectorParent;
		private DetectableParent _detectableParent;
		private ComponentlessParent _componentlessParent;
		private DetectorDetectableParent _detectorDetectableParent;

		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://test/Components/DetectorDetectable/detector_detectable_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();
			AssertThat(_runner.Scene()).IsNotNull().IsInstanceOf<DetectorDetectableTestScene>();

			_scene = _runner.Scene() as DetectorDetectableTestScene;
			AssertThat(_scene.DetectorParent).IsNotNull();
			AssertThat(_scene.DetectableParent).IsNotNull();
			AssertThat(_scene.ComponentlessParent).IsNotNull();
			AssertThat(_scene.DetectorDetectableParent).IsNotNull();

			_detectorParent = _scene.DetectorParent;
			_detectableParent = _scene.DetectableParent;
			_componentlessParent = _scene.ComponentlessParent;
			_detectorDetectableParent = _scene.DetectorDetectableParent;
			AssertThat(_detectorParent.Detector).IsNotNull();
			AssertThat(_detectableParent.Detectable).IsNotNull();
			AssertThat(_detectorDetectableParent.Detector).IsNotNull();
			AssertThat(_detectorDetectableParent.Detectable).IsNotNull();
		}

		[TestCase]
		[RequireGodotRuntime]
		public void DetectorInitialization()
		{
			var detector = _detectorParent.Detector;
			detector.Initialize(new SceneFilePathRes[]{AutoFree(new SceneFilePathRes(_detectableParent))});
			// Check entity parent scene for component
			var currentScene = _detectorParent.Detector.GetEntityScene().ScenePath;
			var expectedScene = AutoFree(new SceneFilePathRes(_detectorParent)).ScenePath;
			AssertThat(currentScene).IsEqual(expectedScene);
			// Check detectable scenes
			var detectableScenes = detector.GetDetectableScenes();
			var expectedScenes = new SceneFilePathRes[] { AutoFree(new SceneFilePathRes(_detectableParent)) };
			AssertThat(detectableScenes)
				.IsNotNull()
				.IsEqual(expectedScenes);

			// Second Initialization
			detector.Initialize(935, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) });
			// Check entity parent scene for component
			currentScene = _detectorParent.Detector.GetEntityScene().ScenePath;
			expectedScene = AutoFree(new SceneFilePathRes(_detectorParent)).ScenePath;
			AssertThat(currentScene).IsEqual(expectedScene);
			// check radius
			AssertThat(detector.GetDetectorRadius()).IsEqual(935);
			// check detectable scenes
			expectedScenes = new SceneFilePathRes[] { AutoFree(new SceneFilePathRes(_componentlessParent)) };
			AssertThat(detector.GetDetectableScenes())
				.IsNotNull()
				.IsEqual(expectedScenes);
		}

		[TestCase]
		[RequireGodotRuntime]
		public void DetectableInitialization()
		{
			var detectable = _detectableParent.Detectable;
			detectable.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectorParent)) });

			// Check entity parent scene for component
			var currentScene = detectable.GetEntityScene().ScenePath;
			var expectedScene = AutoFree(new SceneFilePathRes(_detectableParent)).ScenePath;
			AssertThat(currentScene).IsEqual(expectedScene);

			// Check detector scenes
			var detectorScenes = detectable.GetDetectorScenes();
			var expectedScenes = new SceneFilePathRes[] { AutoFree(new SceneFilePathRes(_detectorParent)) };
			AssertThat(detectorScenes)
				.IsNotNull()
				.IsEqual(expectedScenes);

			// Second Initialization
			detectable.Initialize(935, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) });
			// Check entity parent scene for component
			currentScene = detectable.GetEntityScene().ScenePath;
			expectedScene = AutoFree(new SceneFilePathRes(_detectableParent)).ScenePath;
			AssertThat(currentScene).IsEqual(expectedScene);
			AssertThat(detectable.GetDetectableRadius()).IsEqual(935);
			// check detector scenes
			expectedScenes = new SceneFilePathRes[] { AutoFree(new SceneFilePathRes(_componentlessParent)) };
			AssertThat(detectable.GetDetectorScenes())
				.IsNotNull()
				.IsEqual(expectedScenes);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task ValidDetectionBasic()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable = _detectableParent.Detectable;
			detector.ModifyDetectorRadius(10);
			detectable.ModifyDetectableRadius(10);

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			detector.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectableParent)) });
			detectable.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectorParent)) });

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnUnDetected, detector).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task InvalidDetectionBasic()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable = _detectableParent.Detectable;
			detector.ModifyDetectorRadius(10);
			detectable.ModifyDetectableRadius(10);

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			detector.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) });
			detectable.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) });

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnUnDetected, detector).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task ValidDetectorInvalidDetectable()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable = _detectableParent.Detectable;
			detector.ModifyDetectorRadius(10);
			detectable.ModifyDetectableRadius(10);

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			detector.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectableParent)) }); // looking for DetectableParents
			detectable.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) }); // detectable by ComponentlessParents

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnUnDetected, detector).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task ValidDetectorNoDetectable()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_componentlessParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			detector.ModifyDetectorRadius(10);

			var detectorMonitor = AssertSignal(detector).StartMonitoring();

			detector.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) }); // looking for ComponentlessParents

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, _componentlessParent).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, _componentlessParent).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task InvalidDetectorValidDetectable()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable = _detectableParent.Detectable;
			detector.ModifyDetectorRadius(10);
			detectable.ModifyDetectableRadius(10);

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			detector.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) }); // looking for ComponentlessParents
			detectable.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectorParent)) }); // detectable by DetectorParents

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnUnDetected, detector).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task NoDetectorValidDetectable()
		{
			_detectableParent.GlobalPosition   = new(50,0);
			_componentlessParent.GlobalPosition = new(100,0);
			var detectable = _detectableParent.Detectable;
			detectable.ModifyDetectableRadius(10);

			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			detectable.Initialize(new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) }); // detectable by ComponentlessParents

			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectableParent.GlobalPosition = new(95,0);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected, _componentlessParent).WithTimeout(100);

			// Move them out of range of each other
			_detectableParent.GlobalPosition = new(50,0);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnUnDetected, _componentlessParent).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task DetectorDetectableDoesNotDetectItself()
		{
			_detectorDetectableParent.GlobalPosition = new(50,0);
			var detector = _detectorDetectableParent.Detector;
			var detectable = _detectorDetectableParent.Detectable;

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			// Detecting and detectable by ComponentlessParents
			detector.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) });
			detectable.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) });
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(100);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnUnDetected).WithTimeout(100);

			// Detecting and detectable by DetectorDetectableParents
			detector.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectorDetectableParent)) });
			detectable.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectorDetectableParent)) });
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(100);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnUnDetected).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task MultpleValidDetections()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			_detectorDetectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable1 = _detectableParent.Detectable;
			var detectable2 = _detectorDetectableParent.Detectable;

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectable1Monitor = AssertSignal(detectable1).StartMonitoring();
			var detectable2Monitor = AssertSignal(detectable2).StartMonitoring();

			detector.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectableParent)), AutoFree(new SceneFilePathRes(_detectorDetectableParent)) });
			detectable1.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectorParent)) });
			detectable2.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectorParent)) });

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detectable1Monitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);
			await detectable2Monitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable1).WithTimeout(100);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable2).WithTimeout(100);
			await detectable1Monitor.IsEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);
			await detectable2Monitor.IsEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);
			// detectorMonitor.IsCountEmitted(2, DetectorComponent.SignalName.OnEnterDetector); // todo

			// // Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnExitDetector, detectable1).WithTimeout(100);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnExitDetector, detectable2).WithTimeout(100);
			await detectable1Monitor.IsEmitted(DetectableComponent.SignalName.OnUnDetected, detector).WithTimeout(100);
			await detectable2Monitor.IsEmitted(DetectableComponent.SignalName.OnUnDetected, detector).WithTimeout(100);
			// // detectorMonitor.IsCountEmitted(2, DetectorComponent.SignalName.OnExitDetector, detectable1, detectable2); // todo
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task MultpleValidDetectables()
		{
			_detectableParent.GlobalPosition   = new(50,0);
			_detectorParent.GlobalPosition = new(100,0);
			_detectorDetectableParent.GlobalPosition = new(100,0);
			var detectable = _detectableParent.Detectable;
			var detector1 = _detectorParent.Detector;
			var detector2 = _detectorDetectableParent.Detector;

			var detectableMonitor = AssertSignal(detectable).StartMonitoring();
			var detector1Monitor = AssertSignal(detector1).StartMonitoring();
			var detector2Monitor = AssertSignal(detector2).StartMonitoring();

			detectable.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectorParent)), AutoFree(new SceneFilePathRes(_detectorDetectableParent)) });
			detector1.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectableParent)) });
			detector2.Initialize(10, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_detectableParent)) });

			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);
			await detector1Monitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detector2Monitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);

			// Move them in range of each other
			_detectableParent.GlobalPosition = new(95,0);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnDetected, detector1).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnDetected, detector2).WithTimeout(100);
			await detector1Monitor.IsEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detector2Monitor.IsEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			// detectableMonitor.IsCountEmitted(2, DetectableComponent.SignalName.OnDetected); // todo

			// // Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnUnDetected, detector1).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnUnDetected, detector2).WithTimeout(100);
			await detector1Monitor.IsEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detector2Monitor.IsEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			// detectableMonitor.IsCountEmitted(2, DetectableComponent.SignalName.OnUnDetected); // todo
		}
	}
}
