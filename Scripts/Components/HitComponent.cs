
using CS780GroupProject.Scripts.Utils;
using Godot;

// TODO: CHECK IF HIT COMPONENT HITS BASED ON BOTH SENDER TYPE AND ENTITY TYPE!!!!!!

/// <summary>
/// HitComponent can detect HurtComponents as long as the HitComponent contains the Hurtable's type, and 
/// the Hurtable contains the Hitter's type.
/// </br></br>
/// HitComponent generally will want to make use of its sender's types to decide whether something can be hit or not, 
/// as a HurtComponent will make decisions on whether it can be hit or not based on the type of the sender as opposed to 
/// the type of the entity the HitComponent is attached to.
/// </br>
/// Eg. A HitComponent attached to a Projectile sent by a Turret hits an Enemy with a HurtComponent. The HurtComponent wants to check if it can be 
/// hit by the Projectile object, but also wants to know if the original 'sender' entity (the Turret) is valid to be hit from also. An Enemy may want 
/// to be hurtable by Projectile objects generically, but may not want to be hurtable by Enemies that use projectiles for example. Wheras a Tower 
/// which sends a Projectile could be a valid combination of sender and hitter to satisfy the hurt component.
/// </summary>
public partial class HitComponent : Area2D
{
	[Signal] public delegate void OnHitEventHandler(HurtComponent Hurt, float Damage);
	// [Signal] public delegate void OnExitHitEventHandler(HurtComponent Hurt);

	[Export] private float _hitDamage = 1f;
	[Export] private HurtComponent _target;

	[ExportGroup("Sender Types")]
	[Export] private Groups.GroupTypes _senderTypes;

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes, _validHurtableTypes;

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _hitCollisionShape2D;

	public float Damage { get => _hitDamage; }

	
	public void Initialize(float hitRadius, float damage, Groups.GroupTypes senderTypes, Groups.GroupTypes entityTypes, Groups.GroupTypes validHurtableTypes, HurtComponent target = null)
	{
		SetRadius(hitRadius);
		Initialize(damage, senderTypes, entityTypes, validHurtableTypes, target);
	}
	/// <summary>
	/// Damage and potential target for hit. If there is no specific target to hit, then the general set of hittable targets can be used to hit things.
	/// </summary>
	/// <param name="damage"></param>
	/// <param name="senderTypes"></param>
	/// <param name="validHurtableTypes"></param>
	/// <param name="target"></param>
	public void Initialize(float damage, Groups.GroupTypes senderTypes, Groups.GroupTypes entityTypes, Groups.GroupTypes validHurtableTypes, HurtComponent target = null)
	{
		_hitDamage = damage;
		_senderTypes = senderTypes;
		_thisEntityTypes = entityTypes;
		_validHurtableTypes = validHurtableTypes;
		_target = target;
	}

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (area is HurtComponent hurt && hurt != null)
			{
				if (Hittable(hurt))
				{
					// GD.Print($"HitComponent made contact with {area.Name} and is attempting to deal {_hitDamage} damage. Emitting OnHit signal.");
					hurt.Hurt(this, Damage);
					EmitSignal(SignalName.OnHit, hurt, Damage);
				}
			}
		};
		// AreaExited += (area) =>
		// {
		// 	if (area is HurtComponent hurt && hurt != null)
		// 	{
		// 		if (Hittable(hurt))
		// 		{
		// 			EmitSignal(SignalName.OnExitHit, hurt);
		// 		}
		// 	}
		// };
	}

	/// <summary>
	/// Can this HitComponent hurt a given HurtComponent? If HitComponent has a target, then HurtComponent must also be target.
	/// </summary>
	/// <param name="hurt"></param>
	/// <returns></returns>
	private bool Hittable(HurtComponent hurt)
	{
		return this.CanHit(hurt) && hurt.CanBeHurtBy(this);
	}
	public bool CanHit(HurtComponent hurt)
	{
		return ((hurt.GetEntityTypes() & _validHurtableTypes) != Groups.GroupTypes.None) && // Hurt component can be hit by hit component type
			   ((hurt.GetValidHitterTypes() & _senderTypes) != Groups.GroupTypes.None) && // Hurt component can be hit by sender of hit component
			   (_target == null || _target == hurt); // Hurt component is target (if target is assigned)
	}

	public float GetRadius()
	{
		return ((CircleShape2D)_hitCollisionShape2D.Shape).Radius;
	}
	public void SetRadius(float newRadius)
	{
		((CircleShape2D)_hitCollisionShape2D.Shape).Radius = newRadius;
	}
	public Groups.GroupTypes GetSenderTypes()
	{
		return _senderTypes;
	}
	public Groups.GroupTypes GetEntityTypes()
	{
		return _thisEntityTypes;
	}
	public Groups.GroupTypes GetValidHurtableTypes()
	{
		return _validHurtableTypes;
	}
	public HurtComponent GetTarget()
	{
		return _target;
	}
}
