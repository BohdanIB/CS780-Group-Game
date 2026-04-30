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
		GD.Print("Ballista path exists? " + (GetNodeOrNull<Button>("VBoxContainer/HBoxContainer/VBoxContainer/BallistaTurret") != null));


		var blade    = GetNode<Button>("VBoxContainer/HBoxContainer/VBoxContainer/BladeTurret");
		var ballista = GetNode<Button>("VBoxContainer/HBoxContainer/VBoxContainer2/BallistaTurret");
		var bomb = GetNode<Button>("VBoxContainer/HBoxContainer2/VBoxContainer3/BombTurret");
		var freeze =  GetNode<Button>("VBoxContainer/HBoxContainer2/VBoxContainer4/FreezeTurret");
		var electro =  GetNode<Button>("VBoxContainer/HBoxContainer3/VBoxContainer5/ElectroTurret");
		
		blade.Pressed += () => GD.Print(">>> BLADE BUTTON CLICKED <<<");
		
		bomb.Pressed += () => GD.Print(">>> BOMB BUTTON CLICKED <<<");

		ballista.Pressed += () => OnTurretButtonPressed(TurretStats.Category.Ballista);
		blade.Pressed += () => OnTurretButtonPressed(TurretStats.Category.Blade);
		bomb.Pressed += () => OnTurretButtonPressed(TurretStats.Category.Bomb);
		electro.Pressed += () => OnTurretButtonPressed(TurretStats.Category.Electro);
		freeze.Pressed += () => OnTurretButtonPressed(TurretStats.Category.Freeze);


		
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
