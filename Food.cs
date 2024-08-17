using Godot;
using System;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata;

public partial class Food : Node2D {

    public void SetPosition(TileMap tileMap, Vector2I position) {
        Position = tileMap.MapToLocal(position);
    }
}
