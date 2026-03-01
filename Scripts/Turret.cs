using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 
/// </summary>
public partial class Turret : Area2D
{
	[Export] private TurretStats.Category _turretType = TurretStats.Category.Balista;
	[Export] private bool targetClosest = true; // Todo: WIP Targeting priority. Closest or farthest should be changable, maybe random targeting also as an option
	private TurretStats _turretStats;

	// Scene Children
	private CollisionShape2D _collisionShape2D;
	private Sprite2D _sprite2D;
	private Timer _shotCooldownTimer;

	// Book-keeping members
	private List<PathFollower> _enemiesInRange = new();
	private float _timeSinceLastFire = 0.0f;

	public override void _Ready()
	{
		_turretStats = new(_turretType);

		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		((CircleShape2D)_collisionShape2D.Shape).Radius = _turretStats.AggroRadius; // TODO: Better way of doing this?

		_sprite2D = GetNode<Sprite2D>("Sprite2D");
		// _sprite2D. // todo: Load proper tile from turret tilemap

		_shotCooldownTimer = GetNode<Timer>("ShotCooldownTimer");
		// _shotCooldownTimer.Timeout += () =>
		// {
		// 	GD.Print($"Turret {Name} ready to shoot again!");
		// };

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

		GD.Print($"Turret Stats: {_turretStats}");

	}

	public override void _PhysicsProcess(double delta)
	{

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
				if (targetClosest && enemyDistance < currTargetDistance)
				{
					currTargetEnemy = enemy;
					currTargetDistance = enemyDistance;
				}
				if (!targetClosest && currTargetDistance < enemyDistance)
				{
					currTargetEnemy = enemy;
					currTargetDistance = enemyDistance;
				}
			}
			
			GD.Print($"Turret {Name} firing Projectile at target {currTargetEnemy} with stats: Damage - {_turretStats.Damage}, Speed - {_turretStats.ProjectileSpeed}");

			_shotCooldownTimer.Start(1/_turretStats.FireRate);
			var ProjectileScene = GD.Load<PackedScene>("res://Scenes/Projectile.tscn");
			var Projectile = ProjectileScene.Instantiate<Projectile>();
			
			Projectile.GlobalPosition = GlobalPosition;
			Projectile.AssignTarget(currTargetEnemy, _turretStats.Damage, _turretStats.ProjectileSpeed);
			// Owner.AddChild(Projectile);
			GetTree().GetRoot().AddChild(Projectile);
		}
	}

}
