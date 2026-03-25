
using CS780GroupProject.Scripts.Utils;
using Godot;

/// <summary>
/// TODO
/// </summary>
public partial class HurtComponent : Area2D
{
	[Signal] public delegate void OnEnterHurtEventHandler(HitComponent Hit, float Damage);
	[Signal] public delegate void OnExitHurtEventHandler(HitComponent Hit);

	// Todo: Might be necessary to utilize second sender types instead of merging? Can't come up with the edgecase where this doesn't work atm
	// [ExportGroup("___ Types")]
	// [Export] private Groups.GroupTypes _receiverTypes;

	[ExportGroup("Group Types")]
	[Export] private Groups.GroupTypes _thisEntityTypes, _validHitterTypes;

	[ExportGroup("Exported Child Nodes")]
	[Export] private CollisionShape2D _hurtCollisionShape2D;

	public void Initialize(float hurtRadius, Groups.GroupTypes entityTypes, Groups.GroupTypes validHitterTypes)
	{
		ModifyHurtRadius(hurtRadius);
		Initialize(entityTypes, validHitterTypes);
	}
	public void Initialize(Groups.GroupTypes entityTypes, Groups.GroupTypes validHitterTypes)
	{
		_thisEntityTypes = entityTypes;
		_validHitterTypes = validHitterTypes;
	}
// TODO: The HurtComponent can be hurt by valid hitter types AND valid sender entities. Might not be what we want in future.
	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (area is HitComponent hit && hit != null)
			{
				if (Hurtable(hit))
				{
					EmitSignal(SignalName.OnEnterHurt, hit, hit.Damage);
				}
			}
		};
		AreaExited += (area) =>
		{
			if (area is HitComponent hit && hit != null)
			{
				if (Hurtable(hit))
				{
					EmitSignal(SignalName.OnExitHurt, hit);
				}
			}
		};
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
		return (hit.GetEntityTypes() & _validHitterTypes) != Groups.GroupTypes.None;
	}

	// Todo: Handle shapes other than circles in future?
	public void ModifyHurtRadius(float newRadius)
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
