
using CS780GroupProject.Scripts.Utils;
using GdUnit4;
using GdUnit4.Asserts;
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
	public partial class HitHurt
	{
		private partial class SignalCollector : Node
		{
			public List<(Node sender, float damage)> HitEnterList { get; } = new();
			public List<(Node sender, float damage)> HurtEnterList { get; } = new();
			public List<Node> HitExitList { get; } = new();
			public List<Node> HurtExitList { get; } = new();

			public SignalCollector(HitComponent hit, HurtComponent hurt)
			{
				ConnectComponents(hit, hurt);
			}
			public void ConnectComponents(HitComponent hit, HurtComponent hurt)
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
			}
		}

		private ISceneRunner _runner = null;

		// Scene root
		private HitHurtTestScene _scene;
		// Parent nodes
		// private HitParent _hitParent;
		// private HurtParent _hurtParent;
		// private ComponentlessParent _componentlessParent;
		private HitComponent _hitComponent;
		private HurtComponent _hurtComponent;


		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://Testing/Components/HitHurt/hit_hurt_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();
			AssertThat(_runner.Scene()).IsNotNull().IsInstanceOf<HitHurtTestScene>();

			_scene = _runner.Scene() as HitHurtTestScene;
			AssertThat(_scene.HitComponent).IsNotNull();
			AssertThat(_scene.HurtComponent).IsNotNull();

			_hitComponent = _scene.HitComponent;
			_hurtComponent = _scene.HurtComponent;
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
			AssertThat(signalCollector.HitEnterList).HasSize(1);
			AssertThat(signalCollector.HitEnterList[0].sender).IsSame(hurt); // cannot overload tuple here alas
			AssertThat(signalCollector.HitEnterList[0].damage).IsEqual(damage);
			AssertThat(signalCollector.HurtEnterList).HasSize(1);
			AssertThat(signalCollector.HurtEnterList[0].sender).IsSame(hit);
			AssertThat(signalCollector.HurtEnterList[0].damage).IsEqual(damage);
			AssertThat(signalCollector.HitExitList).IsEmpty();
			AssertThat(signalCollector.HurtExitList).IsEmpty();

			// move components out of range
			hit.GlobalPosition = new(50, 0);
			await _runner.SimulateFrames(4);
			AssertThat(signalCollector.HitExitList)
				.HasSize(1)
				.ContainsSame(hurt);
			AssertThat(signalCollector.HurtExitList)
				.HasSize(1)
				.ContainsSame(hit);
			AssertThat(signalCollector.HurtEnterList).HasSize(1);
			AssertThat(signalCollector.HitEnterList).HasSize(1);
		}

	}
}
