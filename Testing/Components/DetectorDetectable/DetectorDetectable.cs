
using CS780GroupProject.Scripts.Utils;
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
			_runner = ISceneRunner.Load("res://Testing/Components/DetectorDetectable/detector_detectable_test_scene.tscn", true);
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
			detector.Initialize(Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy);
			// Check for proper initialization
			AssertThat(detector.GetEntityTypes()).IsEqual(Groups.GroupTypes.Turret);
			AssertThat(detector.GetValidDetectableTypes()).IsEqual(Groups.GroupTypes.Enemy);

			// Second Initialization
			const float TEST_RADIUS = 935;
			detector.Initialize(TEST_RADIUS, Groups.GroupTypes.Projectile, Groups.GroupTypes.Structure);
			// Check for proper initialization
			AssertThat(detector.GetEntityTypes()).IsEqual(Groups.GroupTypes.Projectile);
			AssertThat(detector.GetDetectorRadius()).IsEqual(TEST_RADIUS);
			AssertThat(detector.GetValidDetectableTypes()).IsEqual(Groups.GroupTypes.Structure);
		}

		[TestCase]
		[RequireGodotRuntime]
		public void DetectableInitialization()
		{
			var detectable = _detectableParent.Detectable;
			detectable.Initialize(Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy);
			// Check for proper initialization
			AssertThat(detectable.GetEntityTypes()).IsEqual(Groups.GroupTypes.Turret);
			AssertThat(detectable.GetValidDetectorTypes()).IsEqual(Groups.GroupTypes.Enemy);

			// Second Initialization
			const float TEST_RADIUS = 935;
			detectable.Initialize(TEST_RADIUS, Groups.GroupTypes.Projectile, Groups.GroupTypes.Structure);
			// Check for proper initialization
			AssertThat(detectable.GetEntityTypes()).IsEqual(Groups.GroupTypes.Projectile);
			AssertThat(detectable.GetDetectableRadius()).IsEqual(TEST_RADIUS);
			AssertThat(detectable.GetValidDetectorTypes()).IsEqual(Groups.GroupTypes.Structure);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task ValidDetectionBasic()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable = _detectableParent.Detectable;

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			detector.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy);
			detectable.Initialize(10, Groups.GroupTypes.Enemy, Groups.GroupTypes.Turret);

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnLostDetection, detector).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task InvalidDetectionBasic()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable = _detectableParent.Detectable;

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			detector.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy);
			detectable.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy);

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnLostDetection, detector).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task ValidDetectorInvalidDetectable()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable = _detectableParent.Detectable;

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			detector.Initialize(10, Groups.GroupTypes.Projectile, Groups.GroupTypes.Enemy); // looking for Enemies
			detectable.Initialize(10, Groups.GroupTypes.Enemy, Groups.GroupTypes.Friendly); // detectable by Friendlies specifically

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnLostDetection, detector).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task InvalidDetectorValidDetectable()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable = _detectableParent.Detectable;

			var detectorMonitor = AssertSignal(detector).StartMonitoring();
			var detectableMonitor = AssertSignal(detectable).StartMonitoring();

			detector.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.None); // looking for Nothing
			detectable.Initialize(10, Groups.GroupTypes.Enemy, Groups.GroupTypes.Turret); // detectable by Turrets

			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnLostDetection, detector).WithTimeout(100);
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

			detector.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy);
			detectable.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy | Groups.GroupTypes.Turret); // Can be detected by other turrets
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(100);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnLostDetection).WithTimeout(100);
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

			detector.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy | Groups.GroupTypes.Projectile);
			detectable1.Initialize(10, Groups.GroupTypes.Enemy, Groups.GroupTypes.Turret);
			detectable2.Initialize(10, Groups.GroupTypes.Projectile, Groups.GroupTypes.Turret);

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
			await detectable1Monitor.IsEmitted(DetectableComponent.SignalName.OnLostDetection, detector).WithTimeout(100);
			await detectable2Monitor.IsEmitted(DetectableComponent.SignalName.OnLostDetection, detector).WithTimeout(100);
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

			detectable.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy | Groups.GroupTypes.Projectile);
			detector1.Initialize(10, Groups.GroupTypes.Enemy, Groups.GroupTypes.Turret);
			detector2.Initialize(10, Groups.GroupTypes.Projectile, Groups.GroupTypes.Turret);

			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(50);
			await detector1Monitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);
			await detector2Monitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector).WithTimeout(50);

			// Move them in range of each other
			_detectableParent.GlobalPosition = new(95,0);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnDetected).WithTimeout(100);
			await detector1Monitor.IsEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detector2Monitor.IsEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			// detectableMonitor.IsCountEmitted(2, DetectableComponent.SignalName.OnDetected); // todo

			// // Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnLostDetection, detector1).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnLostDetection, detector2).WithTimeout(100);
			await detector1Monitor.IsEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detector2Monitor.IsEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			// detectableMonitor.IsCountEmitted(2, DetectableComponent.SignalName.OnUnDetected); // todo
		}

	}
}
