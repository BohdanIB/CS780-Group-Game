using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public delegate void OnContentsChangedEventHandler();
    private Dictionary<MaterialType, int> materialCounts = [];

    public bool HasMaterial(MaterialType material)
    {
        return GetMaterialCount(material) > 0;
    }

    public bool HasMaterial(MaterialType material, int quantity)
    {
        return GetMaterialCount(material) >= quantity;
    }

    public int GetMaterialCount(MaterialType material)
    {
        if (material == null) return 0;
        if (!materialCounts.TryGetValue(material, out int value)) return 0;

        return value;
    }

    public void AddMaterials(MaterialType material, int count)
    {
        if (material == null || count == 0) return;
        if (GetMaterialCount(material) == 0)
        {
            materialCounts[material] = count;
        } 
        else
        {
            materialCounts[material] += count;
        }
    }

    public bool RemoveMaterials(MaterialType material, int count)
    {
        int currentCount = GetMaterialCount(material);
        if (currentCount < count)
        {
            return false;
        }
        else if (currentCount == count)
        {
            materialCounts.Remove(material);
            return true;
        }
        else
        {
            materialCounts[material] -= count;
            return true;
        }

    }

    public MaterialType[] GetContainedMaterials()
    {
        return [.. materialCounts.Keys];
    }

    public void TransferContents(Inventory destinationInventory)
    {
        foreach (MaterialType material in materialCounts.Keys)
        {
            destinationInventory.AddMaterials(material, materialCounts[material]);
            RemoveMaterials(material, materialCounts[material]);
        }
    }

    public override string ToString()
    {
        string output = "Contents of Inventory:";
        foreach (MaterialType material in materialCounts.Keys)
        {
            output += $"\n {material.ResourceName} : {materialCounts[material]}";
        }
        return output;
    }

}
