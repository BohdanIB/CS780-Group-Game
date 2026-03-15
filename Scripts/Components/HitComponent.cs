
using Godot;

/// <summary>
/// The ability for parent entity to hit other entities.
/// 
/// TODO: This component is unfortuantely coupled with HurtComponent. Not sure about a good solution to this coupling.
/// </summary>
public partial class HitComponent : Area2D
{
	[Signal] public delegate void OnHitEventHandler(Node hurtOwnerNode, float damage);

	[Export] private float _hitDamage = 1f;
	[Export] private SceneFilePathRes[] _hitableScenes = []; // Valid scenes for this component to be able to hit.
	[Export] private Node _target;

	[Export] private CollisionShape2D _hitCollisionShape2D;

	public float Damage { get => _hitDamage; }

	/// <summary>
	/// Damage and potential target for hit. If there is no specific target to hit, then a general set of hitable targets can be defined.
	/// </summary>
	/// <param name="damage"></param>
	/// <param name="validTargetGroups"></param>
	/// <param name="target"></param>
	public void Initialize(float damage, SceneFilePathRes[] validTargetGroups = null, Node target = null)
	{
		_hitDamage = damage;
		_hitableScenes = validTargetGroups;
		_target = target;
	}

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			// if (area is HurtComponent h)
			// {
			// 	GD.Print("Entered hurt component.");
			// 	return;
			// }

			if (HitableHurtComponent(area) is var hurtComponent && hurtComponent != null)
			{
				GD.Print($"HitComponent made contact with {area.Owner.Name} and is attempting to deal {_hitDamage} damage. Emitting OnHit signal.");
				hurtComponent.Hit(this.Owner, _hitDamage);
				EmitSignal(SignalName.OnHit, area.Owner, _hitDamage);
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
		// if (_target != null && _target == hurtComponent) { return true; } // This hurt component is the target.
		// if (_target == null && _validTargetTypes == null) { return true; }                   // Hit any hurt component.
		// if (_targetParentNode == null) { return false; }                  // No specified parent node to compare against 

		// // Check if given area is valid to hit.
		// Type parentNodeType = _.GetType();
		// foreach (var type in _validTargetTypes)
		// {
		// 	if (areaType == type)
		// 	{
		// 		return true;
		// 	}
		// } 
		// return false;

		// return _target == null || _target == hurtComponent;

		// if (_target == null && _validTargetGroups == EntityGroups.None) { return false; }
		// else if (_target == null)
		// {
		// 	return hurtComponent.EntityGroup 
		// }


		// if (_target != null)
		// {
		// 	return hurtComponent == _target;
		// }
		// // Check if hurtComponent's EntityGroup is contained within valid target groups.
		// return (_validTargetGroups & hurtComponent.EntityGroup) != EntityGroups.None;


		// if (_target == null)
		// {
		// 	return SceneType.EntitySharesSceneType(node.Owner, _hitableScenes);
		// }
		// return node == _target;


		if (area is HurtComponent hurt) // Todo: Unfortunate coupling, but can't wizard up a better way without getting godot layers or groups involved.
		{
			GD.Print("AREA IS HURT COMPONENT.");
			// return hurt;
			Node entity = hurt.Owner;
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
