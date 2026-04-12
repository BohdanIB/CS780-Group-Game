
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;

/// <summary>
/// Generic representation of a structure which exists on tiles.
/// 
/// Should have a redefined Initialize and ToString whenever a new structure is contrived.
/// </summary>
public partial class GenericStructure: Node2D
{
	public const Groups.GroupTypes TYPES = Groups.GroupTypes.Structure;

	[ExportGroup("Components")]
	[Export] protected HealthComponent _health;
	[Export] protected HurtComponent _hurt;
	[Export] protected AnimationComponent _animation;

	public override void _Ready()
	{
		_health = GetComponentInChildrenOrNull<HealthComponent>(this);
		_hurt = GetComponentInChildrenOrNull<HurtComponent>(this);
		_animation = GetComponentInChildrenOrNull<AnimationComponent>(this); // todo: Multiple types could cause issues here
		if (_health == null || _hurt == null || _animation == null)
		{
			GD.Print($"WARNING - GenericStructure {this} was unable to find health and/or hurt components on _Ready()");
		}
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
