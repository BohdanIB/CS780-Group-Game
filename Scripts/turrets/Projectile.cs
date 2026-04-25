
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;
using System.Collections.Generic;

public partial class Projectile : Node2D
{
	public const float MIN_TARGET_DISTANCE = 0.01f;

	[Signal] public delegate void OnProjectileImpactEventHandler(Vector2 Position, ProjectileStats Stats/*, Groups.GroupTypes SenderTypes*/); // todo: May need more dev; Explosive shots AOE?

	[Export] protected ProjectileStats _stats;
	[Export] protected Vector2 _targetLocation; // Either the target's last known position, or a position given at initialization.
	[Export] protected HurtComponent _target;

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes, _senderTypes;
	private Groups.GroupTypes _validHitableTypes;

	[ExportGroup("Components")]
	[Export] private HitComponent _hit;
	[Export] private AnimationComponent _animation;

	/// <summary>
	/// Initialize projectile to target specific position with specific stats.
	/// </summary>
	/// <param name="targetPosition"></param>
	/// <param name="projectileStats"></param>
	public void Initialize(Vector2 targetPosition, ProjectileStats projectileStats, Groups.GroupTypes senderTypes, Groups.GroupTypes hurtableTypes)
	{
		_targetLocation = targetPosition;
		Initialize(projectileStats, senderTypes, hurtableTypes);
	}

	/// <summary>
	/// Initialize projectile to target specific entity with specific stats.
	/// </summary>
	/// <param name="targetEntity"></param>
	/// <param name="projectileStats"></param>
	public void Initialize(HurtComponent targetNode, ProjectileStats projectileStats, Groups.GroupTypes senderTypes, Groups.GroupTypes hurtableTypes)
	{
		_target = targetNode;
		if (!IsInstanceValid(_target))
		{
			GD.Print($"Projectile {Name} was initialized with target, but target no longer exists... Freeing projectile.");
			QueueFree(); // todo: Might not be proper to queue a free before the _Ready call?
			return;
		}
		_targetLocation = _target.GlobalPosition;
		Initialize(projectileStats, senderTypes, hurtableTypes);
	}

	/// <summary>
	/// Initialize projectile with specific stats.
	/// <br/>
	/// Last layer of initilization for any type of initialization for projectile.
	/// </summary>
	/// <param name="projectileStats"></param>
	private void Initialize(ProjectileStats projectileStats, Groups.GroupTypes senderTypes, Groups.GroupTypes hurtableTypes)
	{
		_stats = projectileStats;
		_senderTypes = senderTypes;
		_thisEntityTypes = _senderTypes | Groups.GroupTypes.Projectile;
		_validHitableTypes = hurtableTypes;
		InitializeComponents();
		UpdateStats();
	}

	public override void _Ready()
	{
		_hit.OnHit += (area, damage) =>
		{
			GD.Print($"PROJECTILE ONHIT: {area.Name} - Damage: {damage}");
			ProjectileImpact();
		};

		// GD.Print($"Projectile stats: {_stats}");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsInstanceValid(_target))
		{
			_targetLocation = _target.GlobalPosition;
		}

		// Todo: Make movement a little easier to expand
		GlobalPosition = GlobalPosition.MoveToward(_targetLocation, (float)delta * _stats.Speed);
		switch (_stats.Type) {
			case ProjectileStats.Category.Bolt:
				LookAt(_targetLocation);
				RotationDegrees += 90f; // Rotate 90 degrees to the right to look at -Y instead of +X (see LookAt() spec)
				break;
			// case ProjectileStats.Category.Blade:
			// 	const float ROTATION_SPEED_DEGREES = 270f;
			// 	GlobalRotationDegrees += (float)delta * ROTATION_SPEED_DEGREES;
			// 	break;
			default:
				// GD.Print($"WARNING: Projectile type {_stats.Type} special movement is UNDEFINED.");
				break;
		}

		if (GlobalPosition.DistanceTo(_targetLocation) < MIN_TARGET_DISTANCE)
		{
			if (!IsInstanceValid(_target))
			{
				// GD.Print($"\tProjectile reached target's last known location without colliding with a specific target.");
				ProjectileImpact();
			}
			else
			{
				// GD.Print($"\tProjectile reached target's last known location without colliding with target AND THE TARGET STILL EXISTS. Impacting without damaging target.");
				ProjectileImpact();
			}
		}
	}

	private void ProjectileImpact()
	{
		var alreadyHit = new HashSet<HurtComponent>();
		// Todo: WIP - Potentially add animation or some other effects to projectile on impact? May want to incorporate signal somehow.
		if (_stats.AOERadius > 0f)
			ApplyAOEDamage(GlobalPosition, alreadyHit);
		if (_stats.ChainCount > 0)
			ApplyChainDamage(GlobalPosition, _stats.ChainCount, alreadyHit);
		if (_stats.Freezes && IsInstanceValid(_target))
		{
			var mover = GetComponentInSiblingsOrNull<MoverComponent>(_target);
			mover?.Freeze(_stats.FreezeDuration);
		}
		
		
		EmitSignal(SignalName.OnProjectileImpact, GlobalPosition, _stats);
		QueueFree();
	}

	public void UpdateStats(ProjectileStats newStats = null)
	{
		if (newStats != null)
		{
			_stats = newStats;
		}
		UpdateComponents();
	}
	private void InitializeComponents()
	{
		if (_stats != null)
		{
			_hit.Initialize(_stats.Hitbox, _stats.Damage, _senderTypes, _thisEntityTypes, _validHitableTypes, target: _target);
			_animation.Initialize(_stats.Animations);
		}
	}
	private void UpdateComponents()
	{
		if (_stats != null)
		{
			_hit.SetRadius(_stats.Hitbox);
			// Todo: Add more updates
			_animation.Animations = _stats.Animations;
		}
	}
	public ProjectileStats GetStats()
	{
		return _stats;
	}
	public HurtComponent GetTarget()
	{
		return _target;
	}
	
	private void ApplyAOEDamage(Vector2 position, HashSet<HurtComponent> alreadyHit)
	{
		foreach (Node node in GetTree().GetNodesInGroup(_validHitableTypes.ToString()))
		{
			if (node is not HurtComponent hurt || !IsInstanceValid(hurt))
			{
				continue;
			}
			float dist = position.DistanceTo(hurt.GlobalPosition);
			if (dist > _stats.AOERadius)
			{
				continue;
			}
			float falloff = 1f - Mathf.Clamp(dist / _stats.AOERadius, 0f, 1f);
			float aoeDamage = _stats.Damage * falloff;
			
			var health = GetComponentInSiblingsOrNull<HealthComponent>(hurt);
			health?.ApplyDamage(aoeDamage);
		}
	}

	private void ApplyChainDamage(Vector2 position, int chainsRemaining, HashSet<HurtComponent> alreadyHit, ProjectileStats stats = null) 
	{		
		stats ??= _stats;
		List<(HurtComponent hurt, float dist)> candidates = new();
		
		foreach (Node node in GetTree().GetNodesInGroup(_validHitableTypes.ToString()))
		{
			if (node is not HurtComponent hurt || !IsInstanceValid(hurt))
			{
				continue;
			}
			if (alreadyHit.Contains(hurt))
			{
				continue;
			}
			float dist = position.DistanceTo(hurt.GlobalPosition);
			if (dist > _stats.ChainRadius)
			{
				continue;
			}
			candidates.Add((hurt, dist));
		}
		candidates.Sort((a, b) => a.dist.CompareTo(b.dist));
		foreach (var (hurt,dist) in candidates)
		{
			float falloff = 1f - Mathf.Clamp(dist / stats.ChainRadius, 0f, 1f);
			float chainDamage = stats.Damage * falloff;
			
			var health = GetComponentInSiblingsOrNull<HealthComponent>(hurt);
			health?.ApplyDamage(chainDamage);
			alreadyHit.Add(hurt);
			
			if (chainsRemaining > 0) 
			{
				ApplyChainDamage(hurt.GlobalPosition, chainsRemaining - 1, alreadyHit, stats.WithChainFalloff());
				break;
			}
		}
	}

}
