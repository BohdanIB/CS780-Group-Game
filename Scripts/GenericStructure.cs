
using Godot;

/// <summary>
/// Generic representation of a structure which exists on tiles.
/// 
/// Should have a redefined Initialize and ToString whenever a new structure is contrived.
/// </summary>
public partial class GenericStructure: Area2D
{

	// Components //
	[Export] protected HurtComponent _hurtComponent;
	[Export] protected HealthComponent _healthComponent;

	// Nodes
	[Export] protected CollisionShape2D _collisionShape2D;
	[Export] protected AnimatedSprite2D _animatedSprite2D;

	protected float _health;

    // public virtual void Initialize() { }

	public override void _Ready()
	{
		_hurtComponent.OnHurt += _healthComponent.ApplyDamage;
		_healthComponent.OnNoHealthLeft += () =>
		{
			GD.Print($"Structure {Name} died.");
			QueueFree();
		};
	}


}
