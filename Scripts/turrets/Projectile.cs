using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;

public partial class Projectile : Node2D
{
	public const float MIN_TARGET_DISTANCE = 0.01f;

	[Signal] public delegate void OnProjectileImpactEventHandler(Vector2 Position, ProjectileStats Stats);

	[Export] public ProjectileStats _stats;
	[Export] public Vector2 _targetLocation;
	[Export] public HurtComponent _target;

	[ExportGroup("Group Types")]
	[Export] public Groups.GroupTypes _thisEntityTypes, _senderTypes;
	private Groups.GroupTypes _validHitableTypes;

	[ExportGroup("Components")]
	[Export] public HitComponent _hit;
	[Export] public AnimationComponent _animation;

	// -------------------------------------------------------------------------
	// INITIALIZATION
	// -------------------------------------------------------------------------

	public void Initialize(Vector2 targetPosition, ProjectileStats projectileStats,
						   Groups.GroupTypes senderTypes, Groups.GroupTypes hurtableTypes)
	{
		_targetLocation = targetPosition;
		Initialize(projectileStats, senderTypes, hurtableTypes);
	}

	public void Initialize(HurtComponent targetNode, ProjectileStats projectileStats,
						   Groups.GroupTypes senderTypes, Groups.GroupTypes hurtableTypes)
	{
		_target = targetNode;

		if (!IsInstanceValid(_target))
		{
			GD.Print($"Projectile {Name} initialized with invalid target. Freeing.");
			QueueFree();
			return;
		}

		_targetLocation = _target.GlobalPosition;
		Initialize(projectileStats, senderTypes, hurtableTypes);
	}

	private void Initialize(ProjectileStats projectileStats,
							Groups.GroupTypes senderTypes, Groups.GroupTypes hurtableTypes)
	{
		_stats = projectileStats;
		_senderTypes = senderTypes;
		_thisEntityTypes = _senderTypes | Groups.GroupTypes.Projectile;
		_validHitableTypes = hurtableTypes;

		InitializeComponents();
		UpdateStats();
	}

	// -------------------------------------------------------------------------
	// READY
	// -------------------------------------------------------------------------

	public override void _Ready()
	{
		// Leave empty — components are not assigned until Initialize() is called.
	}

	// -------------------------------------------------------------------------
	// PHYSICS
	// -------------------------------------------------------------------------

	public override void _PhysicsProcess(double delta)
	{
		if (IsInstanceValid(_target))
			_targetLocation = _target.GlobalPosition;

		GlobalPosition = GlobalPosition.MoveToward(_targetLocation, (float)delta * _stats.Speed);

		switch (_stats.Type)
		{
			case ProjectileStats.Category.Bolt:
				LookAt(_targetLocation);
				RotationDegrees += 90f;
				break;

			default:
				break;
		}

		if (GlobalPosition.DistanceTo(_targetLocation) < MIN_TARGET_DISTANCE)
		{
			ProjectileImpact();
		}
	}

	// -------------------------------------------------------------------------
	// IMPACT
	// -------------------------------------------------------------------------

	private void ProjectileImpact()
	{
		EmitSignal(SignalName.OnProjectileImpact, GlobalPosition, _stats);
		QueueFree();
	}

	// -------------------------------------------------------------------------
	// COMPONENT INITIALIZATION
	// -------------------------------------------------------------------------

	private void InitializeComponents()
	{
		if (_stats == null)
			return;

		_hit.Initialize(_stats.Hitbox, _stats.Damage, _senderTypes, _thisEntityTypes, _validHitableTypes, target: _target);
		_animation.Initialize(_stats.Animations);

		_hit.OnHit += (area, damage) =>
		{
			GD.Print($"PROJECTILE ONHIT: {area.Name}");
			ProjectileImpact();
		};
	}

	private void UpdateComponents()
	{
		if (_stats == null)
			return;

		_hit.SetRadius(_stats.Hitbox);
		_animation.Animations = _stats.Animations;
	}

	// -------------------------------------------------------------------------
	// STATS UPDATE
	// -------------------------------------------------------------------------

	public void UpdateStats(ProjectileStats newStats = null)
	{
		if (newStats != null)
			_stats = newStats;

		UpdateComponents();
	}

	// -------------------------------------------------------------------------
	// GETTERS
	// -------------------------------------------------------------------------

	public ProjectileStats GetStats() => _stats;
	public HurtComponent GetTarget() => _target;
}
