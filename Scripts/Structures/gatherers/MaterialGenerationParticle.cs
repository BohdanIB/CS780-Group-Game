using Godot;
using System;

public partial class MaterialGenerationParticle : Sprite2D
{
	[Export] private float _displayTime;
	[Export] private int _initialAlpha, _finalAlpha;
	[Export] private Vector2 _initialPosition, _finalPosition;

	private float _elapsedTime;
	private bool _isEnabled = false;


	public void StartVisual()
	{
		_elapsedTime = 0;
		_isEnabled = true;
		Position = _initialPosition;
		Modulate = new Color(1, 1, 1, 1);
		Visible = true;
	}

    public override void _Process(double delta)
    {
		if (!_isEnabled) return;

        if (_elapsedTime < _displayTime)
		{

			Color newModulate = SelfModulate;
			newModulate.A = Mathf.Lerp(_initialAlpha, _finalAlpha, _elapsedTime / _displayTime);
			SelfModulate = newModulate;

			Position = _initialPosition.Lerp(_finalPosition, _elapsedTime / _displayTime);
		}
		else if (_elapsedTime > _displayTime)
		{
			_isEnabled = false;
			Visible = false;
		}

		_elapsedTime += (float) delta;
    }


}
