
using CS780GroupProject.Scripts.Utils;
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
	public partial class MoverTestSuite
	{
		private partial class SignalCollector : Node
		{
			public List<(bool hasNextPoint, Vector2 nextPoint)> OnPathPointReachedList { get; private set; } = new();
			public int OnPathCompletedCount { get; private set; } = 0;

			public SignalCollector(MoverComponent mover)
			{
				ConnectComponents(mover);
			}
			public void ConnectComponents(MoverComponent mover)
			{
				mover.OnPathPointReached += (hasNextPoint, nextPoint) =>
				{
					OnPathPointReachedList.Add((hasNextPoint, nextPoint));
				};
				mover.OnPathCompleted += () =>
				{
					OnPathCompletedCount += 1;
				};
			}
			public void ClearSignalCollector()
			{
				OnPathPointReachedList.Clear();
				OnPathCompletedCount = 0;
			}
		}
		private ISceneRunner _runner = null;
		// Scene root
		private MoverTestScene _scene;
		// Nodes
		private Node2D _moverObject;
		private MoverComponent _mover;


		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://Testing/Components/Mover/mover_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();
			AssertThat(_runner.Scene()).IsNotNull().IsInstanceOf<MoverTestScene>();

			_scene = _runner.Scene() as MoverTestScene;
			AssertThat(_scene.MoverObject).IsNotNull();
			AssertThat(_scene.MoverComponent).IsNotNull();

			_moverObject = _scene.MoverObject;
			_mover = _scene.MoverComponent;
		}

		[TestCase]
		[RequireGodotRuntime]
		public void MoverInitialization_NoStart_NoPath()
		{
			var mover = _mover;
			var moverParent = _moverObject;
			var speed = 935f;
			mover.Initialize(speed, moverParent);
			AssertThat(mover.Speed).IsEqual(speed);
			AssertThat(mover.GetMoverPath())
				.IsNotNull()
				.IsEmpty();
			AssertThat(mover.ParentNode).IsSame(moverParent);
			AssertThat(mover.CurrentlyMoving).IsTrue();
		}

		[TestCase]
		[RequireGodotRuntime]
		public void MoverInitialization_Start_NoPath()
		{
			var mover = _mover;
			var moverParent = _moverObject;
			var speed = 935f;
			var start = true;
			mover.Initialize(speed, moverParent, start: start);
			AssertThat(mover.Speed).IsEqual(speed);
			AssertThat(mover.GetMoverPath())
				.IsNotNull()
				.IsEmpty();
			AssertThat(mover.ParentNode).IsSame(moverParent);
			AssertThat(mover.CurrentlyMoving).IsTrue();
		}

		[TestCase]
		[RequireGodotRuntime]
		public void MoverInitialization_NoStart_Path()
		{
			var mover = _mover;
			var moverParent = _moverObject;
			var speed = 935f;
			var moverPath = Array.Empty<Vector2>();
			mover.Initialize(speed, moverParent, moverPath: moverPath);
			AssertThat(mover.Speed).IsEqual(speed);
			AssertThat(mover.GetMoverPath())
				.IsNotNull()
				.IsSame(moverPath);
			AssertThat(mover.ParentNode).IsSame(moverParent);
			AssertThat(mover.CurrentlyMoving).IsTrue();
		}
		[TestCase]
		[RequireGodotRuntime]
		public void MoverInitialization_Start_Path()
		{
			var mover = _mover;
			var moverParent = _moverObject;
			var speed = 935f;
			var start = true;
			var moverPath = Array.Empty<Vector2>();
			mover.Initialize(speed, moverParent, start: start, moverPath: moverPath);
			AssertThat(mover.Speed).IsEqual(speed);
			AssertThat(mover.GetMoverPath())
				.IsNotNull()
				.IsSame(moverPath);
			AssertThat(mover.ParentNode).IsSame(moverParent);
			AssertThat(mover.CurrentlyMoving).IsTrue();
		}
		[TestCase]
		[RequireGodotRuntime]
		public async Task Mover_FollowPath_Basic()
		{
			var mover = _mover;
			var moverParent = _moverObject;
			
			var START_POINT = new Vector2(0, 0);
			var END_POINT   = new Vector2(50, 0);

			moverParent.GlobalPosition = START_POINT;

			var signalCollector = AutoFree(new SignalCollector(mover));

			var speed = 50f;
			var parent = moverParent;
			var start = false;
			var moverPath = new Vector2[] {END_POINT};
			mover.Initialize(speed, parent, start, moverPath);

			// Shouldn't move
			await _runner.SimulateFrames(10, 150);
			AssertThat(parent.GlobalPosition).IsEqual(START_POINT);
			AssertThat(signalCollector.OnPathPointReachedList).IsEmpty();
			AssertThat(signalCollector.OnPathCompletedCount).IsZero();

			// Should have enough time to move to end of path.
			mover.Start();
			await _runner.SimulateFrames(10, 150);
			AssertThat(parent.GlobalPosition).IsEqualApprox(END_POINT, new(0.05f, 0.05f));
			AssertThat(signalCollector.OnPathPointReachedList)
				.HasSize(1)
				.Contains((false, Vector2.Zero));
			AssertThat(signalCollector.OnPathCompletedCount).IsEqual(1);
		}
		[TestCase]
		[RequireGodotRuntime]
		public async Task Mover_FollowPath_SetPath_Basic()
		{
			var mover = _mover;
			var moverParent = _moverObject;
			moverParent.GlobalPosition = new(0, 0);

			var signalCollector = AutoFree(new SignalCollector(mover));

			var speed = 50f;
			var parent = moverParent;
			var start = true;
			var moverPath = new Vector2[]{new(50,0)};
			mover.Initialize(speed, parent, start, moverPath);

			// Should reach first path and fire off signal properly
			await _runner.SimulateFrames(10, 150);
			AssertThat(parent.GlobalPosition).IsEqualApprox(new(50,0), new(0.05f, 0.05f));
			AssertThat(signalCollector.OnPathPointReachedList).HasSize(1);
			AssertThat(signalCollector.OnPathCompletedCount).IsEqual(1);
			signalCollector.ClearSignalCollector();

			// Set a track which it should reach various points of at specific times (moving 50 units per second)
			Vector2 
				POINT_1 = new(100,0),
				POINT_2 = new(100, 50),
				POINT_3 = new(100, 100),
				POINT_4 = new(50, 100),
				POINT_5 = new(0, 100);
			moverPath = [POINT_1, POINT_2, POINT_3, POINT_4, POINT_5];
			mover.SetMoverPath(moverPath);

			await _runner.SimulateFrames(10, 101);
			AssertThat(mover.PathCompleted()).IsFalse();
			AssertThat(signalCollector.OnPathPointReachedList)
				.HasSize(1)
				.Contains((true, POINT_2));
			AssertThat(signalCollector.OnPathCompletedCount).IsZero();
			await _runner.SimulateFrames(10, 101);
			AssertThat(mover.PathCompleted()).IsFalse();
			AssertThat(signalCollector.OnPathPointReachedList)
				.HasSize(2)
				.Contains((true, POINT_3));
			AssertThat(signalCollector.OnPathCompletedCount).IsZero();
			await _runner.SimulateFrames(10, 101);
			AssertThat(mover.PathCompleted()).IsFalse();
			AssertThat(signalCollector.OnPathPointReachedList)
				.HasSize(3)
				.Contains((true, POINT_4));
			AssertThat(signalCollector.OnPathCompletedCount).IsZero();
			await _runner.SimulateFrames(10, 101);
			AssertThat(mover.PathCompleted()).IsFalse();
			AssertThat(signalCollector.OnPathPointReachedList)
				.HasSize(4)
				.Contains((true, POINT_5));
			AssertThat(signalCollector.OnPathCompletedCount).IsZero();
			await _runner.SimulateFrames(10, 150); // Wait some extra time to make sure it has a reasonable amount of time to complete path
			AssertThat(mover.PathCompleted()).IsTrue();
			AssertThat(signalCollector.OnPathPointReachedList)
				.HasSize(5)
				.Contains((false, Vector2.Zero));
			AssertThat(signalCollector.OnPathCompletedCount).IsEqual(1);
		}
			// var moverPath = new List<Vector2>(){new(10,0), new(20, 0), new(30,0), new(40,0), new(50,0)};

		// TODO: Path length tests? Other function tests
	}
}

