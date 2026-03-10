using Godot;
using System;

/// <summary>
/// Entity has health.
/// </summary>
public partial class HealthComponent : Node2D
{
	[Signal] public delegate void OnNoHealthLeftEventHandler();
	[Export] private float _startingHealth = 1f;
	private float _currentHealth;

	public override void _Ready()
	{
		_currentHealth = _startingHealth;
	}

	public void ApplyDamage(float damage)
	{
		_currentHealth -= damage;
		if (_currentHealth <= 0f)
		{
			EmitSignal(SignalName.OnNoHealthLeft);
		}
	}
	public void ApplyHealth(float health)
	{
		_currentHealth += health;
		if (_currentHealth <= 0f)
		{
			EmitSignal(SignalName.OnNoHealthLeft);
		}
	}
}
