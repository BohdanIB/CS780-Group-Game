using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class StructurePlacer : Node2D
{
	[Signal] public delegate void OnStructurePlacedEventHandler();
	[Signal] public delegate void OnPlacementStoppedEventHandler();

	[Export] private Sprite2D _placementGhost;
	[Export] Color _validPlacementColor, _invalidPlacementColor;
	[Export] BoxContainer _selectorContainer;
	private OptionSelector[] _optionSelectors;

	private ConstructionInformation _constructionInformation;
	private Inventory _paymentInventory;
	private TileMapLayer _placementTilemap;
	private GenericGrid<GroundTile> _placementGrid;
	private Vector2I _currentGridCoordinates;
	private bool _isEnabled;
	private bool _isPlacementValid = false;

	[Export] public ConstructionInformation[] temporaryConstructionInfo; //TODO: Remove
	private int infoIndex = 0;

    public override void _Ready()
    {
		List<OptionSelector> selectors = [];
        foreach (Node child in _selectorContainer.GetChildren())
		{
			if (child is OptionSelector selector)
			{
				selectors.Add(selector);
			}
		}
		_optionSelectors = [.. selectors];

		DisablePlacement();
    }


	public void Initialize(PlayArea targetPlayArea, Inventory paymentInventory)
	{
		_placementTilemap = targetPlayArea.GridRenderer.TerrainMap.GetLayers()[0]; // This is not clean
		_placementGrid = targetPlayArea.Grid;
		_paymentInventory = paymentInventory;
	}

	public void SetStructure(ConstructionInformation constructionInformation)
	{
		if (constructionInformation == null)
		{
			// Disable, no structure to place
			DisablePlacement();
		} 
		else
		{
			_constructionInformation = constructionInformation;
			_placementGhost.Texture = constructionInformation.DisplayImageAtlas;
			_placementGhost.RegionRect = constructionInformation.DisplayImageRect;
			

			for (int i = 0; i < _optionSelectors.Length; i++)
			{
				_optionSelectors[i].Visible = false;
			}

			if (constructionInformation.ConfigurationType != GenericStructure.ConfigurationType.None)
			{
				
				int j = 0;
				Dictionary<string, string[]> configDictionary = GenericStructure.GetConfigurationOptions(constructionInformation.ConfigurationType);
				foreach (string key in configDictionary.Keys)
				{
					_optionSelectors[j].SetOptions(configDictionary[key], key);
					_optionSelectors[j].Visible = true;
					j++;
				}

			}


			_isEnabled = true;
			Visible = true;
		}
	}

	public void DisablePlacement()
	{
		_constructionInformation = null;
		_placementGhost.Texture = null;
		_isEnabled = false;
		Visible = false;

		EmitSignal(SignalName.OnPlacementStopped);
	}

    public override void _Process(double delta)
    {
		if (Input.IsActionJustPressed("ToggleTurretPlacementMode")) // TODO: remove.  This is just a placeholder before UI is integrated
		{
			infoIndex = (infoIndex+1) % temporaryConstructionInfo.Length;
			SetStructure(temporaryConstructionInfo[infoIndex]);
		}

		if (!_isEnabled) return;

        UpdatePosition();
		UpdatePlacementValidity();

		if (Input.IsActionJustPressed("Left Click"))
		{
			if (_isPlacementValid) PlaceStructure();
		}

		if (Input.IsActionJustPressed("Escape"))
		{
			// Cancel Placement
			DisablePlacement();
		}
    }

	private void UpdatePosition()
	{
		Vector2 mousePosition = GetViewport().GetMousePosition();
		_currentGridCoordinates = IsometricTileMap.GlobalPositionToMapCoord(_placementTilemap, mousePosition);;
		_placementGhost.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_placementTilemap, _currentGridCoordinates);
	}

	private void UpdatePlacementValidity()
	{
		GroundTile tile = _placementGrid.GetGridValueOrDefault(_currentGridCoordinates.X, _currentGridCoordinates.Y);
		if (tile == null || tile.HasStructure() || (_constructionInformation.PlacementRequirements == null && tile.HasRoadConnection())) 
		{
			_isPlacementValid = false;
		} 
		else if (_constructionInformation.PlacementRequirements != null && !_constructionInformation.PlacementRequirements.IsPlacementValid(_placementGrid, _currentGridCoordinates))
		{
			_isPlacementValid = false;
		}
		else if (_constructionInformation.MaterialRequirements != null && !_constructionInformation.MaterialRequirements.AreMaterialsAvailiable(_paymentInventory))
		{
			_isPlacementValid = false;
		} 
		else
		{
			_isPlacementValid = true;
		}

		_placementGhost.SelfModulate = _isPlacementValid ? _validPlacementColor : _invalidPlacementColor; 

	}

	private void PlaceStructure()
	{
		_constructionInformation.MaterialRequirements?.SpendMaterials(_paymentInventory);

		
		GenericStructure placedStructure = _constructionInformation.Structure.Instantiate<GenericStructure>();
		placedStructure.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_placementTilemap, _currentGridCoordinates);
		_placementGrid.GetGridValueOrDefault(_currentGridCoordinates.X, _currentGridCoordinates.Y).Structure = placedStructure;

		foreach (OptionSelector selector in _optionSelectors)
		{
			if (selector.Visible)
			{
				placedStructure.SetConfigurationOption(selector.OptionsName, selector.GetOptionSelection());
			}
		}
		
		placedStructure.Initialize(_constructionInformation.StructureStats);
		PlayArea.instance.AddChild(placedStructure);
		PlayArea.instance.Render();

	}

}
