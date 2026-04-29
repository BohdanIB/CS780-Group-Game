using Godot;


public partial class AnimatedButton : Button
{
	private Tween _tween;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MouseEntered += OnHover;
		MouseExited += OnUnHover;
		Pressed += OnPressed;
	}

	private void OnHover(){
		if(_tween != null){
			_tween.Kill();
		}
		_tween = CreateTween();

		_tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.12f)
			  .SetTrans(Tween.TransitionType.Cubic)
			  .SetEase(Tween.EaseType.Out);
		
	}

	private void OnUnHover()
	{
		if(_tween != null){
			_tween.Kill();
		}
		_tween = CreateTween();

		_tween.TweenProperty(this, "scale", Vector2.One, 0.12f)
			  .SetTrans(Tween.TransitionType.Cubic)
			  .SetEase(Tween.EaseType.Out);
		
	}

	private void OnPressed()
	{
		if(_tween != null){
			_tween.Kill();
		}
		_tween = CreateTween();

		_tween.TweenProperty(this, "scale", new Vector2(0.9f, 0.9f), 0.08f)
	  		.SetEase(Tween.EaseType.In)
	  		.SetTrans(Tween.TransitionType.Cubic);

		_tween.TweenProperty(this, "scale", Vector2.One, 0.12f)
	  		.SetEase(Tween.EaseType.Out)
	  		.SetTrans(Tween.TransitionType.Cubic);

	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
