
using CS780GroupProject.Scripts.Utils;
using GdUnit4;
using static GdUnit4.Assertions;
using Godot;
using System.Collections.Generic;
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
	public partial class DetectorDetectableTestSuite
	{
		private partial class SignalCollector : Node
		{
			public List<Node> DetectorEnterList { get; } = new();
			public List<Node> DetectableEnterList { get; } = new();
			public List<Node> DetectorExitList { get; } = new();
			public List<Node> DetectableExitList { get; } = new();

			// Lists used for tests with more than one detector or detectable
			public List<Node> Detector2EnterList { get; } = new();
			public List<Node> Detectable2EnterList { get; } = new();
			public List<Node> Detector2ExitList { get; } = new();
			public List<Node> Detectable2ExitList { get; } = new();

			public SignalCollector(DetectorComponent detector, DetectableComponent detectable, DetectorComponent detector2 = null, DetectableComponent detectable2 = null)
			{
				ConnectComponents(detector, detectable, detector2, detectable2);
			}
			public void ConnectComponents(DetectorComponent detector, DetectableComponent detectable, DetectorComponent detector2 = null, DetectableComponent detectable2 = null)
			{
				detector.OnEnterDetector += (detectable) => {
					DetectorEnterList.Add(detectable);
				};
				detector.OnExitDetector += (detectable) =>
				{
					DetectorExitList.Add(detectable);
				};

				detectable.OnEnterDetectable += (detector) => {
					DetectableEnterList.Add(detector);
				};
				detectable.OnExitDetectable += (detector) =>
				{
					DetectableExitList.Add(detector);
				};

				if (detector2 != null)
				{
					detector2.OnEnterDetector += (detectable) => {
						Detector2EnterList.Add(detectable);
					};
					detector2.OnExitDetector += (detectable) =>
					{
						Detector2ExitList.Add(detectable);
					};
				}
				if (detectable2 != null)
				{
					detectable2.OnEnterDetectable += (detector) => {
						Detectable2EnterList.Add(detector);
					};
					detectable2.OnExitDetectable += (detector) =>
					{
						Detectable2ExitList.Add(detector);
					};
				}
			}
		}
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
			AssertThat(detector.GetDetectableTypes()).IsEqual(Groups.GroupTypes.Enemy);

			// Second Initialization
			const float TEST_RADIUS = 935;
			detector.Initialize(TEST_RADIUS, Groups.GroupTypes.Projectile, Groups.GroupTypes.Structure);
			// Check for proper initialization
			AssertThat(detector.GetEntityTypes()).IsEqual(Groups.GroupTypes.Projectile);
			AssertThat(detector.GetRadius()).IsEqual(TEST_RADIUS);
			AssertThat(detector.GetDetectableTypes()).IsEqual(Groups.GroupTypes.Structure);
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
			AssertThat(detectable.GetRadius()).IsEqual(TEST_RADIUS);
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
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnEnterDetectable).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnEnterDetectable, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsEmitted(DetectableComponent.SignalName.OnExitDetectable, detector).WithTimeout(100);
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
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnEnterDetectable).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnEnterDetectable, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnExitDetectable, detector).WithTimeout(100);
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
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnEnterDetectable).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnEnterDetectable, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnExitDetectable, detector).WithTimeout(100);
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
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnEnterDetectable).WithTimeout(50);

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnEnterDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnEnterDetectable, detector).WithTimeout(100);

			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector, detectable).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnExitDetectable, detector).WithTimeout(100);
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
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnEnterDetectable).WithTimeout(100);
			await detectorMonitor.IsNotEmitted(DetectorComponent.SignalName.OnExitDetector).WithTimeout(100);
			await detectableMonitor.IsNotEmitted(DetectableComponent.SignalName.OnExitDetectable).WithTimeout(100);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task MultpleValidDetectables()
		{
			_detectorParent.GlobalPosition   = new(50,0);
			_detectableParent.GlobalPosition = new(100,0);
			_detectorDetectableParent.GlobalPosition = new(100,0);
			var detector = _detectorParent.Detector;
			var detectable1 = _detectableParent.Detectable;
			var detectable2 = _detectorDetectableParent.Detectable;

			var signalCollector = AutoFree(new SignalCollector(detector, detectable1, detectable2: detectable2));
			_scene.AddChild(signalCollector);

			detector.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy | Groups.GroupTypes.Projectile);
			detectable1.Initialize(10, Groups.GroupTypes.Enemy, Groups.GroupTypes.Turret);
			detectable2.Initialize(10, Groups.GroupTypes.Projectile, Groups.GroupTypes.Turret);

			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.DetectorEnterList).IsEmpty();
			AssertThat(signalCollector.DetectableEnterList).IsEmpty();
			AssertThat(signalCollector.Detectable2EnterList).IsEmpty();
			AssertThat(signalCollector.DetectorExitList).IsEmpty();
			AssertThat(signalCollector.DetectableExitList).IsEmpty();
			AssertThat(signalCollector.Detectable2ExitList).IsEmpty();

			// Move them in range of each other
			_detectorParent.GlobalPosition = new(95,0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.DetectorEnterList)
				.HasSize(2)
				.Contains(detectable1, detectable2);
			AssertThat(signalCollector.DetectableEnterList)
				.HasSize(1)
				.Contains(detector);
			AssertThat(signalCollector.Detectable2EnterList)
				.HasSize(1)
				.Contains(detector);
			AssertThat(signalCollector.DetectorExitList).IsEmpty();
			AssertThat(signalCollector.DetectableExitList).IsEmpty();
			AssertThat(signalCollector.Detectable2ExitList).IsEmpty();
			
			// Move them out of range of each other
			_detectorParent.GlobalPosition = new(50,0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.DetectorExitList)
				.HasSize(2)
				.Contains(detectable1, detectable2);
			AssertThat(signalCollector.DetectableExitList)
				.HasSize(1)
				.Contains(detector);
			AssertThat(signalCollector.Detectable2ExitList)
				.HasSize(1)
				.Contains(detector);
			AssertThat(signalCollector.DetectorEnterList).HasSize(2);
			AssertThat(signalCollector.DetectableEnterList).HasSize(1);
			AssertThat(signalCollector.Detectable2EnterList).HasSize(1);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task MultpleValidDetectors()
		{
			_detectableParent.GlobalPosition = new(50,0);
			_detectorParent.GlobalPosition = new(100,0);
			_detectorDetectableParent.GlobalPosition = new(100,0);
			var detectable = _detectableParent.Detectable;
			var detector1 = _detectorParent.Detector;
			var detector2 = _detectorDetectableParent.Detector;

			var signalCollector = AutoFree(new SignalCollector(detector1, detectable, detector2: detector2));
			_scene.AddChild(signalCollector);

			detectable.Initialize(10, Groups.GroupTypes.Turret, Groups.GroupTypes.Enemy | Groups.GroupTypes.Projectile);
			detector1.Initialize(10, Groups.GroupTypes.Enemy, Groups.GroupTypes.Turret);
			detector2.Initialize(10, Groups.GroupTypes.Projectile, Groups.GroupTypes.Turret);

			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.DetectableEnterList).IsEmpty();
			AssertThat(signalCollector.DetectorEnterList).IsEmpty();
			AssertThat(signalCollector.Detector2EnterList).IsEmpty();
			AssertThat(signalCollector.DetectableExitList).IsEmpty();
			AssertThat(signalCollector.DetectorExitList).IsEmpty();
			AssertThat(signalCollector.Detector2ExitList).IsEmpty();

			// Move them in range of each other
			_detectableParent.GlobalPosition = new(95,0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.DetectableEnterList)
				.HasSize(2)
				.Contains(detector1, detector2);
			AssertThat(signalCollector.DetectorEnterList)
				.HasSize(1)
				.Contains(detectable);
			AssertThat(signalCollector.Detector2EnterList)
				.HasSize(1)
				.Contains(detectable);
			AssertThat(signalCollector.DetectableExitList).IsEmpty();
			AssertThat(signalCollector.DetectorExitList).IsEmpty();
			AssertThat(signalCollector.Detector2ExitList).IsEmpty();

			// Move them out of range of each other
			_detectableParent.GlobalPosition = new(50,0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.DetectableExitList)
				.HasSize(2)
				.Contains(detector1, detector2);
			AssertThat(signalCollector.DetectorExitList)
				.HasSize(1)
				.Contains(detectable);
			AssertThat(signalCollector.Detector2ExitList)
				.HasSize(1)
				.Contains(detectable);
			AssertThat(signalCollector.DetectableEnterList).HasSize(2);
			AssertThat(signalCollector.DetectorEnterList).HasSize(1);
			AssertThat(signalCollector.Detector2EnterList).HasSize(1);
		}

	}
}
