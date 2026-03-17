
using Godot;

public partial class EnemyHit : Node2D
{
	[Export] private Enemy _enemy;
	[Export] private Projectile 
		_projectileTargeted,      // Specifically targets enemy, turret sender (should hit enemy)
		_projectileTargetScenes,  // No target, Enemy is part of target scenes, turret sender (should hit enemy)
		_projectileInvalidSender, // Target enemy, nemy is part of target scenes, enemy sender (should not hit enemy)
		_projectileNone,          // No target, no hitableScenes, turret sender (should not hit enemy)
		_projectileKiller;        // Targets enemy, turret sender (should kill enemy)

	public override void _Ready()
	{
		// _projectileTargeted.Dispose();
		// _projectileTargetScenes.Dispose();
		// _projectileInvalidSender.Dispose();
		// _projectileNone.Dispose();
		// _projectileKiller.Dispose();
	}

}
