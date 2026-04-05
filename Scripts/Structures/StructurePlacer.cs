using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class StructurePlacer : Node2D
{
	[Export] private Sprite2D _placementGhost;
	[Export] Color _validPlacementColor, _invalidPlacementColor;
	[Export] BoxContainer _selectorContainer;
	private OptionSelector[] _optionSelectors;

	private ConstructionInformation _constructionInformation;
	private Inventory _paymentInventory;
	private GenericGrid<GroundTile> _placementGrid;
	private Vector2I _currentGridCoordinates;
	private bool _isPlacementValid;

    public override void _Ready()
    {
		List<OptionSelector> selectors = [];
        foreach (Node child in _selectorContainer.GetChildren())
		{
			GD.Print(child);
			if (child is OptionSelector selector)
			{
				selectors.Add(selector);
			}
		}
		_optionSelectors = [.. selectors];
    }


	public void Initialize(GenericGrid<GroundTile> targetGrid, ConstructionInformation constructionInformation, Inventory paymentInventory)
	{
		_placementGrid = targetGrid;
		_constructionInformation = constructionInformation;
		_paymentInventory = paymentInventory;
		_placementGhost.Texture = constructionInformation.DisplayImage;

		for (int i = 0; i < _optionSelectors.Length; i++)
		{
			_optionSelectors[i].Visible = false;
		}
		int j = 0;
		foreach (string key in constructionInformation.ConfigurationOptions.Keys)
		{
			GD.Print($"Displaying Key: {key}  Selectors: {_optionSelectors.Length} j: {j}");
			_optionSelectors[j].SetOptions(constructionInformation.ConfigurationOptions[key], key);
			_optionSelectors[j].Visible = true;
			j++;
		}
	}
    public override void _Process(double delta)
    {
        UpdatePosition();
		UpdatePlacementValidity();

		if (Input.IsActionJustPressed("Left Click"))
		{
			if (_isPlacementValid) PlaceStructure();
		}
    }

	private void UpdatePosition()
	{
		Vector2 mousePosition = GetViewport().GetMousePosition();
		_currentGridCoordinates = (Vector2I) (mousePosition / _placementGrid.cellSize).Clamp(Vector2I.Zero, _placementGrid.GetGridDimensions());
		_placementGhost.Position = (Vector2) _currentGridCoordinates * _placementGrid.cellSize;
	}

	private void UpdatePlacementValidity()
	{
		GroundTile tile = _placementGrid.GetGridValueOrDefault(_currentGridCoordinates.X, _currentGridCoordinates.Y);
		if (tile == null || tile.HasRoadConnection() || tile.HasStructure()) 
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
		placedStructure.GlobalPosition = _placementGrid.GetCentralGridCellPositionPixels(_currentGridCoordinates);
		_placementGrid.GetGridValueOrDefault(_currentGridCoordinates.X, _currentGridCoordinates.Y).Structure = placedStructure;

		PlayArea.instance.AddChild(placedStructure);
	}

}
