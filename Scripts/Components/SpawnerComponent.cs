
using Godot;

public partial class SpawnerComponent : Node2D
{
	[Signal] public delegate void OnSpawnedEventHandler(Node SpawnedNode);

	private string _scenePath = null;
	[Export(PropertyHint.File, "*.tscn")] public string ScenePath
	{
		get => _scenePath;
		set
		{
			_scenePath = value;
			if (_scenePath == null)
			{
				GD.Print($"SpawnerComponent {this} not given scene path to load from...");
				return;
			}
			_scene = ResourceLoader.Load<PackedScene>(_scenePath);
			if (_scene == null)
			{
				GD.Print($"SpawnerComponent {this} unable to load scene for spawning...");
			}
		}
	}

	private PackedScene _scene;

	public void Initialize(string scenePath)
	{
		ScenePath = scenePath;
	}

	public override void _Ready()
	{
		if (ScenePath != null) { Initialize(ScenePath); }
	}

	public Node Spawn()
	{
		if (ScenePath == null || _scene == null)
		{
			return null;
		}
		else if (_scene == null)
		{
			Initialize(ScenePath);
			if (_scene == null) { return null; }
		}

		//var node = _scene.Instantiate<Node>();
		var node = _scene.Instantiate();

		if (node is Node2D node2D && node2D != null)
		{
			node2D.GlobalPosition = GlobalPosition;
		}
		EmitSignal(SignalName.OnSpawned, node);
		return node;
	}
}
