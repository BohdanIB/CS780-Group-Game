
using CS780GroupProject.Scripts.Utils;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;
using Godot;
using System.Collections.Generic;

namespace TestNS
{
	[TestSuite]
	public partial class TargetingTestSuite
	{
		private partial class SignalCollector : Node
		{
			// public List<DetectableComponent> TargetList { get; } = new();
			public DetectableComponent CurrentTarget { get; private set; } = null;
			// public Dictionary<DetectableComponent, int> 
			public SignalCollector(TargetingComponent targeting)
			{
				ConnectComponents(targeting);
			}
			public void ConnectComponents(TargetingComponent targeting)
			{
				targeting.OnTargetSelect += (target) =>
				{
					CurrentTarget = target;
				};
			}
			public void ClearCurrentTarget()
			{
				CurrentTarget = null;
			}
		}

		private ISceneRunner _runner = null;

		// Scene root
		private TargetingTestScene _scene;
		// Nodes
		private TargetingComponent _targeting;
		private DetectorComponent _detector;
		private DetectableComponent _detectable;

		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://Testing/Components/Targeting/targeting_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();
			AssertThat(_runner.Scene()).IsNotNull().IsInstanceOf<TargetingTestScene>();

			_scene = _runner.Scene() as TargetingTestScene;
			AssertThat(_scene.TargetingComponent).IsNotNull();
			AssertThat(_scene.DetectorComponent).IsNotNull();
			AssertThat(_scene.DetectableComponent).IsNotNull();

			_targeting = _scene.TargetingComponent;
			_detector = _scene.DetectorComponent;
			_detectable = _scene.DetectableComponent;
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task Targeting_Basic()
		{
			var targeting = _targeting;
			var detector = _detector;
			var detectable = _detectable;

			detector.GlobalPosition = new(50,0);
			detectable.GlobalPosition = new(100,0);

			SignalCollector signalCollector = AutoFree(new SignalCollector(targeting));

			// Setup targeting component and target (detectable) types
			var radius = 10f;
			var targetComponentTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret | Groups.GroupTypes.Friendly;
			var targetComponentValidTargets = Groups.GroupTypes.Enemy;
			targeting.Initialize(radius, targetComponentTypes, targetComponentValidTargets);
			AssertThat(detector.GetRadius()).IsEqual(radius);
			AssertThat(detector.GetEntityTypes()).IsEqual(targetComponentTypes);
			AssertThat(detector.GetValidDetectableTypes()).IsEqual(targetComponentValidTargets);

			var detectableEntityTypes = Groups.GroupTypes.Enemy;
			var detectableValidDetectableTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret | Groups.GroupTypes.Projectile | Groups.GroupTypes.Friendly;
			detectable.Initialize(radius, detectableEntityTypes, detectableValidDetectableTypes);

			// No targets spotted
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();

			// Move towards targetable
			detector.GlobalPosition = new(95,0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.HasSize(1)
				.Contains(detectable);

			// Move away from target and see if targeting still happens
			detector.GlobalPosition = new(50,0);
			await _runner.SimulateFrames(4);
			signalCollector.ClearCurrentTarget(); // Todo: might not work with signal buffering?
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();


		}

		
		/*
		Todo: 
		  - Targeting mode tests
		  - Multiple targets + proper target selection
		  - Proper ignoring of targets that are not valid for targeting (kind of like detector tests?)
		*/

	}
}
