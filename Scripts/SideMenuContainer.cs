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
		MouseFilter = Control.MouseFilterEnum.Pass;
		GD.Print("SideMenuContainer READY fired");
		GD.Print("Ballista path exists? " + (GetNodeOrNull<Button>("VBoxContainer/HBoxContainer/BallistaTurret") != null));


		var ballista = GetNode<Button>("VBoxContainer/HBoxContainer/BallistaTurret");
		GD.Print("Connected button path: " + ballista.GetPath());

		

		var blade = GetNode<Button>("VBoxContainer/HBoxContainer/BladeTurret");
		blade.Pressed += () => GD.Print(">>> BLADE BUTTON CLICKED <<<");

		ballista.Pressed += () => OnTurretButtonPressed(TurretStats.Category.Ballista);
		blade.Pressed += () => OnTurretButtonPressed(TurretStats.Category.Blade);


		
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
		GD.Print($"Button pressed, emitting TurretSelected: {turretType}");
		EmitSignal(SignalName.TurretSelected, (int)turretType);
	}

	public override void _Process(double delta)
	{
		var hovered = GetViewport().GuiGetHoveredControl();
		//GD.Print("Hovered control: " + (hovered != null ? hovered.Name : "null"));
	}

}
