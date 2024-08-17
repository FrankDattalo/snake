using Godot;
using System;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata;

public partial class Cell : Node2D {

    public void SetPosition(Vector2I relative) {
    }

    public void MovePosition(Vector2I relative) {
    }

    public override void _Ready() {
        ColorRect cellColor = GetNode<ColorRect>("CellColor");
        cellColor.Size = new Godot.Vector2(this.CellSize, this.CellSize);
        cellColor.Color = this.Color;

        // ensure the collider has the same dimensions
        CollisionShape2D collisionShape = GetNode<CollisionShape2D>("CellBody/CellShape");
        collisionShape.Position = new Godot.Vector2(this.CellSize, this.CellSize);
    }
}
