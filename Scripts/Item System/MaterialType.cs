using Godot;
using System;

[GlobalClass]
public partial class MaterialType : Resource
{
    [Export] public Texture2D DisplayImageAtlas {get; private set;}
    [Export] public Rect2 DisplayImageRect {get; private set;}
}
