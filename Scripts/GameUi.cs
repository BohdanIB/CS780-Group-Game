using Godot;

public partial class GameUi : CanvasLayer
{
	public override void _Ready()
	{
		var sideMenu = GetNode<SideMenuContainer>("SideMenuContainer");
		GD.Print($"SideMenu found: {sideMenu}");
		
		var turretPlacer = GetTree().GetRoot().GetNode<TurretPlacer>("Main/TurretPlacer");
		GD.Print($"TurretPlacer found: {turretPlacer}");
		
		sideMenu.TurretSelected += (turretType) => {
			GD.Print($"TurretSelected signal received: {turretType}");
			turretPlacer.EnablePlacementMode(turretType);
		};
	}
}
