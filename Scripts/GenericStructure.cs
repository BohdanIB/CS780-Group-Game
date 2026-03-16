
using Godot;

/// <summary>
/// Generic representation of a structure which exists on tiles.
/// 
/// Should have a redefined Initialize and ToString whenever a new structure is contrived.
/// </summary>
public partial class GenericStructure: Node2D
{

	// Components //
	[ExportGroup("Exported Components")]
	[Export] protected HurtComponent _hurtComponent;
	[Export] protected HealthComponent _healthComponent;

	// Nodes
	[ExportGroup("Exported Child Nodes")]
	[Export] protected AnimatedSprite2D _animatedSprite2D;

	protected float _health;

    // public virtual void Initialize() { }

	public override void _Ready()
	{
		// _hurtComponent.OnHurt += (hitOwnerNode, damage) =>
		// {
		// 	_healthComponent.ApplyDamage(damage);
		// };
		// _healthComponent.OnNoHealthLeft += () =>
		// {
		// 	GD.Print($"Structure {Name} died.");
		// 	QueueFree();
		// };
	}


}
