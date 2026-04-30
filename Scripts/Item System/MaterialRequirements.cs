using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class MaterialRequirements : Resource
{
	[Export] private Dictionary<MaterialType, int> _requirements;


	public bool AreMaterialsAvailiable(Inventory inventory)
	{
		foreach (MaterialType material in _requirements.Keys)
		{
			if (!inventory.HasMaterial(material, _requirements[material])) return false;
		}

		return true;
	}

	public bool SpendMaterials(Inventory inventory)
	{
		if (!AreMaterialsAvailiable(inventory)) return false;

		foreach (MaterialType material in _requirements.Keys)
		{
			inventory.RemoveMaterials(material, _requirements[material]);
		}

		return true;

	}
}
