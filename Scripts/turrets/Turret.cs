
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;

/// <summary>
/// 
/// </summary>
public partial class Turret : GenericStructure
{
	[Export] private TurretStats _stats;
	[Export] private TargetingMode _targetingMode = TargetingMode.Close;

	[ExportGroup("Types")]
	[Export] public Groups.GroupTypes _turretTypes = GenericStructure.TYPES | Groups.GroupTypes.Turret;
	[Export] public Groups.GroupTypes _targetTypes = Groups.GroupTypes.Enemy;
	// [Export] public Groups.GroupTypes _hurtTypes = Groups.GroupTypes.Enemy;
	// [Export] public Groups.GroupTypes _detectorTypes = Groups.GroupTypes.Enemy;
	// [Export] public Groups.GroupTypes _detectableTypes = Groups.GroupTypes.Enemy;
	

	[ExportGroup("Components")]
	[Export] protected DetectorComponent _detector;
	[Export] protected DetectableComponent _detectable;
	[Export] protected ShooterComponent _shooter;
	[Export] protected TargetingComponent _targeting;
	[Export] protected SpawnerComponent _projectileSpawner;

	private bool _visibleTurretRadius = true; // todo

	/// <summary>
	/// Initialize turret with specific stats
	/// </summary>
	/// <param name="stats"></param>
	/// <param name="targetingMode"></param>
	public void Initialize(TurretStats stats, TargetingMode targetingMode = TargetingMode.Close)
	{
		_targetingMode = targetingMode;
		_stats = stats;
		InitializeComponents();
		UpdateStats();
	}

	public override void _Ready()
	{
		base._Ready();

		if (_detector == null || _detectable == null || _shooter == null || _targeting == null || _projectileSpawner == null)
		{
			GD.Print($"WARNING - Turret {this} was unable to find one of its components on _Ready()");
		}

		// Component callbacks //

		_health.OnNoHealthLeft += () =>
		{
			GD.Print($"Turret {Name} died.");
			QueueFree();
		};

		_hurt.OnHurt += (area, damage) =>
		{
			_health.ApplyDamage(damage);
		};

		// Update sprite to aim at target's direction
		_targeting.OnTargetSelect += (target) =>
		{
			var directionRads = GlobalPosition.AngleToPoint(target.GlobalPosition);
			// _animation.SetState(AnimationPackEntry.State.Idle, directionRads); // TODO: Update when new animations roll out
			_animation.SetDirection(directionRads);
		};

		// GD.Print($"Turret Stats: {_stats}");
	}

	// // Change sprite to turn towards target
	// _idleAnimations.SetDirection(GlobalPosition, currTargetEnemy.GlobalPosition);

	public override void _Draw()
	{
		if (_visibleTurretRadius && _stats != null)
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
		_turretTypes = Groups.GroupTypes.None;
		_targetTypes = Groups.GroupTypes.None;
		_detector.SetDetectableTypes(Groups.GroupTypes.None);
		_detectable.SetDetectorTypes(Groups.GroupTypes.None);
	}

	/// <summary>
	/// Replace current stats with newStats, then update all necessary components which react to stat changes.
	/// </summary>
	/// <param name="newStats"></param>
	public void UpdateStats(TurretStats newStats = null)
	{
		if (newStats != null)
		{
			_stats = newStats;
		}
		UpdateComponents();
	}

	private void InitializeComponents()
	{
		// These rely on components which need to be in the scene tree before they can be modified.
		if (_stats != null) {
			// Todo: Add more updates

			// _health.Initialize();
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.Initialize(_stats.HitboxRadius, _turretTypes, _targetTypes);
			_detector.Initialize(_stats.AggroRadius, _turretTypes, _targetTypes);
			_detectable.Initialize(_turretTypes, _targetTypes); // todo: radius
			_shooter.Initialize(_stats.FireRate, _turretTypes, _targetTypes, _stats.ProjectileStats);
			_targeting.TargetingStyle = _targetingMode;
			_animation.Initialize(_stats.Animations);

			QueueRedraw(); // Draw aggro radius
		}
	}
	private void UpdateComponents()
	{
		if (_stats != null)
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.SetRadius(_stats.HitboxRadius);
			_detector.SetRadius(_stats.AggroRadius);
			_detectable.SetRadius(_stats.DetectableRadius);
			_shooter.SetProjectileStats(_stats.ProjectileStats);
			_targeting.TargetingStyle = _targetingMode;
			_animation.Animations = _stats.Animations;
			QueueRedraw(); // Draw aggro radius
		}
	}

	public override string ToString()
	{
		return $"{Name}: {_stats}";
	}

}
