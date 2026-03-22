
using Godot;

public partial class SpawnerComponent : Node2D
{
	[Signal] public delegate void OnSpawnedEventHandler(Node SpawnedNode);

	[Export] private SceneFilePathRes _scenePath;

	private PackedScene _scene;

	public override void _Ready()
	{
		if (_scenePath == null)
		{
			GD.Print($"SpawnerComponent {this} not given scene path to load from...");
			return;
		}
		_scene = ResourceLoader.Load<PackedScene>(SceneFilePathRes.UidToRid(_scenePath.ScenePath));
		if (_scene == null)
		{
			GD.Print($"SpawnerComponent {this} unable to load scene for spawning...");
			return;
		}
	}

	public Node Spawn()
	{
		if (_scene == null)
		{
			return null;
		}
		var node = _scene.Instantiate<Node>();
		if (node is Node2D node2D && node2D != null)
		{
			node2D.GlobalPosition = GlobalPosition;
		}
		EmitSignal(SignalName.OnSpawned, node);
		return node;
	}
}
