using Godot;
using System;
using CS780GroupProject.Scripts.Utils;

public partial class Base : GenericStructure
{
	public static Base instance;

	public event Action<float> BaseDamaged;
	public event Action BaseDestroyed;

	public override void _Ready()
{
	instance ??= this;
	base._Ready();

	if (_health == null)
	{
		GD.Print($"WARNING - Base {this} was unable to find health component on _Ready()");
		return;
	}

	_health.SetHealth(1000f);
	_hurt.Initialize(Groups.GroupTypes.Friendly | Groups.GroupTypes.Structure, Groups.GroupTypes.Enemy);
	_hurt.OnHurt += (hit, damage) =>
	{
		_health.ApplyDamage(damage);
		BaseDamaged?.Invoke(_health.GetHealth());
	};

	_health.OnNoHealthLeft += OnHealthDepleted;

	var area = GetNode<Area2D>("Area2D");
	area.BodyEntered += OnBodyEntered;
}

	public override void _ExitTree()
	{
		if (instance == this)
			instance = null;
	}

	public static void ResetInstance()
	{
		instance = null;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Enemy enemy && IsInstanceValid(enemy))
		{
			GD.Print($"Base taking damage from enemy {enemy}. Current health before damage: {_health.GetHealth()}");
			_health.ApplyDamage(5);
			BaseDamaged?.Invoke(_health.GetHealth());
			GD.Print($"Base health after damage: {_health.GetHealth()}");
			enemy.SetDeferred("monitoring", false); // Disable collisions before freeing
			enemy.QueueFree();
		}
	}

	private void OnHealthDepleted()
	{
		GD.Print($"Base health depleted. Invoking BaseDestroyed.");
		BaseDestroyed?.Invoke();
	}
}
