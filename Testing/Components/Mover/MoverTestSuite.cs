
using CS780GroupProject.Scripts.Utils;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;
using Godot;
using System.Collections.Generic;

namespace TestNS
{
	[TestSuite]
	public partial class MoverTestSuite
	{
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
			AssertThat(mover.GetMoverPath()).IsNull();
			AssertThat(mover.ParentNode).IsSame(moverParent);
			AssertThat(mover.CurrentlyMoving).IsFalse();
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
			AssertThat(mover.GetMoverPath()).IsNull();
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
			var moverPath = new List<Vector2>();
			mover.Initialize(speed, moverParent, moverPath: moverPath);
			AssertThat(mover.Speed).IsEqual(speed);
			AssertThat(mover.GetMoverPath())
				.IsNotNull()
				.IsSame(moverPath);
			AssertThat(mover.ParentNode).IsSame(moverParent);
			AssertThat(mover.CurrentlyMoving).IsFalse();
		}
		[TestCase]
		[RequireGodotRuntime]
		public void MoverInitialization_Start_Path()
		{
			var mover = _mover;
			var moverParent = _moverObject;
			var speed = 935f;
			var start = true;
			var moverPath = new List<Vector2>();
			mover.Initialize(speed, moverParent, start: start, moverPath: moverPath);
			AssertThat(mover.Speed).IsEqual(speed);
			AssertThat(mover.GetMoverPath())
				.IsNotNull()
				.IsSame(moverPath);
			AssertThat(mover.ParentNode).IsSame(moverParent);
			AssertThat(mover.CurrentlyMoving).IsTrue();
		}
	}
}

