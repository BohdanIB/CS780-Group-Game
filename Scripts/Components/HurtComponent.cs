
using CS780GroupProject.Scripts.Utils;
using Godot;

/// <summary>
/// The ability for parent entity to be hurt by other entities.
/// </summary>
public partial class HurtComponent : Area2D
{
	[Signal] public delegate void OnHurtEventHandler(float damage);

	[Export] private Godot.Collections.Array<PackedScene> _allowedToHurtEntity = null;

	[Export] private CollisionShape2D _hurtCollisionShape2D;

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (this.CanBeHurtBy(area))
			{
				if (area is HitComponent h)
				{
					GD.Print($"HurtComponent made contact with {h.Name} and was dealt {h.Damage} damage. Emitting OnHurt signal.");
					EmitSignal(SignalName.OnHurt, h.Damage);
				}
				else // TODO: This is not super extendable, but no idea how we should properly design this.
				{
					GD.Print($"WARNING - HurtComponent '{Name}' can be hurt by '{area.Name}', but '{area.Name}' is not a known component type. Emitting OnHurt signal with no damage applied...");
					EmitSignal(SignalName.OnHurt, 0.0f);
				}
			}
		};
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyHurtRadius(float newRadius)
	{
		((CircleShape2D)_hurtCollisionShape2D.Shape).Radius = newRadius;
	}

	public bool CanBeHurtBy(Node node)
	{
		return SceneType.NodeSharesSceneType(node, _allowedToHurtEntity);
	}
}
