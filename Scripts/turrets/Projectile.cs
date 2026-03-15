using Godot;

/// <summary>
/// TODO: For now, Projectiles always will hit their target and will move towards the target until
/// </summary>
public partial class Projectile : Node2D
{
	public const float MIN_TARGET_DISTANCE = 0.01f;

	[Signal] public delegate void OnProjectileImpactEventHandler(Vector2 Position, ProjectileStats Stats); // todo: May need more dev; Explosive shots AOE?

	[Export] private ProjectileStats _stats;
	[Export] private Vector2 _targetLocation; // Either the target's last known position, or a position given at initialization.
	[Export] private Node2D _target;
	[Export] private SceneFilePathRes[] _targetScenes = []; // Valid scenes for this component to target.

	[ExportGroup("Exported Components")]
	[Export] private HitComponent _hitComponent;

	// private Vector2 _targetLastKnownLocation;
	private AnimatedSprite2D _sprite; // todo: export
	private bool _wasInitialized = false;


	/// <summary>
	/// Initialize generic projectile to target specific position.
	/// </summary>
	/// <param name="targetPosition"></param>
	/// <param name="category"></param>
	public void Initialize(Vector2 targetPosition, ProjectileStats.Category category)
	{
		Initialize(targetPosition, new ProjectileStats(category));
	}
	/// <summary>
	/// Initialize projectile to target specific position with specific stats.
	/// </summary>
	/// <param name="targetPosition"></param>
	/// <param name="projectileStats"></param>
	public void Initialize(Vector2 targetPosition, ProjectileStats projectileStats)
	{
		_targetLocation = targetPosition;
		Initialize(projectileStats);
	}

	/// <summary>
	/// Initialize generic projectile to target specific entity.
	/// </summary>
	/// <param name="targetEntity"></param>
	/// <param name="category"></param>
	public void Initialize(Node2D targetEntity, ProjectileStats.Category category)
	{
		Initialize(targetEntity, new ProjectileStats(category));
	}
	/// <summary>
	/// Initialize projectile to target specific entity with specific stats.
	/// </summary>
	/// <param name="targetEntity"></param>
	/// <param name="projectileStats"></param>
	public void Initialize(Node2D targetEntity, ProjectileStats projectileStats)
	{
		_target = targetEntity;
		if (!IsInstanceValid(_target))
		{
			GD.Print($"Projectile {Name} was initialized with target, but target no longer exists... Freeing projectile.");
			QueueFree(); // todo: Might not be proper to queue a free before the _Ready call?
			return;
		}
		_targetLocation = _target.Position;
		Initialize(projectileStats);
	}

	/// <summary>
	/// Initialize projectile with specific stats.
	/// <br/>
	/// Last layer of initilization for any type of initialization for projectile.
	/// </summary>
	/// <param name="projectileStats"></param>
	private void Initialize(ProjectileStats projectileStats)
	{
		_stats = projectileStats;
		_hitComponent.Initialize(_stats.Damage, _target, _targetScenes);
		_wasInitialized = true;
	}

	public override void _Ready()
	{
		// Debug
		if (_stats != null && !_wasInitialized)
		{
			if (_target != null)
			{
				Initialize(_target, _stats);
			}
			else
			{
				Initialize(_targetLocation, _stats);
			}
		}

		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); // Todo: Nix this for export (or component?)
		_sprite.Frame = _stats.SpriteFrame;

		_hitComponent.OnHit += (hurtOwnerNode, damage) =>
		{
			ProjectileImpact();
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		// GD.Print($"Projectile stats: {_stats}");
		if (IsInstanceValid(_target))
		{
			_targetLocation = _target.Position;
		}

		// Todo: Make movement a little easier to expand
		Position = Position.MoveToward(_targetLocation, (float)delta * _stats.Speed);
		switch (_stats.Type) {
			case ProjectileStats.Category.Bolt:
				LookAt(_targetLocation);
				RotationDegrees += 90f; // Rotate 90 degrees to the right to look at -Y instead of +X (see LookAt() spec)
				break;
			case ProjectileStats.Category.Blade:
				const float ROTATION_SPEED_DEGREES = 270f;
				GlobalRotationDegrees += (float)delta * ROTATION_SPEED_DEGREES;
				break;
			default:
				GD.Print($"WARNING: Projectile type {_stats.Type} special movement is UNDEFINED.");
				break;
		}

		if (Position.DistanceTo(_targetLocation) < MIN_TARGET_DISTANCE)
		{
			if (!IsInstanceValid(_target))
			{
				GD.Print($"\tProjectile reached target's last known location without colliding with a specific target.");
				ProjectileImpact();
			}
			else
			{
				GD.Print($"\tProjectile reached target's last known location without colliding with target AND THE TARGET STILL EXISTS. Impacting without damaging target.");
				ProjectileImpact();
			}
		}
	}

	private void ProjectileImpact()
	{
		// // Todo: WIP - Potentially add animation or some other effects to projectile on impact? May want to incorporate signal somehow.
		EmitSignal(SignalName.OnProjectileImpact, Position, _stats);
		// if (IsInstanceValid(_target))
		// {
		// 	// GD.Print($"Projectile hit target {_target.Name} for {_stats.Damage} damage");
		// 	// _target.ChangeHealth(_stats.Damage);
		// 	// TODO
		// }
		// GD.Print("Freeing Projectile.");
		QueueFree(); // TODO: FREEING AND DISCONNECTION OF SIGNALS? https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html
	}

}
