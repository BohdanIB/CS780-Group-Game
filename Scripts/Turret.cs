using Godot;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public partial class Turret : Area2D
{
	// [Export] private TurretStats.Category _turretType = TurretStats.Category.Balista;
	[Export] private bool _disabled = false;
	[Export] private bool _targetClosest = true; // Todo: WIP Targeting priority. Closest or farthest should be changable, maybe random targeting also as an option
	[Export] private TurretStats _stats;

	// Scene Children
	private CollisionShape2D _collisionShape2D;
	private AnimatedSprite2D _sprite;
	private Timer _shotCooldownTimer;

	private List<PathFollower> _enemiesInRange = new();

	/// <summary>
	/// Initializes turret with custom stats.
	/// </summary>
	/// <param name="turretStats"></param>
	public void Initialize(TurretStats turretStats)
	{
		_stats = turretStats;
	}
	/// <summary>
	/// Initializes turret with "generic" base stats for given type.
	/// </summary>
	/// <param name="turretType"></param>
	public void Initialize(TurretStats.Category turretType)
	{
		Initialize(new TurretStats(turretType));
	}

	public override void _Ready()
	{
		// Some cases where _Ready gets called before Initialize, just set to some value for now and it will get reset later.
		if (_stats == null)
		{
			Initialize(TurretStats.Category.Ballista);
		}

		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		UpdateTurretRadius(_stats.AggroRadius);

		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		UpdateTurretSprite(_stats.SpriteFrame);
		// _sprite2D. // todo: Load proper tile from turret tilemap

		_shotCooldownTimer = GetNode<Timer>("ShotCooldownTimer");
		// _shotCooldownTimer.Timeout += () =>
		// {
		// 	GD.Print($"Turret {Name} ready to shoot again!");
		// };

		// Todo: Primarily to support ghost mode turret in TurretPlacer.
		if (_disabled) { return; }

		// AreaShapeEntered += (rid, area, areaShapeIndex, localShapeIndex) => {
		AreaEntered += (area) => {
			if (area is PathFollower pf)
			{
				GD.Print($"TURRET BODY ENTERED: {pf.Name}");
				_enemiesInRange.Add(pf);
			}
		};
		AreaExited += (area) => // todo: Case where path follower dies within area? Does it still send exit signal?
		{
			if (area is PathFollower pf)
			{
				GD.Print($"TURRET BODY EXITED: {pf.Name}");
				_enemiesInRange.Remove(pf);
			}
		};

		GD.Print($"Turret Stats: {_stats}");

	}

	public override void _PhysicsProcess(double delta)
	{
		// Todo: Primarily to support ghost mode turret in TurretPlacer.
		if (_disabled) { return; }

		// Check if something is in area of turret
		// If something is in area, start shooting at target (spawn Projectile?)
		//   Projectile should send signal to shot path follower when it gets hit (damage)
		// Obey firerate restrictions

		// Shoot at an enemy if there is one in range.
		// TODO: Should target enemies based on how far they are along path.
		if (_enemiesInRange.Count > 0 && _shotCooldownTimer.IsStopped())
		{
			PathFollower currTargetEnemy = _enemiesInRange[0];
			float currTargetDistance = Position.DistanceTo(currTargetEnemy.Position);
			for (int i = 1; i < _enemiesInRange.Count; i++)
			{
				var enemy = _enemiesInRange[i];
				float enemyDistance = Position.DistanceTo(enemy.Position);
				if (_targetClosest && enemyDistance < currTargetDistance)
				{
					currTargetEnemy = enemy;
					currTargetDistance = enemyDistance;
				}
				if (!_targetClosest && currTargetDistance < enemyDistance)
				{
					currTargetEnemy = enemy;
					currTargetDistance = enemyDistance;
				}
			}
			
			GD.Print($"Turret {Name} firing Projectile at target {currTargetEnemy} with stats: {_stats.ProjectileStats}");

			_shotCooldownTimer.Start(1 / _stats.FireRate);
			var projectileScene = GD.Load<PackedScene>("res://Scenes/Projectile.tscn");
			var projectile = projectileScene.Instantiate<Projectile>();
			
			projectile.GlobalPosition = GlobalPosition;
			projectile.Initialize(currTargetEnemy, _stats.ProjectileStats);
			GetTree().GetRoot().AddChild(projectile);
		}
	}

	// public void UpdateStats(TurretStats stats)
	// {

	// }

	public void UpdateTurretRadius(float newRadius)
	{
		_stats.AggroRadius = newRadius;
		((CircleShape2D)_collisionShape2D.Shape).Radius = newRadius; // TODO: Better way of doing this?
	}

	public void UpdateTurretSprite(int newFrame)
	{
		_stats.SpriteFrame = newFrame;
		_sprite.Frame = newFrame;
	}

	public override string ToString()
	{
		return $"{Name}: {_stats}";
	}

}
