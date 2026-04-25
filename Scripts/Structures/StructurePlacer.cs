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
	GD.Print("SetStructure called, info is null: ", constructionInformation == null);
	
	if (constructionInformation == null)
	{
		DisablePlacement();
	} 
	else
	{
		GD.Print("Texture: ", constructionInformation.DisplayImageAtlas);
		GD.Print("Ghost node: ", _placementGhost);
		
		_constructionInformation = constructionInformation;
		_placementGhost.Texture = constructionInformation.DisplayImageAtlas;
		_placementGhost.RegionRect = constructionInformation.DisplayImageRect;
		
		_isEnabled = true;
		Visible = true;
		
		GD.Print("IsEnabled: ", _isEnabled, " Visible: ", Visible);
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
		Vector2 mouseWorldPos = GetGlobalMousePosition();
		
		//GD.Print("Mouse world pos: ", mouseWorldPos);
		//GD.Print("Ghost global pos: ", _placementGhost.GlobalPosition);
		//GD.Print("Camera zoom: ", GetViewport().GetCamera2D().Zoom);
		
		_currentGridCoordinates = IsometricTileMap.GlobalPositionToMapCoord(_placementTilemap, mouseWorldPos);
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
    GD.Print("MaterialRequirements null: ", _constructionInformation.MaterialRequirements == null);
    GD.Print("PaymentInventory null: ", _paymentInventory == null);

    
    if (_constructionInformation.MaterialRequirements != null)
    {
        bool spent = _constructionInformation.MaterialRequirements.SpendMaterials(_paymentInventory);
        GD.Print("SpendMaterials result: ", spent);
        if (!spent)
        {
            GD.Print("Not enough materials!");
            return;
        }
    }

    // Notify GameUI to update the coin label
    var gameUi = GetTree().GetRoot().GetNode<GameUi>("Main/GameUI");
    gameUi?.UpdateCoinDisplay();

    GenericStructure placedStructure = _constructionInformation.Structure.Instantiate<GenericStructure>();
    placedStructure.GlobalPosition = IsometricTileMap.MapCoordToGlobalPosition(_placementTilemap, _currentGridCoordinates);
    _placementGrid.GetGridValueOrDefault(_currentGridCoordinates.X, _currentGridCoordinates.Y).Structure = placedStructure;

    foreach (OptionSelector selector in _optionSelectors)
    {
        if (selector.Visible && selector.HasOptions)
        {
            placedStructure.SetConfigurationOption(selector.OptionsName, selector.GetOptionSelection());
        }
    }

    placedStructure.Initialize(_constructionInformation.StructureStats);

	if (placedStructure is Turret turret)
		{
			turret.HideRadius();
		}
    PlayArea.instance.AddChild(placedStructure);
	DisablePlacement(); 
}
}
