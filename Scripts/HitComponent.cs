using Godot;
using System;

/// <summary>
/// The ability for parent entity to hit other entities.
/// </summary>
public partial class HitComponent : Area2D
{
	[Signal] public delegate void OnHitEventHandler();

	[Export] private float _hitDamage = 1f;
	[Export] private Area2D _target;
	private Type[] _validTargetTypes;

	/// <summary>
	/// Damage and potential target for hit.
	/// </summary>
	/// <param name="damage">Damage to be dealt OnHit</param>
	/// <param name="target">Target of hit. If null, then any Hurt-able entity can get hit.</param>
	public void Initialize(float damage, Type[] validTargetTypes = null, Area2D target = null)
	{
		_hitDamage = damage;
		_validTargetTypes = validTargetTypes;
		_target = target;
	}

	// public override void _Ready()
	// {
	// 	if (_target == null) return; // todo

	// 	AreaEntered += (area) =>
	// 	{

	// 		// if (IsTarget(area))
	// 		// {
				
	// 		// }
	// 	}

	// }

	public bool IsTarget(Area2D area)
	{
		if (_target != null && _target == area) { return true; }

		// Check if target is a valid target for a hit
		foreach (var type in _validTargetTypes)
		{
			if (area.GetType() == type)
			{
				return true;
			}
		} 
		return false;
	}

	public float GetDamage()
	{
		return _hitDamage;
	}

}
