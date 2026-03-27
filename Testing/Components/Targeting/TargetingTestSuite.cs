
using CS780GroupProject.Scripts.Utils;
using GdUnit4;
using static GdUnit4.Assertions;
using System.Threading.Tasks;
using Godot;
using System.Collections.Generic;

namespace TestNS
{
	[TestSuite]
	public partial class TargetingTestSuite
	{
		private ISceneRunner _runner = null;

		// Scene root
		private TargetingTestScene _scene;
		// Nodes
		private TargetingComponent _targeting;
		private DetectorComponent _detector;
		private DetectableComponent _detectable;

		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://Testing/Components/Targeting/targeting_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();
			AssertThat(_runner.Scene()).IsNotNull().IsInstanceOf<TargetingTestScene>();

			_scene = _runner.Scene() as TargetingTestScene;
			AssertThat(_scene.TargetingComponent).IsNotNull();
			AssertThat(_scene.DetectorComponent).IsNotNull();
			AssertThat(_scene.DetectableComponent).IsNotNull();

			_targeting = _scene.TargetingComponent;
			_detector = _scene.DetectorComponent;
			_detectable = _scene.DetectableComponent;
		}

		[TestCase]
		public void T1()
		{

		}

	}
}
