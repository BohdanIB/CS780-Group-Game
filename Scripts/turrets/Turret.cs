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

    public override void _Ready()
    {
        base._Ready();

        if (_detector == null || _detectable == null || _shooter == null || _targeting == null || _projectileSpawner == null)
        {
            GD.PrintErr($"WARNING - Turret {Name} is missing one or more components.");
        }

        InitializeComponents();
        UpdateStats();

        // Death handling
        _health.OnNoHealthLeft += () =>
        {
            GD.Print($"Turret {Name} destroyed.");
            QueueFree();
        };

        // Damage handling
        _hurt.OnHurt += (area, damage) =>
        {
            _health.ApplyDamage(damage);
        };

        // Rotate sprite toward target
        _targeting.OnTargetSelect += (target) =>
        {
            if (target != null)
            {
                float angle = GlobalPosition.AngleToPoint(target.GlobalPosition);
                _animation.SetDirection(angle);
            }
        };

        // Shooting callback
        _shooter.OnShoot += () =>
        {
            var projectile = _projectileSpawner.SpawnProjectile();
            if (projectile != null)
            {
                projectile.GlobalPosition = GlobalPosition;
            }
        };
    }

    // Initialize all components using stats
    private void InitializeComponents()
    {
        if (_stats == null)
            return;

        _health.SetHealth(_stats.Health);
        _hurt.Initialize(_stats.HitboxRadius, _turretTypes, _targetTypes);
        _detector.Initialize(_stats.AggroRadius, _turretTypes, _targetTypes);
        _detectable.Initialize(_turretTypes, _targetTypes);
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
