
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
	// [Export] public Groups.GroupTypes StructureTypes = Groups.GroupTypes.Structure;

	protected HealthComponent _healthComponent;
	protected HurtComponent _hurtComponent;
	protected Groups.GroupTypes _types = Groups.GroupTypes.Structure;

	// Nodes
	[ExportGroup("Exported Child Nodes")]
	[Export] protected AnimatedSprite2D _animatedSprite2D;

    // public virtual void Initialize() { }

	public override void _Ready()
	{
		_healthComponent = GetComponentInChildrenOrNull<HealthComponent>(this);
		_hurtComponent = GetComponentInChildrenOrNull<HurtComponent>(this);
		if (_healthComponent == null || _hurtComponent == null)
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
