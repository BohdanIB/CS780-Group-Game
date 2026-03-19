
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;
using System;
using System.Collections.Generic;

public partial class TargetingComponent : Node2D
{
	[Signal] public delegate void OnTargetSelectEventHandler(Area2D Target);

	[Export] public TargetingMode TargetingStyle { get; set; } = TargetingMode.Close;
	[Export] private DetectorComponent _detector;

	private Random _random = new(); // todo: Seed this?
	private List<Area2D> _targets = [];

	public override void _Ready()
	{
		_detector.OnEnterDetector += (area) => 
		{
			_targets.Add(area);
		};
		_detector.OnExitDetector += (area) => 
		{
			_targets.Remove(area);
		};
	}

	public void PickTargetIfPossible()
	{
		if (_targets.Count > 0)
		{
			Area2D currTarget = TargetingStyle switch
			{
				TargetingMode.Random => RandomPick(),
				TargetingMode.First  => FirstPick(),
				TargetingMode.Last   => LastPick(),
				TargetingMode.Close  => ClosePick(),
				TargetingMode.Weak   => WeakPick(),
				TargetingMode.Strong => StrongPick(),
				_                    => throw new NotImplementedException(),
			};
			// We have a valid target!
			EmitSignal(SignalName.OnTargetSelect, currTarget);
		}
	}

	/// <summary>
	/// Pick a random valid target.
	/// </summary>
	/// <returns></returns>
	private Area2D RandomPick()
	{
		return _targets[_random.Next(_targets.Count)];
	}
	/// <summary>
	/// Pick the valid target that is closest to finishing its path.
	/// </summary>
	/// <returns></returns>
	private Area2D FirstPick()
	{
		Area2D currTarget = null;
		float currTargetPathLength = float.PositiveInfinity;
		{ // Scope mover variable
			var target = _targets[0];
			if (GetComponentOrNull<MoverComponent>(target) is var mover)
			{
				currTarget = target;
				currTargetPathLength = mover.GetPathLengthFromCurrentPosition();
			}
		}
		for (int i = 1; i < _targets.Count; i++)
		{
			var target = _targets[i];
			if (GetComponentOrNull<MoverComponent>(target) is var mover)
			{
				float targetPathLength = mover.GetPathLengthFromCurrentPosition();
				if (targetPathLength < currTargetPathLength)
				{
					currTarget = target;
					currTargetPathLength = targetPathLength;
				} 
			}
		}
		return currTarget;
	}
	/// <summary>
	/// Pick the valid target that is furthest from finishing its path.
	/// </summary>
	/// <returns></returns>
	private Area2D LastPick()
	{
		Area2D currTarget = null;
		float currTargetPathLength = 0.0f;
		{ // Scope mover variable
			var target = _targets[0];
			if (GetComponentOrNull<MoverComponent>(currTarget) is var mover)
			{
				currTarget = target;
				currTargetPathLength = mover.GetPathLengthFromCurrentPosition();
			}
		}
		for (int i = 1; i < _targets.Count; i++)
		{
			var target = _targets[i];
			if (GetComponentOrNull<MoverComponent>(target) is var mover)
			{
				float targetPathLength = mover.GetPathLengthFromCurrentPosition();
				if (currTargetPathLength < targetPathLength)
				{
					currTarget = target;
					currTargetPathLength = targetPathLength;
				} 
			}
		}
		return currTarget;
	}
	/// <summary>
	/// Pick the closest valid target to the targeting position.
	/// </summary>
	/// <returns></returns>
	private Area2D ClosePick()
	{
		Area2D currTarget = _targets[0];
		float currTargetDistanceFromPosition = Position.DistanceTo(currTarget.Position);
		for (int i = 1; i < _targets.Count; i++)
		{
			var target = _targets[i];
			float targetDistanceFromPosition = Position.DistanceTo(target.Position);
			if (targetDistanceFromPosition < currTargetDistanceFromPosition)
			{
				currTarget = target;
				currTargetDistanceFromPosition = targetDistanceFromPosition;
			} 
		}
		return currTarget;
	}
	/// <summary>
	/// Pick the weakest valid target.
	/// </summary>
	/// <returns></returns>
	private Area2D WeakPick()
	{
		Area2D currTarget = null;
		float currTargetHealth = float.PositiveInfinity;
		{ // Scope health variable
			var target = _targets[0];
			if (GetComponentOrNull<HealthComponent>(currTarget) is var health)
			{
				currTarget = target;
				currTargetHealth = health.GetHealth();
			}
		}
		for (int i = 1; i < _targets.Count; i++)
		{
			var target = _targets[i];
			if (GetComponentOrNull<HealthComponent>(target) is var health)
			{
				float targetHealth = health.GetHealth();
				if (targetHealth < currTargetHealth)
				{
					currTarget = target;
					currTargetHealth = targetHealth;
				} 
			}
		}
		return currTarget;
	}
	/// <summary>
	/// Pick the strongest valid target.
	/// </summary>
	/// <returns></returns>
	private Area2D StrongPick()
	{
		Area2D currTarget = null;
		float currTargetHealth = 0.0f;
		{ // Scope health variable
			var target = _targets[0];
			if (GetComponentOrNull<HealthComponent>(currTarget) is var health)
			{
				currTarget = target;
				currTargetHealth = health.GetHealth();
			}
		}
		for (int i = 1; i < _targets.Count; i++)
		{
			var target = _targets[i];
			if (GetComponentOrNull<HealthComponent>(target) is var health)
			{
				float targetHealth = health.GetHealth();
				if (currTargetHealth < targetHealth)
				{
					currTarget = target;
					currTargetHealth = targetHealth;
				} 
			}
		}
		return currTarget;
	}
}
