
using Godot;

/// <summary>
/// Used to specify scene paths for components or entities which need groupings of scenes specified for them. Specifically 
/// choosing this route of implementation to avoid Godot Groups and Layers, as these seem fragile and they will lead to 
/// spaghettification of project over time.
/// <br/>
/// Glorified struct; this is a compromise between fiddlyness and ease of component implementation + automatic updates if attached scene 
/// paths change. String path comparisons avoid cyclic dependencies that you will run into with direct PackedScene references.
/// <br/><br/>
/// Hack to avoid cyclic scene dependencies and C# specific bug with arrays of exported values not working...
/// <list type="bullet">
/// <item><description>
/// If PackedScenes are exported for scenes to use for grouping, then you will run into cyclic dependency.
/// </description></item>
/// <item><description>
/// If you try creating an array of path strings with File property hint in C#, then it breaks and becomes unusable...
/// </description></item>
/// </list>
/// </summary>
[GlobalClass] 
public partial class SceneFilePathRes : Resource
{
	/// <summary>
	/// This is the UID path of the given PackedScene file. This allows for changes to scene dynamically updating 
	/// the path given to this resource.
	/// </summary>
	[Export(PropertyHint.File, "*.tscn")] public string ScenePath { get; set; }

	public override string ToString()
	{
		return ScenePath;
	}

	public static bool EntitySharesScenePath(Node entity, SceneFilePathRes[] scenes)
	{
		foreach (var scene in scenes)
		{
			if (RidToUid(entity.SceneFilePath) == scene.ScenePath)
			{
				return true;
			}
		}
		return false;
	}
	public static bool SceneSharesScenePath(SceneFilePathRes s, SceneFilePathRes[] scenes)
	{
		foreach (var scene in scenes)
		{
			if (s.ScenePath == scene.ScenePath)
			{
				return true;
			}
		}
		return false;
	}
	public static string UidToRid(string uid)
	{
		return ResourceUid.UidToPath(uid);
	}
	public static string RidToUid(string rid)
	{
		return ResourceUid.PathToUid(rid);
	}

}
