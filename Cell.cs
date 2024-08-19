using Godot;
using System;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata;

public partial class Cell : Node2D {

    private ColorRect cell;
    public Color? Color { get; set; } = null;

    public override void _Ready() {
        cell = GetNode<ColorRect>("CellColor");

        if (Color != null) {
            cell.Color = (Color) Color;
        }
    }

    public void SetPosition(TileMapLayer tileMap, Vector2I position) {
        Position = tileMap.MapToLocal(position);
    }

    public void MovePosition(TileMapLayer tileMap, Vector2I direction) {
        SetPosition(tileMap, NextPosition(tileMap, direction));
    }

    public Vector2I NextPosition(TileMapLayer tileMap, Vector2I direction) {
        return tileMap.LocalToMap(Position)  + direction;
    }
}
