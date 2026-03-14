
using CS780GroupProject.Scripts.Utils;
using Godot;

/// <summary>
/// The ability for parent entity to hit other entities.
/// </summary>
public partial class HitComponent : Area2D
{
	[Signal] public delegate void OnHitEventHandler();

	[Export] private float _hitDamage = 1f;
	[Export] private Godot.Collections.Array<PackedScene> _hitableScenes = null;
	[Export] private Node _target;

	[Export] private CollisionShape2D _hitCollisionShape2D;

	public float Damage { get => _hitDamage; }

	/// <summary>
	/// Damage and potential target for hit.
	/// </summary>
	/// <param name="damage">Damage to be dealt OnHit</param>
	/// <param name="target">Target of hit. If null, then any Hurt-able entity can get hit.</param>
	public void Initialize(float damage, PackedScene[] validTargetGroups = null, Node target = null)
	{
		_hitDamage = damage;
		_hitableScenes = new(validTargetGroups);
		_target = target;
	}

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (this.CanHit(area))
			{
				GD.Print($"HitComponent made contact with {area.Name} and dealt {_hitDamage} damage. Emitting OnHit signal.");
				EmitSignal(SignalName.OnHit);
				QueueFree();
			}
		};
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyHitRadius(float newRadius)
	{
		((CircleShape2D)_hitCollisionShape2D.Shape).Radius = newRadius;
	}

	public bool CanHit(Node node)
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

		if (_target == null)
		{
			return SceneType.NodeSharesSceneType(node, _hitableScenes);
		}
		return node == _target;
	}

}
