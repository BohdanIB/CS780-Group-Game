using Godot;

/// <summary>
/// 
/// </summary>
public partial class Turret : Area2D
{
	[Export] private TurretStats.Category _turretType = TurretStats.Category.Balista;
	private TurretStats _turretStats;

	private CollisionShape2D _collisionShape2D;
	private Sprite2D _sprite2D;

	private float _timeSinceLastFire = 0.0f;

	public override void _Ready()
	{
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		_sprite2D = GetNode<Sprite2D>("Sprite2D");

		_turretStats = new(_turretType);

		// _sprite2D. // todo: Load proper tile from turret tilemap
		((CircleShape2D)_collisionShape2D.Shape).Radius = _turretStats.AggroRadius; // TODO: Better way of doing this?


		// Todo: Now setup signals for on_body_enter for path follower shooting

	}

	public override void _Process(double delta)
	{
		// TODO: Turret is just in charge of firing at path followers for now.
		// if ()
	}
	

}
