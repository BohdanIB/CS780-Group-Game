
using Godot;
using System.Diagnostics;

public partial class ShooterComponent : Node2D
{
	[Signal] public delegate void OnShootEventHandler();

	[Export] private SpawnerComponent _projectileSpawner;
	[Export] private TargetingComponent _targeting;
	[Export] private Timer _shotCooldown;

	private ProjectileStats _projectileStats;
	private float _shotsPerSecond = -1.0f;

	public override void _Ready()
	{
		Debug.Assert(IsInstanceValid(_projectileSpawner));
		Debug.Assert(IsInstanceValid(_targeting));
		Debug.Assert(IsInstanceValid(_shotCooldown));

		_targeting.OnTargetSelect += (target) =>
		{
			if (!_shotCooldown.IsStopped()) { return; }
			_shotCooldown.Start(_shotsPerSecond);
			// var projectile = _projectileScene.Instantiate<Projectile>();
			// projectile.GlobalPosition = GlobalPosition;
			// projectile.Initialize(currTargetEnemy, _stats.ProjectileStats);
			// GetTree().GetRoot().AddChild(projectile);
			var projectile = _projectileSpawner.Spawn() as Projectile;
			projectile.Initialize(target, _projectileStats, new SceneFilePathRes(Owner));
			GetTree().GetRoot().AddChild(projectile); // todo: Might not be right

		};
		// _projectileSpawner.OnSpawned += (node) =>
		// {
		// 	if (node is Projectile p)
		// 	{
		// 		p.Initialize()
		// 	}
		// 	// if (node is Node2D node2D)
		// 	// {
		// 	// 	node2D.
		// 	// }
		// };
	}

	public void SetProjectileStats(ProjectileStats stats)
	{
		_projectileStats = stats;
	}

	public void SetFireRate(float shotsPerSecond)
	{
		_shotsPerSecond = 1f / shotsPerSecond;
	}

}
