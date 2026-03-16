
using Godot;

/// <summary>
/// The ability for parent entity to be hurt by other entities.
/// </summary>
public partial class HurtComponent : Area2D
{
	[Signal] public delegate void OnHurtEventHandler(Node HitterOwnerNode, Area2D HitterArea, float Damage);

	[Export] private SceneFilePathRes[] _allowedToHurtEntity = []; // todo?: Valid scenes for this component to be hurt by.

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _hurtCollisionShape2D;

	/// <summary>
	/// HitComponents can deal damage to hurt components
	/// </summary>
	/// <param name="hitterOwner"></param>
	/// <param name="damage"></param>
	public void Hit(Node hitterOwner, Area2D hitterArea, float damage)
	{
		GD.Print($"HurtComponent was hit by {hitterOwner.Name} and was dealt {damage} damage. Emitting OnHurt signal.");
		EmitSignal(SignalName.OnHurt, hitterOwner, hitterArea, damage);
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyHurtRadius(float newRadius)
	{
		((CircleShape2D)_hurtCollisionShape2D.Shape).Radius = newRadius;
	}

	// TODO: Similar check to HitComponent potentially?
	// public bool CanBeHurtBy(Node node)
	// {
	// 	return SceneFilePathRes.EntitySharesScenePath(node.Owner, _allowedToHurtEntity);
	// }
}
