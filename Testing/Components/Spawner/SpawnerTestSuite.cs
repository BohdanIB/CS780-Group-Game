
using CS780GroupProject.Scripts.Utils;
using static CS780GroupProject.Scripts.Utils.NodeComponentChecking;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;
using Godot;
using System.Collections.Generic;
using System;

namespace TestNS
{
	[TestSuite]
    [RequireGodotRuntime]
    public partial class SpawnerTestSuite : Node
    {
        
		private partial class SignalCollector : Node
		{
            public List<Node> SpawnedList {get; private set;} = new();
			public SignalCollector(SpawnerComponent spawner)
			{
				ConnectComponents(spawner);
			}
			public void ConnectComponents(SpawnerComponent spawner)
			{
                spawner.OnSpawned += (spawnedNode) =>
                {
                    SpawnedList.Add(spawnedNode);
                };
			}
		}

		private const uint GENERIC_WAIT_FRAMES = 4;

		private ISceneRunner _runner = null;
		// Scene root
		private SpawnerTestScene _scene;
		// Nodes
		private Node2D _spawnerNode; 
        private SpawnerComponent _spawner;

		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://Testing/Components/Spawner/spawner_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();

			_scene = _runner.Scene() as SpawnerTestScene;
			AssertThat(_scene).IsNotNull();

            _spawnerNode = _scene.SpawnerNode;
            AssertThat(_spawnerNode).IsNotNull();

            _spawner = GetComponentInChildrenOrNull<SpawnerComponent>(_spawnerNode);
            AssertThat(_spawner).IsNotNull();
		}

		[TestCase]
		[RequireGodotRuntime]
		public void Initialization()
		{
            var spawnerNode = _spawnerNode;
            var spawner = _spawner;

            var spawnerPosition = new Vector2(100, 100);
            spawnerNode.GlobalPosition = spawnerPosition;

            var scenePath = "res://Scenes/projectile.tscn";
            spawner.Initialize(scenePath);
            AssertThat(spawner.ScenePath).IsEqual(scenePath);
        }

		[TestCase]
		[RequireGodotRuntime]
		public async Task Spawning_Basic()
		{
            var spawnerNode = _spawnerNode;
            var spawner = _spawner;

            var spawnerPosition = new Vector2(100, 100);
            spawnerNode.GlobalPosition = spawnerPosition;

            var signalCollector = AutoFree(new SignalCollector(spawner));

            var scenePath = "res://Scenes/projectile.tscn";
            spawner.Initialize(scenePath);
            AssertThat(spawner.ScenePath).IsEqual(scenePath);

			// Check that no signals or anything have fired off
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.SpawnedList).IsEmpty();

            // Spawn node
            spawner.Spawn();
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.SpawnedList).HasSize(1);
            var spawnedNode1 = signalCollector.SpawnedList[0] as Projectile;
            AssertThat(spawnedNode1).IsNotNull();
            AssertThat(spawnedNode1.GlobalPosition).IsEqual(spawnerPosition);

            // Switch type of scene to spawn
            spawnerPosition = new(200, 200);
            spawnerNode.GlobalPosition = spawnerPosition;
            scenePath = "res://Scenes/Components/detector_component.tscn";
            spawner.ScenePath = scenePath;
            spawner.Spawn();
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.SpawnedList).HasSize(2);
            var spawnedNode2 = signalCollector.SpawnedList[1] as DetectorComponent;
            AssertThat(spawnedNode2).IsNotNull();
            AssertThat(spawnedNode2.GlobalPosition).IsEqual(spawnerPosition);

            // No other spawning happens?
			await _runner.SimulateFrames(GENERIC_WAIT_FRAMES);
			AssertThat(signalCollector.SpawnedList).HasSize(2);
        }


            


        // scenepath works for loading properly
        // OnSpawned creates expected node at expected location (using spawn function)
        // Switching type of node to spawn works properly

    }
}
