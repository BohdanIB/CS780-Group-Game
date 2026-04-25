
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using Godot;
using System;
using System.Collections.Generic;

public partial class TargetingComponent : Node2D
{
	[Signal] public delegate void OnTargetSelectEventHandler(DetectableComponent Target);

	[Export] public TargetingMode TargetingStyle = TargetingMode.Close;
	[Export] private DetectorComponent _detector;
	// [Export] private HitComponent _hit; // todo: To detect hurt components

	private List<DetectableComponent> _targets = [];

	// /// <summary>
	// /// Initialize associated detector with stats. Convenience function
	// /// </summary>
	// /// <param name="radius"></param>
	// /// <param name="entityTypes"></param>
	// /// <param name="validTargets"></param>
	// public void Initialize(float radius, Groups.GroupTypes entityTypes, Groups.GroupTypes validTargets)
	// {
	// 	_detector.Initialize(radius, entityTypes, validTargets);
	// }
	// /// <summary>
	// /// Initialize associated detector with stats. Convenience function
	// /// </summary>
	// /// <param name="entityTypes"></param>
	// /// <param name="validTargets"></param>
	// public void Initialize(Groups.GroupTypes entityTypes, Groups.GroupTypes validTargets)
	// {
	// 	_detector.Initialize(entityTypes, validTargets);
	// }
	public void Initialize (TargetingMode targetingMode)
	{
		TargetingStyle = targetingMode;
	}

	public override void _Ready()
	{
		if (IsInstanceValid(_detector))
		{
			_detector.OnEnterDetector += (detectable) => 
			{
				_targets.Add(detectable);
			};
			_detector.OnExitDetector += (detectable) => 
			{
				_targets.Remove(detectable);
			};
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		PickTargetIfPossible();
	}

	public void PickTargetIfPossible()
	{
		if (_targets.Count > 0)
		{
			// Filter out frozen targets
			List<DetectableComponent> validTargets = _targets.FindAll(target =>
			{
				var mover = GetComponentInSiblingsOrNull<MoverComponent>(target);
				return mover == null || !mover.IsFrozen();
			});
			
			Area2D currTarget = TargetingStyle switch
			{
				TargetingMode.Random => RandomPick(validTargets),
				TargetingMode.First  => FirstPick(validTargets),
				TargetingMode.Last   => LastPick(validTargets),
				TargetingMode.Close  => ClosePick(validTargets),
				TargetingMode.Weak   => WeakPick(validTargets),
				TargetingMode.Strong => StrongPick(validTargets),
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
	private Area2D RandomPick(List<DetectableComponent> targets)
	{
		if (targets.Count == 0) { return null; }
		return targets[GD.RandRange(0, targets.Count)];
	}
	/// <summary>
	/// Pick the valid target that is closest to finishing its path.
	/// </summary>
	/// <returns></returns>
	private Area2D FirstPick(List<DetectableComponent> targets)
	{
		if (targets.Count == 0) { return null; }

		Area2D currTarget = null;
		float currTargetPathLength = float.PositiveInfinity;
		{ // Scope mover variable
			var target = targets[0];
			if (GetComponentInSiblingsOrNull<MoverComponent>(target) is var mover && mover != null)
			{
				currTarget = target;
				currTargetPathLength = mover.GetPathLengthFromCurrentPosition();
			}
		}
		for (int i = 1; i < targets.Count; i++)
		{
			var target = targets[i];
			if (GetComponentInSiblingsOrNull<MoverComponent>(target) is var mover && mover != null)
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
	private Area2D LastPick(List<DetectableComponent> targets)
	{
		if (_targets.Count == 0) { return null; }

		Area2D currTarget = null;
		float currTargetPathLength = 0.0f;
		{ // Scope mover variable
			var target = targets[0];
			if (GetComponentInSiblingsOrNull<MoverComponent>(target) is var mover && mover != null)
			{
				currTarget = target;
				currTargetPathLength = mover.GetPathLengthFromCurrentPosition();
			}
		}
		for (int i = 1; i < targets.Count; i++)
		{
			var target = targets[i];
			if (GetComponentInSiblingsOrNull<MoverComponent>(target) is var mover && mover != null)
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
	private Area2D ClosePick(List<DetectableComponent> targets)
	{
		if (targets.Count == 0) { return null; }

		Area2D currTarget = targets[0];
		float currTargetDistanceFromPosition = GlobalPosition.DistanceTo(currTarget.GlobalPosition);
		for (int i = 1; i < targets.Count; i++)
		{
			var target = targets[i];
			float targetDistanceFromPosition = GlobalPosition.DistanceTo(target.GlobalPosition);
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
	private Area2D WeakPick(List<DetectableComponent> targets)
	{
		if (targets.Count == 0) { return null; }

		Area2D currTarget = null;
		float currTargetHealth = float.PositiveInfinity;
		{ // Scope health variable
			var target = targets[0];
			if (GetComponentInSiblingsOrNull<HealthComponent>(target) is var health && health != null)
			{
				currTarget = target;
				currTargetHealth = health.GetHealth();
			}
		}
		for (int i = 1; i < targets.Count; i++)
		{
			var target = targets[i];
			if (GetComponentInSiblingsOrNull<HealthComponent>(target) is var health && health != null)
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
	private Area2D StrongPick(List<DetectableComponent> targets)
	{
		if (_targets.Count == 0) { return null; }

		Area2D currTarget = null;
		float currTargetHealth = 0.0f;
		{ // Scope health variable
			var target = targets[0];
			if (GetComponentInSiblingsOrNull<HealthComponent>(target) is var health && health != null)
			{
				currTarget = target;
				currTargetHealth = health.GetHealth();
			}
		}
		for (int i = 1; i < targets.Count; i++)
		{
			var target = targets[i];
			if (GetComponentInSiblingsOrNull<HealthComponent>(target) is var health && health != null)
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

	public void SetRadius(float newRadius)
	{
		_detector.SetRadius(newRadius);
	}
	/// <summary>
	/// Used for testing to see what the current target list is for TargetingComponent
	/// </summary>
	/// <returns></returns>
	public List<DetectableComponent> GetTargetList()
	{
		return _targets;
	}

}
