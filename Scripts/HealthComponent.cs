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

	public void SetHealth(float health)
	{
		_startingHealth = health;
		_currentHealth = health;
		CheckHealthLeft();
	}

	public void ApplyDamage(float damage)
	{
		float oldHealth = _currentHealth;
		_currentHealth -= damage;
		GD.Print($"HealthComponent applying damage {damage} to current health of {oldHealth}. New health value: {_currentHealth}.");
		CheckHealthLeft();
	}
	public void ApplyHealth(float health)
	{
		float oldHealth = _currentHealth;
		_currentHealth += health;
		GD.Print($"HealthComponent applying health {health} to current health of {oldHealth}. New health value: {_currentHealth}.");
		CheckHealthLeft();
	}

	private void CheckHealthLeft()
	{
		if (_currentHealth <= 0f)
		{
			GD.Print($"\tHealthComponent current health: '{_currentHealth}' is less than or equal to 0. Emitting OnNoHealthLeft signal.");
			EmitSignal(SignalName.OnNoHealthLeft);
		}
	}
}
