using Godot;
using System;

public partial class SideMenuContainer : PanelContainer
{
	private bool _open = false;
	private float _width = 250f;

	public override void _Ready()
	{
		CallDeferred(nameof(SetHiddenState));
	}

	private void SetHiddenState()
	{
		OffsetLeft = 0f;
		OffsetRight = _width;
	}

	public void ToggleMenu()
	{
		var tween = CreateTween();
		tween.SetParallel(true); // animate both offsets simultaneously

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
}
