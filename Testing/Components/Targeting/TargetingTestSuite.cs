
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
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

		private static uint GENERIC_WAIT_FRAMES = 4;

		private ISceneRunner _runner = null;

		// Scene root
		private TargetingTestScene _scene;
		// Nodes
		private Node2D 
			_targetingNode,
			_target1, _target2, _target3;
		// Convenience references
		private TargetingComponent _targeting;
		private DetectorComponent _targetingDetector;

		private DetectableComponent _detectable1, _detectable2, _detectable3;
		private HealthComponent _health1, _health2, _health3;
		private MoverComponent _mover1, _mover2, _mover3;

		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://Testing/Components/Targeting/targeting_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();
			AssertThat(_runner.Scene()).IsNotNull().IsInstanceOf<TargetingTestScene>();

			_scene = _runner.Scene() as TargetingTestScene;
			AssertThat(_scene.TargetingNode).IsNotNull();
			AssertThat(_scene.Target1).IsNotNull();
			AssertThat(_scene.Target2).IsNotNull();
			AssertThat(_scene.Target3).IsNotNull();

			_targetingNode = _scene.TargetingNode;
			_target1 = _scene.Target1;
			_target2 = _scene.Target2;
			_target3 = _scene.Target3;
			
			// Targeting Components
			_targeting = GetComponentInChildrenOrNull<TargetingComponent>(_targetingNode); 
			_targetingDetector = GetComponentInChildrenOrNull<DetectorComponent>(_targetingNode);
			AssertThat(_targeting).IsNotNull();
			AssertThat(_targetingDetector).IsNotNull();

			// Target Components
			_detectable1 = GetComponentInChildrenOrNull<DetectableComponent>(_target1);
			_detectable2 = GetComponentInChildrenOrNull<DetectableComponent>(_target2);
			_detectable3 = GetComponentInChildrenOrNull<DetectableComponent>(_target3);
			_health1 = GetComponentInChildrenOrNull<HealthComponent>(_target1);
			_health2 = GetComponentInChildrenOrNull<HealthComponent>(_target2);
			_health3 = GetComponentInChildrenOrNull<HealthComponent>(_target3);
			_mover1 = GetComponentInChildrenOrNull<MoverComponent>(_target1);
			_mover2 = GetComponentInChildrenOrNull<MoverComponent>(_target2);
			_mover3 = GetComponentInChildrenOrNull<MoverComponent>(_target3);
			AssertThat(_detectable1).IsNotNull();
			AssertThat(_detectable2).IsNotNull();
			AssertThat(_detectable3).IsNotNull();
			AssertThat(_health1).IsNotNull();
			AssertThat(_health2).IsNotNull();
			AssertThat(_health3).IsNotNull();
			AssertThat(_mover1).IsNotNull();
			AssertThat(_mover2).IsNotNull();
			AssertThat(_mover3).IsNotNull();
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task Targeting_Basic()
		{
			var targeting = _targeting;
			var detector = _targetingDetector;
			var detectable = _detectable1;

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
			AssertThat(detector.GetDetectableTypes()).IsEqual(targetComponentValidTargets);

			var detectableEntityTypes = Groups.GroupTypes.Enemy;
			var detectableValidDetectableTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret | Groups.GroupTypes.Projectile | Groups.GroupTypes.Friendly;
			detectable.Initialize(radius, detectableEntityTypes, detectableValidDetectableTypes);

			// No targets spotted
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();

			// Move towards targetable
			detector.GlobalPosition = new(95,0);
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.HasSize(1)
				.Contains(detectable);

			// Move away from target and see if targeting still happens
			detector.GlobalPosition = new(50,0);
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			signalCollector.ClearCurrentTarget(); // Todo: hard test condition here
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();
		}

		/// <summary>
		/// Targeting targets which are furthest down their respective paths
		/// </summary>
		/// <returns></returns>
		[TestCase]
		[RequireGodotRuntime]
		public async Task Targeting_First()
		{
			// T->  |T1|  |T2|  |T3|
			const float INTER_TARGET_DISTANCE = 10;
			Vector2 INTER_TARGET_VECTOR = new(INTER_TARGET_DISTANCE, 0);
			Vector2 TARGETING_POSITION = new(100, 0);
			Vector2 CLOSEST_POSITION   = TARGETING_POSITION + INTER_TARGET_VECTOR;
			Vector2 MIDDLE_POSITION    = CLOSEST_POSITION + INTER_TARGET_VECTOR;
			Vector2 FARTHEST_POSITION  = MIDDLE_POSITION + INTER_TARGET_VECTOR;

			var targetingNode = _targetingNode;
			var targeting = _targeting;
			
			var target1 = _target1;
			var target2 = _target2;
			var untargetable = _target3;

			var detectable1 = _detectable1;
			var detectable2 = _detectable2;
			var undetectable = _detectable3;

			var mover1 = _mover1;
			var mover2 = _mover2;
			var moverUndetectable = _mover3;

			targetingNode.GlobalPosition = -FARTHEST_POSITION; // Make sure we're not near the targets to start
			target1.GlobalPosition = CLOSEST_POSITION;
			target2.GlobalPosition = MIDDLE_POSITION;
			untargetable.GlobalPosition = FARTHEST_POSITION;

			SignalCollector signalCollector = AutoFree(new SignalCollector(targeting));

			// Setup components
			var range = FARTHEST_POSITION.X - CLOSEST_POSITION.X + (INTER_TARGET_DISTANCE*2);
			var targetingTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret | Groups.GroupTypes.Friendly;
			var targetingValidTargets = Groups.GroupTypes.Enemy;
			targeting.Initialize(range, targetingTypes, targetingValidTargets);
			targeting.TargetingStyle = TargetingMode.First; // target closest to finishing their path

			var targetTypes = Groups.GroupTypes.Enemy;
			var targetValidTargetingEntitites = targetingTypes | Groups.GroupTypes.Projectile;
			detectable1.Initialize(targetTypes, targetValidTargetingEntitites);
			detectable2.Initialize(targetTypes, targetValidTargetingEntitites);
			undetectable.Initialize(targetTypes, Groups.GroupTypes.None); // valid target, but undetectable

			// All movers are using the same movement path to judge if targeting is targeting properly, but will not move using mover components (mover test suite)
			var speed = 0f;
			Vector2[] movementPath = [FARTHEST_POSITION + INTER_TARGET_VECTOR, (FARTHEST_POSITION*2) + INTER_TARGET_VECTOR];
			mover1.Initialize(speed, target1, start: false, moverPath: movementPath);
			mover2.Initialize(speed, target2, start: false, moverPath: movementPath);
			moverUndetectable.Initialize(speed, untargetable, start: false, moverPath: movementPath);

			// Check that no signals or anything have fired off
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();

			// Move targeting in range of targets, make sure proper target being targeted
			targetingNode.GlobalPosition = TARGETING_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			// shuffle target ordering and check targeting changes appropriately
			target2.GlobalPosition = CLOSEST_POSITION;
			untargetable.GlobalPosition = MIDDLE_POSITION;
			target1.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			undetectable.GlobalPosition = CLOSEST_POSITION;
			target1.GlobalPosition = MIDDLE_POSITION;
			target2.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			// reverse path for targets
			target1.GlobalPosition = CLOSEST_POSITION;
			target2.GlobalPosition = MIDDLE_POSITION;
			undetectable.GlobalPosition = FARTHEST_POSITION;
			for (int i = 0; i < movementPath.Length; i++)
			{
				movementPath[i] = -movementPath[i];
			}
			mover1.SetMoverPath(movementPath);
			mover2.SetMoverPath(movementPath);
			moverUndetectable.SetMoverPath(movementPath);
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			target2.GlobalPosition = CLOSEST_POSITION;
			untargetable.GlobalPosition = MIDDLE_POSITION;
			target1.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			untargetable.GlobalPosition = CLOSEST_POSITION;
			target1.GlobalPosition = MIDDLE_POSITION;
			target2.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			// All targets go out of range == no targets.
			targetingNode.GlobalPosition = -FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			signalCollector.ClearCurrentTarget(); // Todo: hard test condition here
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();
		}

		/// <summary>
		/// Targeting targets which have made the least progress down their respective paths
		/// </summary>
		/// <returns></returns>
		[TestCase]
		[RequireGodotRuntime]
		public async Task Targeting_Last()
		{
			// T->  |T1|  |T2|  |T3|
			const float INTER_TARGET_DISTANCE = 10;
			Vector2 INTER_TARGET_VECTOR = new(INTER_TARGET_DISTANCE, 0);
			Vector2 TARGETING_POSITION = new(100, 0);
			Vector2 CLOSEST_POSITION   = TARGETING_POSITION + INTER_TARGET_VECTOR;
			Vector2 MIDDLE_POSITION    = CLOSEST_POSITION + INTER_TARGET_VECTOR;
			Vector2 FARTHEST_POSITION  = MIDDLE_POSITION + INTER_TARGET_VECTOR;

			var targetingNode = _targetingNode;
			var targeting = _targeting;
			
			var target1 = _target1;
			var target2 = _target2;
			var untargetable = _target3;

			var detectable1 = _detectable1;
			var detectable2 = _detectable2;
			var undetectable = _detectable3;

			var mover1 = _mover1;
			var mover2 = _mover2;
			var moverUndetectable = _mover3;

			targetingNode.GlobalPosition = -FARTHEST_POSITION; // Make sure we're not near the targets to start
			target1.GlobalPosition = CLOSEST_POSITION;
			target2.GlobalPosition = MIDDLE_POSITION;
			untargetable.GlobalPosition = FARTHEST_POSITION;

			SignalCollector signalCollector = AutoFree(new SignalCollector(targeting));

			// Setup components
			var range = FARTHEST_POSITION.X - CLOSEST_POSITION.X + (INTER_TARGET_DISTANCE*2);
			var targetingTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret | Groups.GroupTypes.Friendly;
			var targetingValidTargets = Groups.GroupTypes.Enemy;
			targeting.Initialize(range, targetingTypes, targetingValidTargets);
			targeting.TargetingStyle = TargetingMode.Last; // target farthest away from finishing their path

			var targetTypes = Groups.GroupTypes.Enemy;
			var targetValidTargetingEntitites = targetingTypes | Groups.GroupTypes.Projectile;
			detectable1.Initialize(targetTypes, targetValidTargetingEntitites);
			detectable2.Initialize(targetTypes, targetValidTargetingEntitites);
			undetectable.Initialize(targetTypes, Groups.GroupTypes.None); // valid target, but undetectable

			// All movers are using the same movement path to judge if targeting is targeting properly, but will not move using mover components (mover test suite)
			var speed = 0f;
			Vector2[] movementPath = [FARTHEST_POSITION + INTER_TARGET_VECTOR, (FARTHEST_POSITION*2) + INTER_TARGET_VECTOR];
			mover1.Initialize(speed, target1, start: false, moverPath: movementPath);
			mover2.Initialize(speed, target2, start: false, moverPath: movementPath);
			moverUndetectable.Initialize(speed, untargetable, start: false, moverPath: movementPath);

			// Check that no signals or anything have fired off
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();

			// Move targeting in range of targets, make sure proper target being targeted
			targetingNode.GlobalPosition = TARGETING_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			// shuffle target ordering and check targeting changes appropriately
			target2.GlobalPosition = CLOSEST_POSITION;
			untargetable.GlobalPosition = MIDDLE_POSITION;
			target1.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			undetectable.GlobalPosition = CLOSEST_POSITION;
			target1.GlobalPosition = MIDDLE_POSITION;
			target2.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);			

			// reverse path for targets
			target1.GlobalPosition = CLOSEST_POSITION;
			target2.GlobalPosition = MIDDLE_POSITION;
			undetectable.GlobalPosition = FARTHEST_POSITION;
			for (int i = 0; i < movementPath.Length; i++)
			{
				movementPath[i] = -movementPath[i];
			}
			mover1.SetMoverPath(movementPath);
			mover2.SetMoverPath(movementPath);
			moverUndetectable.SetMoverPath(movementPath);
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			target2.GlobalPosition = CLOSEST_POSITION;
			untargetable.GlobalPosition = MIDDLE_POSITION;
			target1.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			untargetable.GlobalPosition = CLOSEST_POSITION;
			target1.GlobalPosition = MIDDLE_POSITION;
			target2.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			// All targets go out of range == no targets.
			targetingNode.GlobalPosition = -FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			signalCollector.ClearCurrentTarget(); // Todo: hard test condition here
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();
		}

		/// <summary>
		/// Targeting closest target
		/// </summary>
		/// <returns></returns>
		[TestCase]
		[RequireGodotRuntime]
		public async Task Targeting_Close()
		{
			// T->  |T1|  |T2|  |T3|
			const float INTER_TARGET_DISTANCE = 10;
			Vector2 INTER_TARGET_VECTOR = new(INTER_TARGET_DISTANCE, 0);
			Vector2 TARGETING_POSITION = new(100, 0);
			Vector2 CLOSEST_POSITION   = TARGETING_POSITION + INTER_TARGET_VECTOR;
			Vector2 MIDDLE_POSITION    = CLOSEST_POSITION + INTER_TARGET_VECTOR;
			Vector2 FARTHEST_POSITION  = MIDDLE_POSITION + INTER_TARGET_VECTOR;

			var targetingNode = _targetingNode;
			var targeting = _targeting;
			
			var target1 = _target1;
			var target2 = _target2;
			var untargetable = _target3;

			var detectable1 = _detectable1;
			var detectable2 = _detectable2;
			var undetectable = _detectable3;

			targetingNode.GlobalPosition = -FARTHEST_POSITION; // Make sure we're not near the targets to start
			target1.GlobalPosition = CLOSEST_POSITION;
			target2.GlobalPosition = MIDDLE_POSITION;
			untargetable.GlobalPosition = FARTHEST_POSITION;

			SignalCollector signalCollector = AutoFree(new SignalCollector(targeting));

			// Setup components
			var range = FARTHEST_POSITION.X - CLOSEST_POSITION.X + (INTER_TARGET_DISTANCE*2);
			var targetingTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret | Groups.GroupTypes.Friendly;
			var targetingValidTargets = Groups.GroupTypes.Enemy;
			targeting.Initialize(range, targetingTypes, targetingValidTargets);
			targeting.TargetingStyle = TargetingMode.Close; // closest target

			var targetTypes = Groups.GroupTypes.Enemy;
			var targetValidTargetingEntitites = targetingTypes | Groups.GroupTypes.Projectile;
			detectable1.Initialize(targetTypes, targetValidTargetingEntitites);
			detectable2.Initialize(targetTypes, targetValidTargetingEntitites);
			undetectable.Initialize(targetTypes, Groups.GroupTypes.None); // valid target, but undetectable

			// Check that no signals or anything have fired off
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();

			// Move targeting in range of targets, make sure proper target being targeted
			targetingNode.GlobalPosition = TARGETING_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			// shuffle target ordering and check targeting changes appropriately
			target2.GlobalPosition = CLOSEST_POSITION;
			untargetable.GlobalPosition = MIDDLE_POSITION;
			target1.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			undetectable.GlobalPosition = CLOSEST_POSITION;
			target1.GlobalPosition = MIDDLE_POSITION;
			target2.GlobalPosition = FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);			

			// All targets go out of range == no targets.
			targetingNode.GlobalPosition = -FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			signalCollector.ClearCurrentTarget(); // Todo: hard test condition here
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();
		}

		/// <summary>
		/// Targeting weakest target
		/// </summary>
		/// <returns></returns>
		[TestCase]
		[RequireGodotRuntime]
		public async Task Targeting_Weak()
		{
			// T->  |T1|  |T2|  |T3|
			const float INTER_TARGET_DISTANCE = 10;
			Vector2 INTER_TARGET_VECTOR = new(INTER_TARGET_DISTANCE, 0);
			Vector2 TARGETING_POSITION = new(100, 0);
			Vector2 CLOSEST_POSITION   = TARGETING_POSITION + INTER_TARGET_VECTOR;
			Vector2 MIDDLE_POSITION    = CLOSEST_POSITION + INTER_TARGET_VECTOR;
			Vector2 FARTHEST_POSITION  = MIDDLE_POSITION + INTER_TARGET_VECTOR;

			var targetingNode = _targetingNode;
			var targeting = _targeting;
			
			var target1 = _target1;
			var target2 = _target2;
			var untargetable = _target3;

			var detectable1 = _detectable1;
			var detectable2 = _detectable2;
			var undetectable = _detectable3;

			var health1 = _health1;
			var health2 = _health2;
			var healthUndetectable = _health3;

			targetingNode.GlobalPosition = -FARTHEST_POSITION; // Make sure we're not near the targets to start
			target1.GlobalPosition = CLOSEST_POSITION;
			target2.GlobalPosition = MIDDLE_POSITION;
			untargetable.GlobalPosition = FARTHEST_POSITION;

			SignalCollector signalCollector = AutoFree(new SignalCollector(targeting));

			// Setup components
			var range = FARTHEST_POSITION.X - CLOSEST_POSITION.X + (INTER_TARGET_DISTANCE*2);
			var targetingTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret | Groups.GroupTypes.Friendly;
			var targetingValidTargets = Groups.GroupTypes.Enemy;
			targeting.Initialize(range, targetingTypes, targetingValidTargets);
			targeting.TargetingStyle = TargetingMode.Weak; // weakest target

			var targetTypes = Groups.GroupTypes.Enemy;
			var targetValidTargetingEntitites = targetingTypes | Groups.GroupTypes.Projectile;
			detectable1.Initialize(targetTypes, targetValidTargetingEntitites);
			detectable2.Initialize(targetTypes, targetValidTargetingEntitites);
			undetectable.Initialize(targetTypes, Groups.GroupTypes.None); // valid target, but undetectable

			const float LOW_HEALTH    = 10f;
			const float MEDIUM_HEALTH = 50f;
			const float HIGH_HEALTH   = 100f;
			health1.SetHealth(LOW_HEALTH);
			health2.SetHealth(MEDIUM_HEALTH);
			healthUndetectable.SetHealth(HIGH_HEALTH);

			// Check that no signals or anything have fired off
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();

			// Move targeting in range of targets, make sure proper target being targeted
			targetingNode.GlobalPosition = TARGETING_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			// shuffle target health
			health1.SetHealth(MEDIUM_HEALTH);
			health2.SetHealth(HIGH_HEALTH);
			healthUndetectable.SetHealth(LOW_HEALTH);
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			health1.SetHealth(HIGH_HEALTH);
			health2.SetHealth(LOW_HEALTH);
			healthUndetectable.SetHealth(MEDIUM_HEALTH);
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);			

			// All targets go out of range == no targets.
			targetingNode.GlobalPosition = -FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			signalCollector.ClearCurrentTarget(); // Todo: hard test condition here
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();
		}

		/// <summary>
		/// Targeting strongest target
		/// </summary>
		/// <returns></returns>
		[TestCase]
		[RequireGodotRuntime]
		public async Task Targeting_Strong()
		{
			// T->  |T1|  |T2|  |T3|
			const float INTER_TARGET_DISTANCE = 10;
			Vector2 INTER_TARGET_VECTOR = new(INTER_TARGET_DISTANCE, 0);
			Vector2 TARGETING_POSITION = new(100, 0);
			Vector2 CLOSEST_POSITION   = TARGETING_POSITION + INTER_TARGET_VECTOR;
			Vector2 MIDDLE_POSITION    = CLOSEST_POSITION + INTER_TARGET_VECTOR;
			Vector2 FARTHEST_POSITION  = MIDDLE_POSITION + INTER_TARGET_VECTOR;

			var targetingNode = _targetingNode;
			var targeting = _targeting;
			
			var target1 = _target1;
			var target2 = _target2;
			var untargetable = _target3;

			var detectable1 = _detectable1;
			var detectable2 = _detectable2;
			var undetectable = _detectable3;

			var health1 = _health1;
			var health2 = _health2;
			var healthUndetectable = _health3;

			targetingNode.GlobalPosition = -FARTHEST_POSITION; // Make sure we're not near the targets to start
			target1.GlobalPosition = CLOSEST_POSITION;
			target2.GlobalPosition = MIDDLE_POSITION;
			untargetable.GlobalPosition = FARTHEST_POSITION;

			SignalCollector signalCollector = AutoFree(new SignalCollector(targeting));

			// Setup components
			var range = FARTHEST_POSITION.X - CLOSEST_POSITION.X + (INTER_TARGET_DISTANCE*2);
			var targetingTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret | Groups.GroupTypes.Friendly;
			var targetingValidTargets = Groups.GroupTypes.Enemy;
			targeting.Initialize(range, targetingTypes, targetingValidTargets);
			targeting.TargetingStyle = TargetingMode.Strong; // strongest target

			var targetTypes = Groups.GroupTypes.Enemy;
			var targetValidTargetingEntitites = targetingTypes | Groups.GroupTypes.Projectile;
			detectable1.Initialize(targetTypes, targetValidTargetingEntitites);
			detectable2.Initialize(targetTypes, targetValidTargetingEntitites);
			undetectable.Initialize(targetTypes, Groups.GroupTypes.None); // valid target, but undetectable

			const float LOW_HEALTH    = 10f;
			const float MEDIUM_HEALTH = 50f;
			const float HIGH_HEALTH   = 100f;
			health1.SetHealth(LOW_HEALTH);
			health2.SetHealth(MEDIUM_HEALTH);
			healthUndetectable.SetHealth(HIGH_HEALTH);

			// Check that no signals or anything have fired off
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();

			// Move targeting in range of targets, make sure proper target being targeted
			targetingNode.GlobalPosition = TARGETING_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			// shuffle target health
			health1.SetHealth(MEDIUM_HEALTH);
			health2.SetHealth(HIGH_HEALTH);
			healthUndetectable.SetHealth(LOW_HEALTH);
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable2);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);

			health1.SetHealth(HIGH_HEALTH);
			health2.SetHealth(LOW_HEALTH);
			healthUndetectable.SetHealth(MEDIUM_HEALTH);
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget)
				.IsNotNull()
				.IsEqual(detectable1);
			AssertThat(targeting.GetTargetList())
				.IsNotEmpty()
				.Contains(detectable1, detectable2)
				.NotContains(undetectable);			

			// All targets go out of range == no targets.
			targetingNode.GlobalPosition = -FARTHEST_POSITION;
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			signalCollector.ClearCurrentTarget(); // Todo: hard test condition here
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentTarget).IsNull();
			AssertThat(targeting.GetTargetList()).IsEmpty();
		}
		/*
		Todo: 
			- Random targeting test?
		*/

	}
}
