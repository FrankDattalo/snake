using Godot;
using System;

public partial class Main : Node2D {

	[Export]
	public PackedScene FoodScene { get; set; }

	private TileMap tileMap;

	private Player player;

	private Vector2 viewportSize;

	public override void _Ready() {
		viewportSize = GetViewportRect().Size;

		tileMap = GetNode<TileMap>("TileMap");
		player = GetNode<Player>("Player");

		Vector2I startingTileMapPosition = tileMap.LocalToMap(Vector2.Zero);
		player.SetPosition(tileMap, startingTileMapPosition);

		Timer movementTimer = GetNode<Timer>("MovementTimer");
		Timer foodTimer = GetNode<Timer>("FoodTimer");

		movementTimer.Start();
		foodTimer.Start();
	}

	private void OnMovementTick() {
		this.player.Tick(tileMap);
	}

	private void OnFoodTick() {
		Random random = Random.Shared;
		Vector2 position = new Vector2((
			float) random.NextDouble() * viewportSize.X,
			(float) random.NextDouble() * viewportSize.Y);
		Food food = FoodScene.Instantiate<Food>();
		Vector2I tileMapPosition = tileMap.LocalToMap(position);
		food.SetPosition(tileMap, tileMapPosition);
		AddChild(food);

	}
}
