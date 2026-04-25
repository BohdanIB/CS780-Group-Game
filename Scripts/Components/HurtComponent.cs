
using CS780GroupProject.Scripts.Utils;
using Godot;

/// <summary>
/// TODO
/// </summary>
public partial class HurtComponent : Area2D
{
	[Signal] public delegate void OnHurtEventHandler(HitComponent Hit, float Damage);
	// [Signal] public delegate void OnExitHurtEventHandler(HitComponent Hit);

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes, _validHitterTypes;

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _hurtCollisionShape2D;

	public void Initialize(float hurtRadius, Groups.GroupTypes entityTypes, Groups.GroupTypes validHitterTypes)
	{
		SetRadius(hurtRadius);
		Initialize(entityTypes, validHitterTypes);
	}
	public void Initialize(Groups.GroupTypes entityTypes, Groups.GroupTypes validHitterTypes)
	{
		_thisEntityTypes = entityTypes;
		_validHitterTypes = validHitterTypes;
	}

	public override void _Ready()
	{
		// AreaEntered += (area) =>
		// {
		// 	if (area is HitComponent hit && hit != null)
		// 	{
		// 		if (Hurtable(hit))
		// 		{
		// 			EmitSignal(SignalName.OnEnterHurt, hit, hit.Damage);
		// 		}
		// 	}
		// };
		// AreaExited += (area) =>
		// {
		// 	if (area is HitComponent hit && hit != null)
		// 	{
		// 		if (Hurtable(hit))
		// 		{
		// 			EmitSignal(SignalName.OnExitHurt, hit);
		// 		}
		// 	}
		// };
	}

	public void Hurt(HitComponent hit, float damage)
	{
		if (this.Hurtable(hit))
		{
			EmitSignal(SignalName.OnHurt, hit, damage);
		}
	}

	/// <summary>
	/// Can this HurtComponent be hit by a given HitComponent?
	/// </summary>
	/// <param name="hit"></param>
	/// <returns></returns>
	private bool Hurtable(HitComponent hit)
	{
		return this.CanBeHurtBy(hit) && hit.CanHit(this);
	}
	public bool CanBeHurtBy(HitComponent hit)
	{
		// Todo: Might want to split hitter types and receiver types for hurt?
		return ((hit.GetEntityTypes() & _validHitterTypes) != Groups.GroupTypes.None) && // hitter types are valid for hurt
		       ((hit.GetSenderTypes() & _validHitterTypes) != Groups.GroupTypes.None); // hitter sender is valid for hurt
	}

	public float GetRadius()
	{
		return ((CircleShape2D)_hurtCollisionShape2D.Shape).Radius;
	}
	public void SetRadius(float newRadius)
	{
		((CircleShape2D)_hurtCollisionShape2D.Shape).Radius = newRadius;
	}
    public Groups.GroupTypes GetEntityTypes()
	{
		return _thisEntityTypes;
	}
	public Groups.GroupTypes GetValidHitterTypes()
	{
		return _validHitterTypes;
	}
}