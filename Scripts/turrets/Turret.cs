using Godot;
using CS780GroupProject.Scripts.Utils;

public partial class Turret : GenericStructure
{
	[Export] private TurretStats _stats;
	[Export] private TargetingMode _targetingMode = TargetingMode.Close;

	[ExportGroup("Types")]
	[Export] public Groups.GroupTypes _turretTypes = GenericStructure.TYPES | Groups.GroupTypes.Turret;
	[Export] public Groups.GroupTypes _targetTypes = Groups.GroupTypes.Enemy;

	[ExportGroup("Components")]
	[Export] private DetectorComponent _detector;
	[Export] private DetectableComponent _detectable;
	[Export] private ShooterComponent _shooter;
	[Export] private TargetingComponent _targeting;
	[Export] private SpawnerComponent _projectileSpawner;

	private bool _visibleTurretRadius = true;

	public void Initialize(TurretStats stats, TargetingMode targetingMode)
	{
		_stats = stats;
		_targetingMode = targetingMode;

		_detector ??= GetNodeOrNull<DetectorComponent>("DetectorComponent");
		_detectable ??= GetNodeOrNull<DetectableComponent>("DetectableComponent");
		_shooter ??= GetNodeOrNull<ShooterComponent>("ShooterComponent");
		_targeting ??= GetNodeOrNull<TargetingComponent>("ShooterComponent/TargetingComponent");
		_projectileSpawner ??= GetNodeOrNull<SpawnerComponent>("ShooterComponent/ProjectileSpawnerComponent");

		if (_targeting != null && _detector != null)
			_targeting.SetDetector(_detector);

		InitializeComponents();
		UpdateStats();
	}

	public override void _Ready()
	{
		base._Ready();

		_detector = GetNodeOrNull<DetectorComponent>("DetectorComponent");
		_detectable = GetNodeOrNull<DetectableComponent>("DetectableComponent");
		_shooter = GetNodeOrNull<ShooterComponent>("ShooterComponent");
		_targeting = GetNodeOrNull<TargetingComponent>("ShooterComponent/TargetingComponent");
		_projectileSpawner = GetNodeOrNull<SpawnerComponent>("ShooterComponent/ProjectileSpawnerComponent");

		if (_detector == null || _detectable == null || _shooter == null || _targeting == null || _projectileSpawner == null)
		{
			GD.PrintErr($"WARNING - Turret {Name} is missing one or more components.");
			return;
		}

		if (_targeting != null && _detector != null)
			_targeting.SetDetector(_detector);

		InitializeComponents();
		UpdateStats();

		_health.OnNoHealthLeft += () =>
		{
			GD.Print($"Turret {Name} destroyed.");
			QueueFree();
		};

		_hurt.OnHurt += (area, damage) =>
		{
			_health.ApplyDamage(damage);
		};

		_targeting.OnTargetSelect += (target) =>
		{
			if (target != null)
			{
				float angle = GlobalPosition.AngleToPoint(target.GlobalPosition);
				_animation.SetDirection(angle);
			}
		};

		_shooter.OnShoot += (target, projectile) =>
		{
			if (projectile != null)
				projectile.GlobalPosition = GlobalPosition;
		};
	}

	private void InitializeComponents()
	{
		if (_stats == null)
			return;

		_health.SetHealth(_stats.Health);
		_hurt.Initialize(_turretTypes, _targetTypes);
		_hurt.SetRadius(_stats.HitboxRadius);
		_detector.Initialize(_turretTypes, _targetTypes);
		_detector.SetRadius(_stats.AggroRadius);
		_detectable.Initialize(_turretTypes, _targetTypes);
		_detectable.SetRadius(_stats.DetectableRadius);
		_shooter.Initialize(_stats.FireRate, _turretTypes, _targetTypes, _stats.ProjectileStats);
		_targeting.TargetingStyle = _targetingMode;
		_animation.Initialize(_stats.Animations);
		QueueRedraw();
	}

	public void UpdateStats(TurretStats newStats = null)
	{
		if (newStats != null)
			_stats = newStats;

		if (_stats == null)
			return;

		_health.SetHealth(_stats.Health);
		_hurt.SetRadius(_stats.HitboxRadius);
		_detector.SetRadius(_stats.AggroRadius);
		_detectable.SetRadius(_stats.DetectableRadius);
		_shooter.SetProjectileStats(_stats.ProjectileStats);
		_targeting.TargetingStyle = _targetingMode;
		_animation.Animations = _stats.Animations;
		QueueRedraw();
	}

	public void UpdateTargetingMode(TargetingMode newMode)
	{
		_targetingMode = newMode;
		if (IsNodeReady())
			_targeting.TargetingStyle = newMode;
	}

	public void HideRadius()
	{
		_visibleTurretRadius = false;
		QueueRedraw();
	}

	public void DisableTurret()
	{
		if (_detector != null) _detector.SetRadius(0);
		if (_hurt != null) _hurt.SetRadius(0);
		if (_detectable != null) _detectable.SetRadius(0);
		if (_shooter != null) _shooter.SetProjectileStats(null);
	}

	public override void _Draw()
	{
		if (_visibleTurretRadius && _stats != null)
		{
			DrawCircle(Vector2.Zero, _stats.AggroRadius, new Color(0xff000020), filled: true);
		}
	}

	public override string ToString()
	{
		return $"{Name}: {_stats}";
	}
}