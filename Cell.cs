using Godot;
using System;

public partial class Cell : Node2D {

    private ColorRect cell;
    public Color? Color { get; set; } = null;

    public Player Player { get; set; }

    public bool HasMoved { get; private set; } = false;

    public bool Moving { get; private set; } = false;

    public bool IsHead { get; set; } = false;

    private Area2D collsionArea;

    private ColorRect movementCell;

    public override void _Ready() {
        cell = GetNode<ColorRect>("CellColor");
        movementCell = GetNode<ColorRect>("CellColor/Movement");
        collsionArea = GetNode<Area2D>("Area2D");

        if (Color != null) {
            cell.Color = (Color) Color;
            movementCell.Color = (Color) Color;
        }
    }

    public void SetPosition(TileMapLayer tileMap, Vector2I position) {
        Position = tileMap.MapToLocal(position);
    }

    public void MovePosition(TileMapLayer tileMap, Vector2I direction) {
        Moving = true;
        Vector2 collisionAreaPosition = direction * 16;
        Vector2 nextPosition = tileMap.MapToLocal(NextPosition(tileMap, direction));
        Vector2 fromNextPosition = nextPosition - collsionArea.Position;
        collsionArea.Position = collisionAreaPosition;
        if (!IsHead) {
            movementCell.Position = collisionAreaPosition;
        }
        Tween tween = CreateTween();
        double duration = 0.1;
        tween.Parallel().TweenProperty(this, "position", nextPosition, duration);
        tween.Parallel().TweenProperty(collsionArea, "position", Vector2.Zero, duration);
        tween.Parallel().TweenProperty(movementCell, "position", Vector2.Zero, duration);
        tween.TweenCallback(Callable.From(MovementDone));
    }

    private void MovementDone() {
        Moving = false;
        HasMoved = true;
    }

    private Vector2I NextPosition(TileMapLayer tileMap, Vector2I direction) {
        return tileMap.LocalToMap(Position)  + direction;
    }

    public void OnCellCollision(Area2D other) {
        if (!IsHead) {
            return;
        }

        if (HasMoved && Player.IsAncestorOf(other)) {
            Player.OnSelfCollision();
        }
    }
}
