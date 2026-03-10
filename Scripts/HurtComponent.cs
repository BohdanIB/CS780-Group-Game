using Godot;
using System;

/// <summary>
/// The ability for parent entity to be hurt by other entities.
/// </summary>
public partial class HurtComponent : Area2D
{

	[Signal] public delegate void OnHurtEventHandler(float damage);

	public override void _Ready()
	{
		AreaEntered += (area) =>
		{
			if (area is HitComponent hitComponent && hitComponent.IsTarget(this)) // todo
			{
				EmitSignal(SignalName.OnHurt, hitComponent.GetDamage());
			}
		};
	}
}
