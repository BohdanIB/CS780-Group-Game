using Godot;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public partial class Turret : Area2D
{

	public enum TurretType
	{
		Balista,
		Blade,
	};
	private static readonly Dictionary<TurretType, Vector2I> TURRET_ATLAS_COORDS = new()
	{
		{TurretType.Balista, new Vector2I(0, 0)},
		{TurretType.Blade,   new Vector2I(1, 0)},
	};

	[Export] private TurretType _turretType = TurretType.Balista;

	private CollisionShape2D _collisionShape2D;
	private Sprite2D _sprite2D;

	public override void _Ready()
	{
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		_sprite2D = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Process(double delta)
	{
		// TODO: Turret is just in charge of firing at path followers for now.
	}

	public static Vector2I GetAtlasCoordinates(TurretType type)
	{
		return TURRET_ATLAS_COORDS[type];
	}

	public Vector2I GetAtlasCoordinates()
	{
		return GetAtlasCoordinates(_turretType);
	}

}
