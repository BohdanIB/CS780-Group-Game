
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;

public partial class Projectile : Node2D
{
	public const float MIN_TARGET_DISTANCE = 0.01f;

	[Signal] public delegate void OnProjectileImpactEventHandler(Vector2 Position, ProjectileStats Stats, Groups.GroupTypes SenderTypes); // todo: May need more dev; Explosive shots AOE?

	[Export] public ProjectileStats Stats;
	[Export] public Vector2 TargetLocation; // Either the target's last known position, or a position given at initialization.
	[Export] public Node2D Target;

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes, _senderTypes;

	[ExportGroup("Exported Components")]
	[Export] private HitComponent _hitComponent;

	private AnimatedSprite2D _sprite; // todo: export
	private bool _wasInitialized = false;

	/// <summary>
	/// Initialize generic projectile to target specific position.
	/// </summary>
	/// <param name="targetPosition"></param>
	/// <param name="category"></param>
	public void Initialize(Vector2 targetPosition, ProjectileStats.Category category, Groups.GroupTypes senderTypes)
	{
		Initialize(targetPosition, new ProjectileStats(category), senderTypes);
	}
	/// <summary>
	/// Initialize projectile to target specific position with specific stats.
	/// </summary>
	/// <param name="targetPosition"></param>
	/// <param name="projectileStats"></param>
	public void Initialize(Vector2 targetPosition, ProjectileStats projectileStats, Groups.GroupTypes senderTypes)
	{
		TargetLocation = targetPosition;
		Initialize(projectileStats, senderTypes);
	}

	/// <summary>
	/// Initialize generic projectile to target specific entity.
	/// </summary>
	/// <param name="targetEntity"></param>
	/// <param name="category"></param>
	public void Initialize(Node2D targetNode, ProjectileStats.Category category, Groups.GroupTypes senderTypes)
	{
		Initialize(targetNode, new ProjectileStats(category), senderTypes);
	}
	/// <summary>
	/// Initialize projectile to target specific entity with specific stats.
	/// </summary>
	/// <param name="targetEntity"></param>
	/// <param name="projectileStats"></param>
	public void Initialize(Node2D targetNode, ProjectileStats projectileStats, Groups.GroupTypes senderTypes)
	{
		Target = targetNode;
		if (!IsInstanceValid(Target))
		{
			GD.Print($"Projectile {Name} was initialized with target, but target no longer exists... Freeing projectile.");
			QueueFree(); // todo: Might not be proper to queue a free before the _Ready call?
			return;
		}
		TargetLocation = Target.GlobalPosition;
		Initialize(projectileStats, senderTypes);
	}

	/// <summary>
	/// Initialize projectile with specific stats.
	/// <br/>
	/// Last layer of initilization for any type of initialization for projectile.
	/// </summary>
	/// <param name="projectileStats"></param>
	private void Initialize(ProjectileStats projectileStats, Groups.GroupTypes senderTypes)
	{
		Stats = projectileStats;
		_senderTypes = senderTypes;
		_thisEntityTypes = _senderTypes | Groups.GroupTypes.Projectile;

		// _hitComponent.Initialize(_stats.Damage, _senderScene, _target, _targetScenes);
		_wasInitialized = true;
	}

	/*

	public override void _Ready()
	{
		// Debug
		if (_stats != null && !_wasInitialized)
		{
			if (_target != null && _senderScene != null)
			{
				Initialize(_target, _stats, _senderScene);
			}
			else if (_senderScene != null)
			{
				Initialize(_targetLocation, _stats, _senderScene);
			}
			else
			{
				GD.Print($"WARNING - Projectile {Name} could not initialized on _Ready with given values {{ Stats: {_stats}, Target: {_target}, SenderScene: {_senderScene}}}");
			}
		}

		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); // Todo: Nix this for export (or component?)
		_sprite.Frame = _stats.SpriteFrame;

		// _hitComponent.OnHit += (area, damage) =>
		// {
		// 	GD.Print($"PROJECTILE ONHIT: {area.Name} - Damage: {damage}");
		// 	ProjectileImpact();
		// };
	}

	public override void _PhysicsProcess(double delta)
	{
		// GD.Print($"Projectile stats: {_stats}");
		if (IsInstanceValid(_target))
		{
			_targetLocation = _target.GlobalPosition;
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
		EmitSignal(SignalName.OnProjectileImpact, Position, _stats, _senderScene);
		// if (IsInstanceValid(_target) && GetComponentOrNull<HurtComponent>(_target) is var hurt && IsInstanceValid(hurt))
		// {
		// 	// GD.Print($"Projectile hit target {_target.Name} for {_stats.Damage} damage");
		// 	// _target.ChangeHealth(_stats.Damage);
		// 	hurt.Hit(_hitComponent, _senderScene, _stats.Damage);
		// 	// TODO
		// }
		// GD.Print("Freeing Projectile.");
		QueueFree(); // TODO: FREEING AND DISCONNECTION OF SIGNALS? https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html
	}

	*/

}
