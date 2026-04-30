using Godot;
using System;

public partial class MaterialGatherer : GenericStructure
{
	public int currentMaterialQuantity;

	private MaterialType _gatheredMaterial;
	private float _gatheringTime;

	[Export] private Timer _gatherTimer;
	[Export] private MaterialGenerationParticle _particle;


	public override void Initialize(StructureStats stats)
	{
		base.Initialize(stats);

		if (stats is MaterialGathererStats gathererStats)
		{
			_gatheredMaterial = gathererStats.gatheredMaterial;
			_gatheringTime = gathererStats.gatherTime;

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
		ConnectedPort?.StorageInventory.AddMaterials(_gatheredMaterial, 1);
		_particle.StartVisual();
		GD.Print($"  Produced {_gatheredMaterial}");
	}

}
