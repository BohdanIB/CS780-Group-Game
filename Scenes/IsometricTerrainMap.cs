
using Godot;

public partial class IsometricTerrainMap : Node2D
{
    [Export] private TileMapLayer[] _layers;

    public TileMapLayer[] GetLayers()
    {
        return _layers;
    }

    /// <summary>
    /// Clear all layers in terrain map
    /// </summary>
    public void Clear()
    {
        foreach (var layer in _layers)
        {
            layer.Clear();
        }
    }

}
