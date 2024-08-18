using Godot;
using System;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata;

public partial class Cell : Node2D {

    public void SetPosition(TileMap tileMap, Vector2I position) {
        Position = tileMap.MapToLocal(position);
    }

    public void MovePosition(TileMap tileMap, Vector2I direction) {
        SetPosition(tileMap, NextPosition(tileMap, direction));
    }

    public Vector2I NextPosition(TileMap tileMap, Vector2I direction) {
        return tileMap.LocalToMap(Position)  + direction;
    }
}
