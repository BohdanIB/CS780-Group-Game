
using System;
using System.Collections.Generic;
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;

/// <summary>
/// Generic representation of a structure which exists on tiles.
/// 
/// Should have a redefined Initialize and ToString whenever a new structure is contrived.
/// </summary>
public partial class GenericStructure: Node2D
{
	public enum ConfigurationType
	{
		None,
		TurretTargeting
	}

	public const Groups.GroupTypes TYPES = Groups.GroupTypes.Structure;
	protected GroundTile _locationTile;

	[ExportGroup("Components")]
	[Export] protected HealthComponent _health;
	[Export] protected HurtComponent _hurt; // Do ALL structures really need these components?
	[Export] protected AnimationComponent _animation;

	public TradingPort ConnectedPort {get; set;}
	public int ClosestPortProximity {get; set;}

	public virtual void Initialize(StructureStats stats, GroundTile tile)
	{
		if (stats != null) _animation.Initialize(stats.Animations, AnimationPackEntry.State.Idle);
		_locationTile = tile;
	}

	public override void _Ready()
	{
		_health = GetComponentInChildrenOrNull<HealthComponent>(this);
		_hurt = GetComponentInChildrenOrNull<HurtComponent>(this);
		_animation = GetComponentInChildrenOrNull<AnimationComponent>(this); // todo: Multiple types could cause issues here
		if (_health == null || _hurt == null || _animation == null)
		{
			GD.Print($"WARNING - GenericStructure {this} was unable to find health and/or hurt components on _Ready()");
		}
	}

	public static Dictionary<string, string[]> GetConfigurationOptions(ConfigurationType configurationType)
	{
		return configurationType switch
		{
			ConfigurationType.None => null,
			ConfigurationType.TurretTargeting => new()
				{
					["Target"] = Enum.GetNames(typeof(TargetingMode))
				},
			_ => null,
		};
	}

	public virtual void SetConfigurationOption(string configurationName, string configurationSelection) { }
}
