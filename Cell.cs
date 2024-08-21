using Godot;
using System;

public partial class Cell : Node2D {

    private ColorRect cell;
    public Color? Color { get; set; } = null;

    public Player Player { get; set; }

    public bool HasMoved { get; set; } = false;

    public bool Moving { get; set; } = false;

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
        if (Moving) return;
        Moving = true;
        Vector2 nextPosition = tileMap.MapToLocal(NextPosition(tileMap, direction));
        Tween tween = CreateTween();
        tween.TweenProperty(this, "position", nextPosition, 0.1);
        tween.TweenCallback(Callable.From(MovementDone));
    }

    private void MovementDone() {
        Moving = false;
    }

    private Vector2I NextPosition(TileMapLayer tileMap, Vector2I direction) {
        return tileMap.LocalToMap(Position)  + direction;
    }

    public void OnCellCollision(Area2D other) {
        if (HasMoved && Player.IsAncestorOf(other)) {
            // bypass for now
            // Player.OnSelfCollision();
        }
    }
}
