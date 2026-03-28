
using Godot;

public partial class TargetingTestScene : Node2D
{
	[Export] public Node2D
		TargetingNode,             // What is being tested for targeting capabilities
		Target1, Target2, Target3; // potential targets

}
