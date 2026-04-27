using Godot;
using System;

public partial class OptionSelector : Panel
{
	[Export] private TextureButton _leftButton, _rightButton;
	[Export] private Label _selectionLabel, _nameLabel;

	public string OptionsName {get; private set;}
	private string[] _options;
	private int _currentIndex;

	public bool HasOptions => _options != null && _options.Length > 0;

	[Signal] public delegate void OnIndexChangedEventHandler(int index);


	public void SetOptions(string[] options, string name, int index=0)
	{
		this._options = options;
		this.OptionsName = name;
		_nameLabel.Text = name;
		_currentIndex = Math.Max(0, Math.Min(options.Length-1, index));

		if (options != null && options.Length > 0)
		{
			Visible = true;
			UpdateDisplay();
		} else
		{
			Visible = false;
		}
	}

	private void UpdateDisplay()
	{
		_selectionLabel.Text = _options[_currentIndex];
		_leftButton.Visible = _currentIndex > 0;
		_rightButton.Visible = _currentIndex < _options.Length-1;
	}

	public void IncreaseIndex()
	{
		_currentIndex = Math.Min(_options.Length-1, _currentIndex+1);
		EmitSignal(SignalName.OnIndexChanged, _currentIndex);
		UpdateDisplay();
	}

	public void DecreaseIndex()
	{
		_currentIndex = Math.Max(0, _currentIndex-1);
		EmitSignal(SignalName.OnIndexChanged, _currentIndex);
		UpdateDisplay();
	}

	public string GetOptionSelection()
	{
		GD.Print("GetOptionSelection called, HasOptions: ", HasOptions, " OptionsName: ", OptionsName);
		return _options[_currentIndex];
	}
}
