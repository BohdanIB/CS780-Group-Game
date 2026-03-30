
using CS780GroupProject.Scripts.Utils;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;
using Godot;
using System.Collections.Generic;

namespace TestNS
{
	/// <summary>
	/// Testing:
	/// <list type="bullet">
	/// <item>Initialization</item>
	/// <item>Valid hit basic</item>
	/// <item>Invalid hit basic</item>
	/// <item>Valid hit of specific scene, but invalid hurt</item>
	/// <item>Node with hit and hurt works properly (cannot detect itself also important!)</item>
	/// <item></item>
	/// </list>
	/// </summary>
	[TestSuite]
	[RequireGodotRuntime]
	public partial class HitHurtTestSuite
	{
		private partial class SignalCollector : Node
		{
			public List<(Node sender, float damage)> HitEnterList { get; } = new();
			public List<(Node sender, float damage)> Hit2EnterList { get; } = new();
			public List<(Node sender, float damage)> HurtEnterList { get; } = new();
			public List<(Node sender, float damage)> Hurt2EnterList { get; } = new();
			public List<Node> HitExitList { get; } = new();
			public List<Node> Hit2ExitList { get; } = new();
			public List<Node> HurtExitList { get; } = new();
			public List<Node> Hurt2ExitList { get; } = new();

			public SignalCollector(HitComponent hit, HurtComponent hurt, HitComponent hit2 = null, HurtComponent hurt2 = null)
			{
				ConnectComponents(hit, hurt, hit2, hurt2);
			}
			public void ConnectComponents(HitComponent hit, HurtComponent hurt, HitComponent hit2 = null, HurtComponent hurt2 = null)
			{
				hit.OnEnterHit += (hurt, damage) => {
					HitEnterList.Add((hurt, damage));
				};
				hit.OnExitHit += (hurt) =>
				{
					HitExitList.Add(hurt);
				};

				hurt.OnEnterHurt += (hit, damage) => {
					HurtEnterList.Add((hit, damage));
				};
				hurt.OnExitHurt += (hit) =>
				{
					HurtExitList.Add(hit);
				};

				if (hit2 != null)
				{
					hit2.OnEnterHit += (hurt, damage) => {
						Hit2EnterList.Add((hurt, damage));
					};
					hit2.OnExitHit += (hurt) =>
					{
						Hit2ExitList.Add(hurt);
					};
				}
				if (hurt2 != null)
				{
					hurt2.OnEnterHurt += (hit, damage) => {
						Hurt2EnterList.Add((hit, damage));
					};
					hurt2.OnExitHurt += (hit) =>
					{
						Hurt2ExitList.Add(hit);
					};
				}
			}
		}

		private ISceneRunner _runner = null;

		// Scene root
		private HitHurtTestScene _scene;
		// Nodes
		private HitComponent _hitComponent, _hitComponent2;
		private HurtComponent _hurtComponent, _hurtComponent2;


		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://Testing/Components/HitHurt/hit_hurt_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();
			AssertThat(_runner.Scene()).IsNotNull().IsInstanceOf<HitHurtTestScene>();

			_scene = _runner.Scene() as HitHurtTestScene;
			AssertThat(_scene.HitComponent).IsNotNull();
			AssertThat(_scene.HitComponent2).IsNotNull();
			AssertThat(_scene.HurtComponent).IsNotNull();
			AssertThat(_scene.HurtComponent2).IsNotNull();

			_hitComponent = _scene.HitComponent;
			_hitComponent2 = _scene.HitComponent2;
			_hurtComponent = _scene.HurtComponent;
			_hurtComponent2 = _scene.HurtComponent2;
		}

		[TestCase]
		[RequireGodotRuntime]
		public void HitInitialization_NoRange_NoTarget()
		{
			var hit = _hitComponent;
			var damage = 115;
			var sender = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var entity = Groups.GroupTypes.Projectile;
			var validHurtTypes = Groups.GroupTypes.Enemy;

			hit.Initialize(damage, sender, entity, validHurtTypes);
			AssertThat(hit.Damage).IsEqual(damage);
			AssertThat(hit.GetSenderTypes()).IsEqual(sender);
			AssertThat(hit.GetEntityTypes()).IsEqual(entity);
			AssertThat(hit.GetValidHurtableTypes()).IsEqual(validHurtTypes);
			AssertThat(hit.GetTarget()).IsNull();
		}
		[TestCase]
		[RequireGodotRuntime]
		public void HitInitialization_Range_NoTarget()
		{
			var hit = _hitComponent;
			var radius = 935;
			var damage = 115;
			var sender = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var entity = Groups.GroupTypes.Projectile;
			var validHurtTypes = Groups.GroupTypes.Enemy;

			hit.Initialize(radius, damage, sender, entity, validHurtTypes);
			AssertThat(hit.GetRadius()).IsEqual(radius);
			AssertThat(hit.Damage).IsEqual(damage);
			AssertThat(hit.GetSenderTypes()).IsEqual(sender);
			AssertThat(hit.GetEntityTypes()).IsEqual(entity);
			AssertThat(hit.GetValidHurtableTypes()).IsEqual(validHurtTypes);
			AssertThat(hit.GetTarget()).IsNull();
		}
		[TestCase]
		[RequireGodotRuntime]
		public void HitInitialization_NoRange_Target()
		{
			var hit = _hitComponent;
			var damage = 115;
			var sender = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var entity = Groups.GroupTypes.Projectile;
			var validHurtTypes = Groups.GroupTypes.Enemy;
			var target = _hurtComponent;

			hit.Initialize(damage, sender, entity, validHurtTypes, target);
			AssertThat(hit.Damage).IsEqual(damage);
			AssertThat(hit.GetSenderTypes()).IsEqual(sender);
			AssertThat(hit.GetEntityTypes()).IsEqual(entity);
			AssertThat(hit.GetValidHurtableTypes()).IsEqual(validHurtTypes);
			AssertThat(hit.GetTarget()).IsNotNull().IsEqual(target);
		}
		[TestCase]
		[RequireGodotRuntime]
		public void HitInitialization_Range_Target()
		{
			var hit = _hitComponent;
			var radius = 935;
			var damage = 115;
			var sender = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var entity = Groups.GroupTypes.Projectile;
			var validHurtTypes = Groups.GroupTypes.Enemy;
			var target = _hurtComponent;

			hit.Initialize(radius, damage, sender, entity, validHurtTypes, target);
			AssertThat(hit.GetRadius()).IsEqual(radius);
			AssertThat(hit.Damage).IsEqual(damage);
			AssertThat(hit.GetSenderTypes()).IsEqual(sender);
			AssertThat(hit.GetEntityTypes()).IsEqual(entity);
			AssertThat(hit.GetValidHurtableTypes()).IsEqual(validHurtTypes);
			AssertThat(hit.GetTarget()).IsNotNull().IsEqual(target);
		}

		[TestCase]
		[RequireGodotRuntime]
		public void HurtInitialization_NoRange()
		{
			var hurt = _hurtComponent;
			var entity = Groups.GroupTypes.Enemy;
			var validHitterTypes = Groups.GroupTypes.Turret | Groups.GroupTypes.Friendly;

			hurt.Initialize(entity, validHitterTypes);
			AssertThat(hurt.GetEntityTypes()).IsEqual(entity);
			AssertThat(hurt.GetValidHitterTypes()).IsEqual(validHitterTypes);
		}
		[TestCase]
		[RequireGodotRuntime]
		public void HurtInitialization_Range()
		{
			var hurt = _hurtComponent;
			var range = 935;
			var entity = Groups.GroupTypes.Enemy;
			var validHitterTypes = Groups.GroupTypes.Turret | Groups.GroupTypes.Friendly;

			hurt.Initialize(range, entity, validHitterTypes);
			AssertThat(hurt.GetRadius()).IsEqual(range);
			AssertThat(hurt.GetEntityTypes()).IsEqual(entity);
			AssertThat(hurt.GetValidHitterTypes()).IsEqual(validHitterTypes);
		}

		[TestCase]
		[RequireGodotRuntime]
		public async Task ValidHitBasic_NoTarget()
		{
			var hit = _hitComponent;
			var hurt = _hurtComponent;
			hit.GlobalPosition = new(50, 0);
			hurt.GlobalPosition = new(100, 0);

			var signalCollector = AutoFree(new SignalCollector(hit, hurt));
			_scene.AddChild(signalCollector);

			var radius = 10f;
			var damage = 1337;
			var hitSenderTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var hitEntityTypes = Groups.GroupTypes.Projectile;
			var hitValidHurtTypes = Groups.GroupTypes.Enemy;
			hit.Initialize(radius, damage, hitSenderTypes, hitEntityTypes, hitValidHurtTypes);
			
			var hurtEntityTypes = Groups.GroupTypes.Enemy;
			var hurtValidHitTypes = Groups.GroupTypes.Turret | Groups.GroupTypes.Projectile;
			hurt.Initialize(radius, hurtEntityTypes, hurtValidHitTypes);

			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitEnterList).IsEmpty();
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.HurtEnterList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();

			// move components into range
			hit.GlobalPosition = new(95, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitEnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hurt, damage)});
			AssertThat(signalCollector.HurtEnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hit, damage)});
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();

			// move components out of range
			hit.GlobalPosition = new(50, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitExitList)
				.HasSize(1)
				.Contains(hurt);
			AssertThat(signalCollector.HurtExitList)
				.HasSize(1)
				.Contains(hit);
			AssertThat(signalCollector.HurtEnterList).HasSize(1);
			AssertThat(signalCollector.HitEnterList).HasSize(1);
		}
		/// <summary>
		/// Expecting same result as NoTarget case
		/// </summary>
		/// <returns></returns>
		[TestCase]
		[RequireGodotRuntime]
		public async Task ValidHitBasic_Target()
		{
			var hit = _hitComponent;
			var hurt = _hurtComponent;
			hit.GlobalPosition = new(50, 0);
			hurt.GlobalPosition = new(100, 0);

			var signalCollector = AutoFree(new SignalCollector(hit, hurt));
			_scene.AddChild(signalCollector);

			var radius = 10f;
			var damage = 1337;
			var hitSenderTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var hitEntityTypes = Groups.GroupTypes.Projectile;
			var hitValidHurtTypes = Groups.GroupTypes.Enemy;
			var target = hurt;
			hit.Initialize(radius, damage, hitSenderTypes, hitEntityTypes, hitValidHurtTypes, target);
			
			var hurtEntityTypes = Groups.GroupTypes.Enemy;
			var hurtValidHitTypes = Groups.GroupTypes.Turret | Groups.GroupTypes.Projectile;
			hurt.Initialize(radius, hurtEntityTypes, hurtValidHitTypes);

			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitEnterList).IsEmpty();
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.HurtEnterList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();

			// move components into range
			hit.GlobalPosition = new(95, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitEnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hurt, damage)});
			AssertThat(signalCollector.HurtEnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hit, damage)});
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();

			// move components out of range
			hit.GlobalPosition = new(50, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitExitList)
				.HasSize(1)
				.Contains(hurt);
			AssertThat(signalCollector.HurtExitList)
				.HasSize(1)
				.Contains(hit);
			AssertThat(signalCollector.HurtEnterList).HasSize(1);
			AssertThat(signalCollector.HitEnterList).HasSize(1);
		}
		/// <summary>
		/// Only expecting hits for target even though both hurts are viable
		/// </summary>
		/// <returns></returns>
		[TestCase]
		[RequireGodotRuntime]
		public async Task MultipleValidHits_Target()
		{
			var hit = _hitComponent;
			var hurt = _hurtComponent;
			var hurt2 = _hurtComponent2; // Target
			
			hit.GlobalPosition = new(50, 0);
			hurt.GlobalPosition = new(100, 0);
			hurt2.GlobalPosition = new(100, 0);
			
			var signalCollector = AutoFree(new SignalCollector(hit, hurt, hurt2: hurt2));
			_scene.AddChild(signalCollector);

			var radius = 10f;
			var damage = 1337;
			var hitSenderTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var hitEntityTypes = Groups.GroupTypes.Projectile;
			var hitValidHurtTypes = Groups.GroupTypes.Enemy;
			hit.Initialize(radius, damage, hitSenderTypes, hitEntityTypes, hitValidHurtTypes, hurt2); // targeting hurt2
			
			var hurtEntityTypes = Groups.GroupTypes.Enemy;
			var hurtValidHitTypes = Groups.GroupTypes.Turret | Groups.GroupTypes.Projectile;
			hurt.Initialize(radius, hurtEntityTypes, hurtValidHitTypes);
			hurt2.Initialize(radius, hurtEntityTypes, hurtValidHitTypes);

			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitEnterList).IsEmpty();
			AssertThat(signalCollector.HurtEnterList).IsEmpty();
			AssertThat(signalCollector.Hurt2EnterList).IsEmpty();
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();
			AssertThat(signalCollector.Hurt2ExitList).IsEmpty();

			// hit into range of components (Should only hit target)
			hit.GlobalPosition = new(95, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitEnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hurt2, damage)});
			AssertThat(signalCollector.HurtEnterList).IsEmpty();
			AssertThat(signalCollector.Hurt2EnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hit, damage)});
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();
			AssertThat(signalCollector.Hurt2ExitList).IsEmpty();

			// move hit out of range
			hit.GlobalPosition = new(50, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitExitList)
				.HasSize(1)
				.Contains(hurt2);
			AssertThat(signalCollector.HurtExitList).IsEmpty();
			AssertThat(signalCollector.Hurt2ExitList)
				.HasSize(1)
				.Contains(hit);
			AssertThat(signalCollector.HitEnterList).HasSize(1);
			AssertThat(signalCollector.HurtEnterList).IsEmpty();
			AssertThat(signalCollector.Hurt2EnterList).HasSize(1);
		}

		/// <summary>
		/// Expecting hits on everything
		/// </summary>
		/// <returns></returns>
		[TestCase]
		[RequireGodotRuntime]
		public async Task MultipleValidHits_NoTarget()
		{
			var hit = _hitComponent;
			var hurt = _hurtComponent;
			var hurt2 = _hurtComponent2;
			
			hit.GlobalPosition = new(50, 0);
			hurt.GlobalPosition = new(100, 0);
			hurt2.GlobalPosition = new(100, 0);
			
			var signalCollector = AutoFree(new SignalCollector(hit, hurt, hurt2: hurt2));
			_scene.AddChild(signalCollector);

			var radius = 10f;
			var damage = 1337;
			var hitSenderTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var hitEntityTypes = Groups.GroupTypes.Projectile;
			var hitValidHurtTypes = Groups.GroupTypes.Enemy;
			hit.Initialize(radius, damage, hitSenderTypes, hitEntityTypes, hitValidHurtTypes);
			
			var hurtEntityTypes = Groups.GroupTypes.Enemy;
			var hurtValidHitTypes = Groups.GroupTypes.Turret | Groups.GroupTypes.Projectile;
			hurt.Initialize(radius, hurtEntityTypes, hurtValidHitTypes);
			hurt2.Initialize(radius, hurtEntityTypes, hurtValidHitTypes);

			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitEnterList).IsEmpty();
			AssertThat(signalCollector.HurtEnterList).IsEmpty();
			AssertThat(signalCollector.Hurt2EnterList).IsEmpty();
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();
			AssertThat(signalCollector.Hurt2ExitList).IsEmpty();

			// hit into range of components (Should only hit target)
			hit.GlobalPosition = new(95, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitEnterList)
				.HasSize(2)
				.Contains(new List<(Node, float)>(){(hurt, damage), (hurt2, damage)});
			AssertThat(signalCollector.HurtEnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hit, damage)});
			AssertThat(signalCollector.Hurt2EnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hit, damage)});
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();
			AssertThat(signalCollector.Hurt2ExitList).IsEmpty();

			// move hit out of range
			hit.GlobalPosition = new(50, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitExitList)
				.HasSize(2)
				.Contains(hurt, hurt2);
			AssertThat(signalCollector.HurtExitList)
				.HasSize(1)
				.Contains(hit);
			AssertThat(signalCollector.Hurt2ExitList)
				.HasSize(1)
				.ContainsSame(hit);
			AssertThat(signalCollector.HitEnterList).HasSize(2);
			AssertThat(signalCollector.HurtEnterList).HasSize(1);
			AssertThat(signalCollector.Hurt2EnterList).HasSize(1);
		}

		/// <summary>
		/// Expecting same output as if there was no target
		/// </summary>
		/// <returns></returns>
		[TestCase]
		[RequireGodotRuntime]
		public async Task MultipleValidHitters_Target()
		{
			var hurt = _hurtComponent;
			var hit = _hitComponent;
			var hit2 = _hitComponent2;

			hurt.GlobalPosition = new(50, 0);
			hit.GlobalPosition = new(100, 0);
			hit2.GlobalPosition = new(100, 0);
			
			var signalCollector = AutoFree(new SignalCollector(hit, hurt, hit2: hit2));
			_scene.AddChild(signalCollector);

			var radius = 10f;
			var hurtEntityTypes = Groups.GroupTypes.Enemy;
			var hurtValidHitTypes = Groups.GroupTypes.Turret | Groups.GroupTypes.Projectile;
			hurt.Initialize(radius, hurtEntityTypes, hurtValidHitTypes);

			var damage = 1337f;
			var damage2 = 7331f;
			var hitSenderTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var hitEntityTypes = Groups.GroupTypes.Projectile;
			var hitValidHurtTypes = Groups.GroupTypes.Enemy;
			var target = hurt;
			hit.Initialize(radius, damage, hitSenderTypes, hitEntityTypes, hitValidHurtTypes, target);
			hit2.Initialize(radius, damage2, hitSenderTypes, hitEntityTypes, hitValidHurtTypes, target);

			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HurtEnterList).IsEmpty();
			AssertThat(signalCollector.HitEnterList).IsEmpty();
			AssertThat(signalCollector.Hit2EnterList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.Hit2ExitList).IsEmpty();

			// hurt into range of components
			hurt.GlobalPosition = new(95, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HurtEnterList)
				.HasSize(2)
				.Contains(new List<(Node, float)>(){(hit, damage), (hit2, damage2)});
			AssertThat(signalCollector.HitEnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hurt, damage)});
			AssertThat(signalCollector.Hit2EnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hurt, damage2)});
			AssertThat(signalCollector.HurtExitList).IsEmpty();
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.Hit2ExitList).IsEmpty();

			// move hit out of range
			hurt.GlobalPosition = new(50, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HurtExitList)
				.HasSize(2)
				.Contains(hit, hit2);
			AssertThat(signalCollector.HitExitList)
				.HasSize(1)
				.Contains(hurt);
			AssertThat(signalCollector.Hit2ExitList)
				.HasSize(1)
				.ContainsSame(hurt);
			AssertThat(signalCollector.HurtEnterList).HasSize(2);
			AssertThat(signalCollector.HitEnterList).HasSize(1);
			AssertThat(signalCollector.Hit2EnterList).HasSize(1);
		}
		[TestCase]
		[RequireGodotRuntime]
		public async Task MultipleValidHitters_NoTarget()
		{
			var hurt = _hurtComponent;
			var hit = _hitComponent;
			var hit2 = _hitComponent2;
			
			hurt.GlobalPosition = new(50, 0);
			hit.GlobalPosition = new(100, 0);
			hit2.GlobalPosition = new(100, 0);
			
			var signalCollector = AutoFree(new SignalCollector(hit, hurt, hit2: hit2));
			_scene.AddChild(signalCollector);

			var radius = 10f;
			var hurtEntityTypes = Groups.GroupTypes.Enemy;
			var hurtValidHitTypes = Groups.GroupTypes.Turret | Groups.GroupTypes.Projectile;
			hurt.Initialize(radius, hurtEntityTypes, hurtValidHitTypes);

			var damage = 1337f;
			var damage2 = 7331f;
			var hitSenderTypes = Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
			var hitEntityTypes = Groups.GroupTypes.Projectile;
			var hitValidHurtTypes = Groups.GroupTypes.Enemy;
			hit.Initialize(radius, damage, hitSenderTypes, hitEntityTypes, hitValidHurtTypes);
			hit2.Initialize(radius, damage2, hitSenderTypes, hitEntityTypes, hitValidHurtTypes);

			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HurtEnterList).IsEmpty();
			AssertThat(signalCollector.HitEnterList).IsEmpty();
			AssertThat(signalCollector.Hit2EnterList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.Hit2ExitList).IsEmpty();

			// hurt into range of components
			hurt.GlobalPosition = new(95, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HurtEnterList)
				.HasSize(2)
				.Contains(new List<(Node, float)>(){(hit, damage), (hit2, damage2)});
			AssertThat(signalCollector.HitEnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hurt, damage)});
			AssertThat(signalCollector.Hit2EnterList)
				.HasSize(1)
				.Contains(new List<(Node, float)>(){(hurt, damage2)});
			AssertThat(signalCollector.HurtExitList).IsEmpty();
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.Hit2ExitList).IsEmpty();

			// move hit out of range
			hurt.GlobalPosition = new(50, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HurtExitList)
				.HasSize(2)
				.Contains(hit, hit2);
			AssertThat(signalCollector.HitExitList)
				.HasSize(1)
				.Contains(hurt);
			AssertThat(signalCollector.Hit2ExitList)
				.HasSize(1)
				.ContainsSame(hurt);
			AssertThat(signalCollector.HurtEnterList).HasSize(2);
			AssertThat(signalCollector.HitEnterList).HasSize(1);
			AssertThat(signalCollector.Hit2EnterList).HasSize(1);
		}
	}
}
