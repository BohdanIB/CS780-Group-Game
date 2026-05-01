using Godot;
using System;

public partial class MaterialGatherer : GenericStructure
{
	public int currentMaterialQuantity;

	private MaterialType _gatheredMaterial;
	private float _gatheringTime;
	private int _gatherQuantity;

	[Export] private Timer _gatherTimer;
	[Export] private MaterialGenerationParticle _particle;


	public override void Initialize(StructureStats stats, GroundTile tile)
	{
		base.Initialize(stats, tile);

		if (stats is MaterialGathererStats gathererStats)
		{
			_gatheredMaterial = gathererStats.gatheredMaterial;
			_gatheringTime = gathererStats.gatherTime;
			_gatherQuantity = gathererStats.gatherQuantity;

			_particle.RegionRect = _gatheredMaterial.DisplayImageRect;
			_particle.Texture = _gatheredMaterial.DisplayImageAtlas;
		}
	}

	public override void _Ready()
	{
		_gatherTimer.WaitTime = _gatheringTime;
		_gatherTimer.Start();
	}



	public void ProduceMaterial()
	{
		ConnectedPort?.StorageInventory.AddMaterials(_gatheredMaterial, _gatherQuantity);
		_particle.StartVisual();
		GD.Print($"  Produced {_gatheredMaterial}");
	}

}
