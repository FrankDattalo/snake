using Godot;
using System;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata;

public partial class Food : Node2D {

    private Cell cell;
    public Vector2I PositionI { get; set; }

    public override void _Ready() {
        cell = GetNode<Cell>("FoodCell");
        cell.Color = Utils.Colors.YELLOW;
        cell.SetPosition(PositionI);
    }

    public override void _Process(double delta) {
    }
}
