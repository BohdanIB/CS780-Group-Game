using Godot;
using System;

public partial class ToggleMenuButton : Button
{
	public override void _Pressed()
	{
		GetNode<SideMenuContainer>("../SideMenuContainer").ToggleMenu();
	}
}
