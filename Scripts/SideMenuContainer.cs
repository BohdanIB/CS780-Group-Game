using Godot;
using System;

public partial class SideMenuContainer : PanelContainer
{
	[Signal]
	public delegate void StructureSelectedEventHandler(ConstructionInformation constructionInformation);
	

	private bool _open = false;
	private float _width = 250f;

	public override void _Ready()
	{
		CallDeferred(nameof(SetHiddenState));
		MouseFilter = Control.MouseFilterEnum.Pass;
		GD.Print("SideMenuContainer READY fired");
		GD.Print("Ballista path exists? " + (GetNodeOrNull<Button>("VBoxContainer/HBoxContainer/VBoxContainer/BallistaTurret") != null));
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

	public void OnStructureButtonPressed(ConstructionInformation constructionInformation)
	{
		GD.Print($"Button pressed, emitting selected ConstructionInformation: {constructionInformation}");
		EmitSignal(SignalName.StructureSelected, constructionInformation);
	}

	public override void _Process(double delta)
	{
		var hovered = GetViewport().GuiGetHoveredControl();
		//GD.Print("Hovered control: " + (hovered != null ? hovered.Name : "null"));
	}

}
