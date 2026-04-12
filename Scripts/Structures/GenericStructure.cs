
using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Generic representation of a structure which exists on tiles.
/// 
/// Should have a redefined Initialize and ToString whenever a new structure is contrived.
/// </summary>
public partial class GenericStructure: Area2D
{
	public enum ConfigurationType
	{
		None,
		TurretTargetting
	}

	// Nodes
	[Export] protected CollisionShape2D _collisionShape2D;
	[Export] protected AnimationManager _idleAnimations;

	protected float _health;

	// public virtual void Initialize() { }

	public static Dictionary<string, string[]> GetConfigurationOptions(ConfigurationType configurationType)
	{
        return configurationType switch
        {
            ConfigurationType.None => null,
            ConfigurationType.TurretTargetting => new()
				{
					["Target"] = Enum.GetNames(typeof(Turret.TargetingMode))
				},
            _ => null,
        };
    }

	public virtual void SetConfigurationOption(string configurationName, string configurationSelection) { }
}
