
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;

/// <summary>
/// 
/// </summary>
public partial class Turret : GenericStructure
{
	[Export] private TargetingMode _targetingMode = TargetingMode.Close;
	[Export] private TurretStats _stats;

	[ExportGroup("Types")]
	// [Export] public Groups.GroupTypes TurretTypes = Groups.GroupTypes.Friendly | Groups.GroupTypes.Structure | Groups.GroupTypes.Turret;
	[Export] public Groups.GroupTypes TurretTypes
	{
		get => _types;
		set
		{
			_types |= value;
		}
	}

	// Components
	private DetectorComponent _detector;
	private DetectableComponent _detectable;
	private ShooterComponent _shooter;
	private TargetingComponent _targeting;
	private SpawnerComponent _projectileSpawner;

	private bool _visibleTurretRadius = true; // todo

	/// <summary>
	/// Initializes generic turret.
	/// </summary>
	/// <param name="turretType"></param>
	public void Initialize(TurretStats.Category turretType)
	{
		Initialize(new TurretStats(turretType));
	}
	/// <summary>
	/// Initialize generic turret with specific targeting mode.
	/// </summary>
	/// <param name="turretType"></param>
	/// <param name="targetingMode"></param>
	public void Initialize(TurretStats.Category turretType, TargetingMode targetingMode)
	{
		_targetingMode = targetingMode;
		Initialize(turretType);
	}
	/// <summary>
	/// Initialize turret with specific stats
	/// </summary>
	/// <param name="turretStats"></param>
	public void Initialize(TurretStats turretStats)
	{
		UpdateStats(turretStats);
	}

	public override void _Ready()
	{
		base._Ready();

		_detector = GetComponentInChildrenOrNull<DetectorComponent>(this);
		_detectable = GetComponentInChildrenOrNull<DetectableComponent>(this);
		_shooter = GetComponentInChildrenOrNull<ShooterComponent>(this);
		_targeting = GetComponentInChildrenOrNull<TargetingComponent>(this);
		_projectileSpawner = GetComponentInChildrenOrNull<SpawnerComponent>(this);
	
		if (_detector == null || _detectable == null || _shooter == null || _targeting == null || _projectileSpawner == null)
		{
			GD.Print($"WARNING - Turret {this} was unable to find one of its components on _Ready()");
			return;
		}

		if (_stats != null)
		{
			Initialize(_stats);
		}

		// Component callbacks //

		_healthComponent.OnNoHealthLeft += () =>
		{
			GD.Print($"Turret {Name} died.");
			QueueFree();
		};

		_hurtComponent.OnEnterHurt += (area, damage) =>
		{
			_healthComponent.ApplyDamage(damage);
		};
		// _hurtComponent.OnExitHurt += (area) => {};

		// _shooter.OnShoot += (target, projectile) => {};

		// GD.Print($"Turret Stats: {_stats}");
	}

	public override void _Draw()
	{
		if (_visibleTurretRadius)
		{
			DrawCircle(Vector2.Zero, _stats.AggroRadius, new Color(0xff000020), filled: true);
		}
	}
	
	public void UpdateTargetingMode(TargetingMode newMode)
	{
		_targetingMode = newMode;
		if (this.IsNodeReady())
		{
			_targeting.TargetingStyle = newMode;
		}
	}

	/// <summary>
	/// Disabling turret also makes it undetectable. This is used for turret placer.
	/// </br>
	/// If turret disabling becomes a feature, then this function needs to get reworked.
	/// </summary>
	public void DisableTurret()
	{
		_detector.SetDetectableTypes(Groups.GroupTypes.None);
		_detectable.SetDetectorTypes(Groups.GroupTypes.None);
	}
	// public void UpdateDetectableEntities(Groups.GroupTypes detectableTypes)
	// {
	// 	_detector.SetDetectableTypes(detectableTypes);
	// }
	// public void UpdateAbleToDetectEntities(Groups.GroupTypes detectableTypes)
	// {
	// 	_detectable.SetDetectorTypes(detectableTypes);
	// }

	/// <summary>
	/// Replace current stats with newStats, then update all necessary components which react to stat changes.
	/// </summary>
	/// <param name="newStats"></param>
	public void UpdateStats(TurretStats newStats)
	{
		_stats = newStats;
		UpdateStats();
	}
	public void UpdateStats()
	{
		// These rely on components which need to be in the scene tree before they can be modified.
		if (this.IsNodeReady()) {
			UpdateHitboxRadius(_stats.HitboxRadius);
			UpdateDetectorRadius(_stats.AggroRadius);
			UpdateDetectableRadius(_stats.DetectableRadius);
			UpdateTargetingMode(_targetingMode);

			// Todo: Add more updates

			UpdateTurretHealth(_stats.Health);

			UpdateProjectileStats(_stats.ProjectileStats);
		}

		UpdateTurretSprite();
		// Redraw detector radius
		QueueRedraw();
	}
	protected void UpdateHitboxRadius(float newRadius)
	{
		_hurtComponent.SetRadius(newRadius);
	}
	protected void UpdateDetectorRadius(float newRadius)
	{
		_detector.SetRadius(newRadius);
	}
	protected void UpdateDetectableRadius(float newRadius)
	{
		_detectable.SetRadius(newRadius);
	}
	protected void UpdateTurretSprite()
	{
		_animatedSprite2D.Frame = _stats.SpriteFrame; // todo
	}
	protected void UpdateTurretHealth(float newHealth)
	{
		_healthComponent.SetHealth(newHealth);
	}
	protected void UpdateProjectileStats(ProjectileStats newStats)
	{
		_shooter.SetProjectileStats(newStats);
	}



	public override string ToString()
	{
		return $"{Name}: {_stats}";
	}

}
