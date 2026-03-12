
using CS780GroupProject.Scripts.Utils;
using Godot;

/// <summary>
/// The ability for parent entity to be hurt by other entities.
/// </summary>
public partial class HurtComponent : Area2D
{
	[Signal] public delegate void OnHurtEventHandler(float damage);

	[Export] private EntityGroups _entityGroup;

	[Export] private CollisionShape2D _hurtCollisionShape2D;

	public EntityGroups EntityGroup { get => _entityGroup; }

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (area is HitComponent hitComponent && hitComponent.IsTarget(this)) // todo
			{
				GD.Print($"HurtComponent made contact with {hitComponent.Name} and was dealt {hitComponent.Damage} damage. Emitting OnHurt signal.");
				EmitSignal(SignalName.OnHurt, hitComponent.Damage);
			}
		};
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyCollisionShapeRadius(float newRadius)
	{
		((CircleShape2D)_hurtCollisionShape2D.Shape).Radius = newRadius;
	}
}
