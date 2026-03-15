using Godot;

/// <summary>
/// TODO: For now, Projectiles always will hit their target and will move towards the target until
/// </summary>
public partial class Projectile : Node2D
{
	public const float MIN_TARGET_DISTANCE = 0.01f;

	// [Signal] public delegate void OnProjectileImpactEventHandler(Vector2 Position, ProjectileStats Stats);

	[Export] private ProjectileStats _stats;
	[Export] private Node2D _target;
	[Export] private HitComponent _hitComponent;

	private Vector2 _targetLastKnownLocation;
	private AnimatedSprite2D _sprite;

	/// <summary>
	/// Initializes projectile with custom stats.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="projectileStats"></param>
	public void Initialize(Node2D target, ProjectileStats projectileStats) // TODO: Give projectile a vector to target instead of target?
	{
		_target = target;
		if (!IsInstanceValid(_target))
		{
			GD.Print($"Projectile was instantiated, but target no longer exists... Freeing projectile.");
			QueueFree(); // todo: Might not be proper to queue a free before the _Ready call?
			return;
		}
		_targetLastKnownLocation = _target.Position;
		_stats = projectileStats;

		_hitComponent.Initialize(_stats.Damage, target: _target);
	}
	/// <summary>
	/// Initializes projectile with "generic" base stats for given type.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="projectileType"></param>
	public void Initialize(Node2D target, ProjectileStats.Category projectileType)
	{
		Initialize(target, new ProjectileStats(projectileType));
	}

	public override void _Ready()
	{
		Initialize(_target, _stats);

		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); // Todo: Nix this for export (or component?)
		_sprite.Frame = _stats.SpriteFrame;


		_hitComponent.OnHit += (hurtOwnerNode, damage) =>
		{
			ProjectileImpact();
		};

		// AreaEntered += (area) =>
		// {
		// 	if (area is HurtComponent hurtComponent && hurtComponent == _target)
		// 	{
		// 		ProjectileImpact();
		// 	}
		// };

		// GD.Print($"Projectile ready with stats: {_stats}");
	}

	public override void _PhysicsProcess(double delta)
	{
		// GD.Print($"Projectile stats: {_stats}");
		if (IsInstanceValid(_target))
		{
			_targetLastKnownLocation = _target.Position;
		}

		// Todo: Make movement a little easier to expand
		Position = Position.MoveToward(_targetLastKnownLocation, (float)delta * _stats.Speed);
		switch (_stats.Type) {
			case ProjectileStats.Category.Bolt:
				LookAt(_targetLastKnownLocation);
				RotationDegrees += 90f; // Rotate 90 degrees to the right to look at -Y instead of +X
				break;
			case ProjectileStats.Category.Blade:
				const float ROTATION_SPEED_DEGREES = 270f;
				GlobalRotationDegrees += (float)delta * ROTATION_SPEED_DEGREES;
				break;
			default:
				GD.Print($"WARNING: Projectile type {_stats.Type} special movement is UNDEFINED.");
				break;
		}

		if (Position.DistanceTo(_targetLastKnownLocation) < MIN_TARGET_DISTANCE)
		{
			if (!IsInstanceValid(_target))
			{
				GD.Print($"\tProjectile reached target's last known location without colliding with target.");
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
		// EmitSignal(SignalName.OnProjectileImpact, Position, _stats);
		// if (IsInstanceValid(_target))
		// {
		// 	// GD.Print($"Projectile hit target {_target.Name} for {_stats.Damage} damage");
		// 	// _target.ChangeHealth(_stats.Damage);
		// 	// TODO
		// }
		GD.Print("Freeing Projectile.");
		QueueFree(); // TODO: FREEING AND DISCONNECTION OF SIGNALS? https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html
	}

}
