using Godot;
using System;

public partial class Food : Node2D {

    public Main Main { get; set; }
    public Player Player { get; set; }

    private AnimationPlayer animationPlayer;

    public override void _Ready() {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void SetPosition(TileMapLayer tileMap, Vector2I position) {
        Position = tileMap.MapToLocal(position);
    }

    public void OnFoodCollision(Area2D area2D) {
        if (Player.IsAncestorOf(area2D)) {
            Player.OnFoodCollision();
            Main.OnFoodCollision();
            animationPlayer.Play("FoodEaten");
        }
    }
}
