
using CS780GroupProject.Scripts.Utils;
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

	// Todo: Targeting doesn't work for any modes since this component is just receiving area2Ds.
	public void PickTargetIfPossible()
	{
		if (_targets.Count > 0)
		{
			Area2D currTarget;
			if (TargetingStyle == TargetingMode.Random)
			{
				currTarget = _targets[_random.Next(_targets.Count)];
			}
			else
			{
				// Look for an appropriate target given TargetingMode.
				currTarget = _targets[0];
				float currTargetDistanceFromPosition = Position.DistanceTo(currTarget.Position);
				// float currTargetDistanceFromGoal = currTarget.GetDistanceToGoalPixels();
				// float currTargetHealth = currTarget.GetCurrentHealth();
				for (int i = 1; i < _targets.Count; i++)
				{
					var target = _targets[i];
					float targetDistanceFromPosition = Position.DistanceTo(target.Position);
					// float targetDistanceFromGoal = target.GetDistanceToGoalPixels();
					// float targetHealth = target.GetCurrentHealth();
					if (/*(TargetingStyle == TargetingMode.First  && targetDistanceFromGoal < currTargetDistanceFromGoal) || 
						(TargetingStyle == TargetingMode.Last   && currTargetDistanceFromGoal < targetDistanceFromGoal) || */
						(TargetingStyle == TargetingMode.Close  && targetDistanceFromPosition < currTargetDistanceFromPosition)/* ||
						(TargetingStyle == TargetingMode.Weak   && targetHealth < currTargetHealth) ||
						(TargetingStyle == TargetingMode.Strong && currTargetHealth < targetHealth)*/)
					{
						currTarget = target;
						currTargetDistanceFromPosition = targetDistanceFromPosition;
						// currTargetDistanceFromGoal = targetDistanceFromGoal;
						// currTargetHealth = targetHealth;
					}
				}
			}
			// We have a valid target!
			EmitSignal(SignalName.OnTargetSelect, currTarget);
		}
	}
}
