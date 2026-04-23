using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using CS780GroupProject.Scripts.Utils;
using Godot;

public partial class ShooterComponent : Node2D
{
	[Signal] public delegate void OnShootEventHandler(HurtComponent Target, Projectile Projectile);

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes, _targetTypes;

	[ExportGroup("Exported Components")]
	[Export] private SpawnerComponent _projectileSpawner;
	[Export] private TargetingComponent _targeting;
	[Export] private Timer _shotCooldown;

	private ProjectileStats _projectileStats;
	private float
		_shotsPerSecond = -1, // How many shots are there per second? Human readability 
		_shotTime = -1; // The actual cooldown time between shots

	/// <summary>
	/// Just initialize shooter component
	/// </summary>
	/// <param name="fireRate"></param>
	/// <param name="entityTypes"></param>
	/// <param name="stats"></param>

	public void Initialize(float fireRate, Groups.GroupTypes entityTypes, Groups.GroupTypes targetTypes, ProjectileStats stats)
	{
		SetFireRate(fireRate);
		SetProjectileStats(stats);
		_thisEntityTypes = entityTypes;
		_targetTypes = targetTypes;

		_projectileSpawner ??= GetNodeOrNull<SpawnerComponent>("ProjectileSpawnerComponent");
		_targeting ??= GetNodeOrNull<TargetingComponent>("TargetingComponent");
		SubscribeToTargeting();
	}

	/// <summary>
	/// Initialize shooter and targeting component (which in-turn will initialize the detectable component associated with targeting component).
	/// </br></br>
	/// This is a convenience initialization for the shooter component pipeline
	/// </summary>
	/// <param name="range"></param>
	/// <param name="fireRate"></param>
	/// <param name="entityTypes"></param>
	/// <param name="targetTypes"></param>
	/// <param name="stats"></param>
	// public void Initialize(float range, float fireRate, Groups.GroupTypes entityTypes, Groups.GroupTypes targetTypes, ProjectileStats stats)
	// {
	// 	SetFireRate(fireRate);
	// 	SetProjectileStats(stats);
	// 	_thisEntityTypes = entityTypes;
	// 	_targeting.Initialize(range, entityTypes, targetTypes);
	// }


	public override void _Ready()
	{
		_projectileSpawner ??= GetNodeOrNull<SpawnerComponent>("ProjectileSpawnerComponent");
		_targeting ??= GetNodeOrNull<TargetingComponent>("TargetingComponent");
		
		//GD.Print($"ShooterComponent _Ready: spawner={_projectileSpawner}, targeting={_targeting}, cooldown={_shotCooldown}");


		if (_projectileSpawner == null || _targeting == null || _shotCooldown == null)
		{
			GD.PrintErr($"ShooterComponent {Name}: missing components (spawner={_projectileSpawner}, targeting={_targeting}, cooldown={_shotCooldown})");
			return;
		}

		

		SubscribeToTargeting();
	}

	private void SubscribeToTargeting()
	{
		if (_targeting == null) return;

		_targeting.OnTargetSelect -= OnTargetSelected;
		_targeting.OnTargetSelect += OnTargetSelected;
	}

	private void OnTargetSelected(DetectableComponent target)
	{
		//GD.Print($"OnTargetSelected fired, target={target}, cooldownStopped={_shotCooldown?.IsStopped()}, entityTypes={_thisEntityTypes}, stats={_projectileStats}");

		if (!_shotCooldown.IsStopped()) return;
		if (_thisEntityTypes == Groups.GroupTypes.None || _projectileStats == null)
		{
			GD.Print($"WARNING - Target selected by ShooterComponent {this}, but shooter is not properly initialized.");
			return;
		}

		_shotCooldown.Start(_shotTime);

		var projectile = _projectileSpawner.Spawn() as Projectile;
		_projectileSpawner.GetParent().AddChild(projectile);

		if (GetComponentInSiblingsOrNull<HurtComponent>(target) is var hurt)
		{
			// Initialize ONCE, immediately after adding to the tree
			projectile.Initialize(hurt, _projectileStats, _thisEntityTypes, _targetTypes);

			EmitSignal(SignalName.OnShoot, hurt, projectile);
		}
		else
		{
			GD.Print($"WARNING - Target selected by ShooterComponent {this}, but target does not have a hurt component?");
			projectile.QueueFree();
		}
	}

	public Groups.GroupTypes GetEntityTypes()
	{
		return _thisEntityTypes;
	}

	public float GetFireRate()
	{
		return _shotsPerSecond;
	}

	public ProjectileStats GetProjectileStats()
	{
		return _projectileStats;
	}

	public void SetProjectileStats(ProjectileStats stats)
	{
		_projectileStats = stats;
	}

	public void SetFireRate(float shotsPerSecond)
	{
		_shotsPerSecond = shotsPerSecond;
		_shotTime = 1 / shotsPerSecond;
	}
}
