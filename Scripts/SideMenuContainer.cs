using Godot;
using System;

public partial class SideMenuContainer : PanelContainer
{
	[Signal]
	public delegate void TurretSelectedEventHandler(TurretStats.Category turretType);

	private bool _open = false;
	private float _width = 250f;

	public override void _Ready()
	{
		CallDeferred(nameof(SetHiddenState));

		 
			GetNode<Button>("BallistaTurret").Pressed += () => OnTurretButtonPressed(TurretStats.Category.Ballista);
			GetNode<Button>("BladeTurret").Pressed += () => OnTurretButtonPressed(TurretStats.Category.Blade);
		
	}

	private void SetHiddenState()
	{
		OffsetLeft = 0f;
		OffsetRight = _width;
	}

	public void ToggleMenu()
	{
		var tween = CreateTween();
		tween.SetParallel(true); 

		if (_open)
{
	tween.TweenProperty(this, "offset_left", 0f, 0.4f)
		 .SetTrans(Tween.TransitionType.Cubic)
		 .SetEase(Tween.EaseType.Out);
	tween.TweenProperty(this, "offset_right", _width, 0.4f)
		 .SetTrans(Tween.TransitionType.Cubic)
		 .SetEase(Tween.EaseType.Out);
}
else
{
	tween.TweenProperty(this, "offset_left", -_width, 0.4f)
		 .SetTrans(Tween.TransitionType.Cubic)
		 .SetEase(Tween.EaseType.Out);
	tween.TweenProperty(this, "offset_right", 0f, 0.4f)
		 .SetTrans(Tween.TransitionType.Cubic)
		 .SetEase(Tween.EaseType.Out);
}

		_open = !_open;
	}

	private void OnTurretButtonPressed(TurretStats.Category turretType)
	{
		EmitSignal(SignalName.TurretSelected, (int)turretType);
	}
}
