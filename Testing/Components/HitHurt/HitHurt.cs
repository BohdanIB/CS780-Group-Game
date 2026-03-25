
using GdUnit4;
using GdUnit4.Asserts;
using static GdUnit4.Assertions;
using System.Threading.Tasks;

namespace TestNS
{
	/// <summary>
	/// Testing:
	/// <list type="bullet">
	/// <item>Initialization</item>
	/// <item>Valid hit basic</item>
	/// <item>Invalid hit basic</item>
	/// <item>Valid hit of specific scene, but invalid hurt</item>
	/// <item>Node with hit and hurt works properly (cannot detect itself also important!)</item>
	/// <item></item>
	/// </list>
	/// </summary>
	[TestSuite]
	public class HitHurt
	{
		private ISceneRunner _runner = null;

		// Scene root
		private HitHurtTestScene _scene;
		// Parent nodes
		private HitParent _hitParent;
		private HurtParent _hurtParent;
		private ComponentlessParent _componentlessParent;


		[BeforeTest]
		[RequireGodotRuntime]
		public void SetupScene()
		{
			_runner = ISceneRunner.Load("res://test/Components/HitHurt/hit_hurt_test_scene.tscn", true);
			AssertThat(_runner).IsNotNull();
			AssertThat(_runner.Scene()).IsNotNull().IsInstanceOf<HitHurtTestScene>();

			_scene = _runner.Scene() as HitHurtTestScene;
			AssertThat(_scene.HitParent).IsNotNull();
			AssertThat(_scene.HurtParent).IsNotNull();
			AssertThat(_scene.ComponentlessParent).IsNotNull();

			_hitParent = _scene.HitParent;
			_hurtParent = _scene.HurtParent;
			_componentlessParent = _scene.ComponentlessParent;
			AssertThat(_hitParent.Hit).IsNotNull();
			AssertThat(_hurtParent.Hurt).IsNotNull();
		}

		[TestCase]
		[RequireGodotRuntime]
		public void HitInitialization()
		{
			var hit = _hitParent.Hit;

			// First initialization
			// hit.Initialize(935, AutoFree(new SceneFilePathRes(_componentlessParent)));




			// TODO: Follow along with DetectorDetectable tests. Add and modify HitComponent and HurtComponent!




			// var detector = _detectorParent.Detector;
			// detector.Initialize(new SceneFilePathRes[]{AutoFree(new SceneFilePathRes(_detectableParent))});
			// // Check entity parent scene for component
			// var currentScene = _detectorParent.Detector.GetEntityScene().ScenePath;
			// var expectedScene = AutoFree(new SceneFilePathRes(_detectorParent)).ScenePath;
			// AssertThat(currentScene).IsEqual(expectedScene);
			// // Check detectable scenes
			// var detectableScenes = detector.GetDetectableScenes();
			// var expectedScenes = new SceneFilePathRes[] { AutoFree(new SceneFilePathRes(_detectableParent)) };
			// AssertThat(detectableScenes)
			// 	.IsNotNull()
			// 	.IsEqual(expectedScenes);

			// // Second Initialization
			// detector.Initialize(935, new SceneFilePathRes[]{ AutoFree(new SceneFilePathRes(_componentlessParent)) });
			// // Check entity parent scene for component
			// currentScene = _detectorParent.Detector.GetEntityScene().ScenePath;
			// expectedScene = AutoFree(new SceneFilePathRes(_detectorParent)).ScenePath;
			// AssertThat(currentScene).IsEqual(expectedScene);
			// // check radius
			// AssertThat(detector.GetDetectorRadius()).IsEqual(935);
			// // check detectable scenes
			// expectedScenes = new SceneFilePathRes[] { AutoFree(new SceneFilePathRes(_componentlessParent)) };
			// AssertThat(detector.GetDetectableScenes())
			// 	.IsNotNull()
			// 	.IsEqual(expectedScenes);
		}

	}
}

