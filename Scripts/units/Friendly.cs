
using CS780GroupProject.Scripts.Utils;
using Godot;
using System;
using System.Collections.Generic;

public partial class Friendly : PathFollower
{
	[Export] private FriendlyStats _stats = new(FriendlyStats.Category.Regular);

	[ExportGroup("Types")]
	[Export] public Groups.GroupTypes _friendlyTypes = PathFollower.TYPES | Groups.GroupTypes.Friendly;
	[Export] public Groups.GroupTypes _enemyTypes = Groups.GroupTypes.Enemy;
	// [Export] public Groups.GroupTypes _hurtTypes = Groups.GroupTypes.Enemy;
	// [Export] public Groups.GroupTypes _detectorTypes = Groups.GroupTypes.None;
	// [Export] public Groups.GroupTypes _detectableTypes = Groups.GroupTypes.Enemy;


	/// <summary>
	/// Initializes friendly with "generic" base stats for given type.
	/// </summary>
	/// <param name="type"></param>
	public void Initialize(FriendlyStats.Category type, Vector2[] path = null)
	{
		Initialize(new FriendlyStats(type), path);
	}
	/// <summary>
	/// Initializes friendly with custom stats.
	/// </summary>
	/// <param name="stats"></param>
	public void Initialize(FriendlyStats stats, Vector2[] path = null)
	{
		SetPath(path);
		_stats = stats;
		InitializeComponents();
		UpdateStats();
	}

	public override void _Ready()
	{
		base._Ready();

		// Component callbacks //

		_health.OnNoHealthLeft += () =>
		{
			GD.Print($"Friendly {Name} died.");
			QueueFree();
		};
		_hurt.OnEnterHurt += (area, damage) => 
		{
			_health.ApplyDamage(damage); 
		}; 
	}

	public void UpdateStats(FriendlyStats newStats = null)
	{
		if (newStats != null)
		{
			_stats = newStats;
		}
		UpdateComponents();
	}
	public void UpdateComponents()
	{
		if (this.IsNodeReady() && _stats != null)
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.SetRadius(_stats.HitboxRadius);
			_detector.SetRadius(_stats.AggroRadius);
			_detectable.SetRadius(_stats.DetectableRadius);
			_mover.Speed = _stats.MovementSpeed;
			
			// Todo: Add more updates
			UpdateSprite(); // todo: should be a component?
		}
	}
	private void InitializeComponents()
	{
		if (this.IsNodeReady() && _stats != null)
		{
			_health.SetHealth(_stats.Health); // todo: this might not want to update everytime components are updated.
			_hurt.Initialize(_friendlyTypes, _enemyTypes);
			_detector.Initialize(_friendlyTypes, _enemyTypes);
			_detectable.Initialize(_friendlyTypes, _enemyTypes);
			_mover.Initialize(_stats.MovementSpeed, this, start: true);

			UpdateSprite(); // todo: should be a component?
		}
	}
	private void UpdateSprite()
	{
		_animatedSprite2D.Frame = _stats.SpriteFrame;
	}
	public override string ToString()
	{
		return $"Friendly '{Name}': {_stats}";
	}

}
