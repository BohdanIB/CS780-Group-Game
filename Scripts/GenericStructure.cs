
using Godot;

/// <summary>
/// Generic representation of a structure which exists on tiles.
/// 
/// Should have a redefined Initialize and ToString whenever a new structure is contrived.
/// </summary>
public partial class GenericStructure: Area2D
{
	// Nodes
	[Export] protected CollisionShape2D _collisionShape2D;
	[Export] protected AnimatedSprite2D _animatedSprite2D;

	protected float _health;

	// public virtual void Initialize() { }

}
