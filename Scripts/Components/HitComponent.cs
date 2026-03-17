
using Godot;

/// <summary>
/// The ability for parent entity to hit other entities.
/// 
/// TODO: This component is unfortuantely coupled with HurtComponent. Not sure about a good solution to this coupling.
/// </summary>
public partial class HitComponent : Area2D
{
	[Signal] public delegate void OnHitEventHandler(Node HurtOwnerNode, Area2D HurtArea, float Damage);

	[Export] private float _hitDamage = 1f;
	[Export] private Node _target;
	[Export] private SceneFilePathRes[] _hitableScenes = []; // Valid scenes for this component to be able to hit.
	[Export] private SceneFilePathRes _senderScene;

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _hitCollisionShape2D;

	public float Damage { get => _hitDamage; }

	/// <summary>
	/// Damage and potential target for hit. If there is no specific target to hit, then a general set of hitable targets can be defined.
	/// </summary>
	/// <param name="damage"></param>
	/// <param name="validTargetGroups"></param>
	/// <param name="target"></param>
	public void Initialize(float damage, SceneFilePathRes sender, Node target = null, SceneFilePathRes[] validTargetGroups = null)
	{
		_hitDamage = damage;
		_senderScene = sender;
		_target = target;
		if (validTargetGroups != null)
		{
			_hitableScenes = validTargetGroups;
		}
		if (_target == null && _hitableScenes.Length <= 0)
		{
			GD.Print($"WARNING - HitComponent for {Owner.Name}: No target or hitable scenes assigned!");
		}
	}

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (HitableHurtComponent(area) is var hurtComponent && hurtComponent != null)
			{
				GD.Print($"HitComponent made contact with {area.Owner.Name} and is attempting to deal {_hitDamage} damage. Emitting OnHit signal.");
				hurtComponent.Hit(Owner, this, _senderScene, _hitDamage);
				EmitSignal(SignalName.OnHit, area.Owner, area, _hitDamage);
			}
		};
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyHitRadius(float newRadius)
	{
		((CircleShape2D)_hitCollisionShape2D.Shape).Radius = newRadius;
	}

	public HurtComponent HitableHurtComponent(Area2D area)
	{
		if (area is HurtComponent hurt) // Todo: Unfortunate coupling, but can't wizard up a better way without getting godot layers or groups involved.
		{
			// GD.Print("AREA IS HURT COMPONENT.");
			var entity = hurt.Owner;
			// Going for specific target
			if (_target != null && _target == entity)
			{
				return hurt;
			}
			// Check if the owner scene for area is hitable by this component.
			else if (SceneFilePathRes.EntitySharesScenePath(entity, _hitableScenes))
			{
				return hurt;
			}
		}
		return null;
	}

}
