
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;
using Godot;
using System.Collections.Generic;
using System;

namespace TestNS
{
    /// <summary>
    /// Shooter test suite.
    /// </br>
    /// Note: Targeting for shooter is set to 'closest' for all tests. For targeting testing, 
    /// check targeting test suite
    /// </summary>
	[TestSuite]
    [RequireGodotRuntime]
    public partial class ShooterTestSuite : Node
    {
		private partial class SignalCollector : Node
		{
            public List<(Node2D target, Projectile projectile)> ShotList {get; private set;} = new();
			public (Node2D target, Projectile projectile) CurrentShot {get; private set;} = new(null, null);
			public SignalCollector(ShooterComponent shooter)
			{
				ConnectComponents(shooter);
			}
			public void ConnectComponents(ShooterComponent shooter)
			{
                shooter.OnShoot += (target, projectile) => {
                    var shot = (target, projectile);
                    ShotList.Add(shot);
                    CurrentShot = shot;
                };
			}
            public bool CurrentShotNull()
            {
                return CurrentShot.target == null && CurrentShot.projectile == null;
            }
            public void ClearSignalCollector()
            {
                ShotList.Clear();
                CurrentShot = new(null, null);
            }
		}

		private const uint GENERIC_WAIT_FRAMES = 4;
		private const float INTER_TARGET_DISTANCE = 10;
        Vector2 INTER_TARGET_VECTOR = new(INTER_TARGET_DISTANCE, 0);
        Vector2 TARGETING_POSITION = new(100, 0);
        Vector2 CLOSEST_POSITION, MIDDLE_POSITION, FARTHEST_POSITION;

		private ISceneRunner _runner = null;
		// Scene root
		private ShooterTestScene _scene;
		// Nodes
		private Node2D 
            _shooterNode, 
            _target1, _target2, _target3;
        // Convenience references
        private ShooterComponent _shooter;
        private SpawnerComponent _projectileSpawner;
        private TargetingComponent _targeting;
        private DetectorComponent _targetingDetector;

        private DetectableComponent _detectable1, _detectable2, _detectable3;
        private HealthComponent _health1, _health2, _health3;
        private MoverComponent _mover1, _mover2, _mover3;


		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://Testing/Components/Shooter/shooter_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();

			_scene = _runner.Scene() as ShooterTestScene;
			AssertThat(_scene).IsNotNull();

            _shooterNode = _scene.ShooterNode;
            _target1 = _scene.Target1;
            _target2 = _scene.Target2;
            _target3 = _scene.Target3;
            AssertThat(_shooterNode).IsNotNull();
            AssertThat(_target1).IsNotNull();
            AssertThat(_target2).IsNotNull();
            AssertThat(_target3).IsNotNull();

            // Shooter components
            _shooter = GetComponentInChildrenOrNull<ShooterComponent>(_shooterNode);
            _projectileSpawner = GetComponentInChildrenOrNull<SpawnerComponent>(_shooterNode);
            _targeting = GetComponentInChildrenOrNull<TargetingComponent>(_shooterNode);
            _targetingDetector = GetComponentInChildrenOrNull<DetectorComponent>(_shooterNode);
            AssertThat(_shooter).IsNotNull();
            AssertThat(_projectileSpawner).IsNotNull();
            AssertThat(_targeting).IsNotNull();
            AssertThat(_targetingDetector).IsNotNull();

            _targeting.TargetingStyle = TargetingMode.Close; // Targeting for shooter tests will always be 'close'.

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

            // Common dynamic variables
			TARGETING_POSITION = new(100, 0);
			CLOSEST_POSITION   = TARGETING_POSITION + INTER_TARGET_VECTOR;
			MIDDLE_POSITION    = CLOSEST_POSITION + INTER_TARGET_VECTOR;
			FARTHEST_POSITION  = MIDDLE_POSITION + INTER_TARGET_VECTOR;
		}

		[TestCase]
		[RequireGodotRuntime]
		public void Initialization()
		{
            var shooter = _shooter;
            var range = 20f;
            var fireRate = 10f;
            var shooterTypes = Groups.GroupTypes.Friendly | Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
            var shooterTargetTypes = Groups.GroupTypes.Enemy;
            var projectileStats = AutoFree(new ProjectileStats(ProjectileStats.Category.Bolt));
            shooter.Initialize(fireRate, shooterTypes, shooterTargetTypes, projectileStats);
            var targeting = _targeting;
            targeting.Initialize(TargetingMode.Close);
            var detector = _targetingDetector;
            detector.Initialize(range, shooterTypes, shooterTargetTypes);

            AssertThat(shooter.GetFireRate()).IsEqual(fireRate);
            AssertThat(shooter.GetEntityTypes()).IsEqual(shooterTypes);
            AssertThat(shooter.GetProjectileStats())
                .IsNotNull()
                .IsEqual(projectileStats);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task Shoot_Basic()
		{
            // shooter properly shoots target and OnShoot is correct
            // firerate is as expected
            // spawned projectiles have expected stats

            var shooterNode = _shooterNode;
            var target = _target1;
            var untargetable = _target2;

            shooterNode.GlobalPosition = -FARTHEST_POSITION;
            target.GlobalPosition = CLOSEST_POSITION;
            untargetable.GlobalPosition = FARTHEST_POSITION;

            // Shooter initialization
            var shooter = _shooter;
			var range = FARTHEST_POSITION.X - CLOSEST_POSITION.X + (INTER_TARGET_DISTANCE*2);
            var fireRate = 2f; // 2 shots per second
            var shooterTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret | Groups.GroupTypes.Friendly;
            var shooterTargetTypes = Groups.GroupTypes.Enemy;
            var projectileStats = AutoFree(new ProjectileStats(ProjectileStats.Category.Bolt));
            shooter.Initialize(fireRate, shooterTypes, shooterTargetTypes, projectileStats);
            var targeting = _targeting;
            targeting.Initialize(TargetingMode.Close);
            var detector = _targetingDetector;
            detector.Initialize(range, shooterTypes, shooterTargetTypes);

            // Target initialization
            var targetDetectable = _detectable1;
            var untargetableDetectable = _detectable2;
			var targetTypes = Groups.GroupTypes.Enemy;
			var targetValidTargetingEntitites = shooterTypes | Groups.GroupTypes.Projectile;
			targetDetectable.Initialize(targetTypes, targetValidTargetingEntitites);
			untargetableDetectable.Initialize(targetTypes, Groups.GroupTypes.None); // valid but undetectable

            SignalCollector signalCollector = AutoFree(new SignalCollector(shooter));
			
			// Check that no signals or anything have fired off
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.CurrentShotNull()).IsTrue();
			AssertThat(signalCollector.ShotList).IsEmpty();

			// Move shooter in range of targets, make sure proper target being shot at
			shooterNode.GlobalPosition = TARGETING_POSITION;
			await _runner.SimulateFrames(5, 50); // wait 0.25 seconds
			AssertThat(signalCollector.ShotList).HasSize(1); // Proper firerate 
            AssertThat(signalCollector.CurrentShot.target).IsEqual(targetDetectable);
            AssertThat(signalCollector.CurrentShot.projectile.GetStats()).IsEqual(projectileStats);
            AssertThat(signalCollector.CurrentShot.projectile.GetTarget()).IsEqual(targetDetectable);

            // Wait for another shot (expecting a second shot after 0.5 seconds of initial spotting)
			await _runner.SimulateFrames(5, 52);
			AssertThat(signalCollector.ShotList).HasSize(2); // Proper firerate 
            AssertThat(signalCollector.CurrentShot.target).IsEqual(targetDetectable);
            AssertThat(signalCollector.CurrentShot.projectile.GetStats()).IsEqual(projectileStats);
            AssertThat(signalCollector.CurrentShot.projectile.GetTarget()).IsEqual(targetDetectable);
			
			// shuffle target ordering and check shooting target does not change
			target.GlobalPosition = CLOSEST_POSITION;
			untargetable.GlobalPosition = MIDDLE_POSITION;
			await _runner.SimulateFrames(5, 105);
			AssertThat(signalCollector.ShotList).HasSize(3); // Proper firerate 
            AssertThat(signalCollector.CurrentShot.target).IsEqual(targetDetectable);
            AssertThat(signalCollector.CurrentShot.projectile.GetStats()).IsEqual(projectileStats);
            AssertThat(signalCollector.CurrentShot.projectile.GetTarget()).IsEqual(targetDetectable);

			// All targets go out of range == no targets.
			shooterNode.GlobalPosition = -FARTHEST_POSITION;
			await _runner.SimulateFrames(5, 150);
			signalCollector.ClearSignalCollector(); // Todo: hard test condition here
			await _runner.SimulateFrames(5, 200); // wait a second
			AssertThat(signalCollector.CurrentShotNull()).IsTrue();
			AssertThat(signalCollector.ShotList).IsEmpty();
		}

    }
}
