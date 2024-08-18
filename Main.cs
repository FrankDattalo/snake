using Godot;
using System;

public partial class Main : Node2D {

	private static readonly int MAX_FOOD_ATTEMPTS = 1000;

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

		for (int attempt = 0; attempt < MAX_FOOD_ATTEMPTS; attempt++) {

			Vector2 position = new Vector2(
				(float) random.NextDouble() * viewportSize.X,
				(float) random.NextDouble() * viewportSize.Y);

			Vector2I tileMapPosition = tileMap.LocalToMap(position);

			if (IsTileEmpty(tileMapPosition)) {

				tileMap.SetCell(
					Utils.Tiles.TILE_MAP_LAYER,
					tileMapPosition,
					Utils.Tiles.TILE_MAP_SOURCE_ID,
					/* atlas coords */ Vector2I.Zero,
					Utils.Tiles.FOOD_TILE_ALT_ID);

				return;
			}
		}
	}

	private bool IsTileEmpty(Vector2I tileMapPosition) {
		if (!Utils.Tiles.TileIsEmpty(tileMap, tileMapPosition)) {
			return false;
		}
		foreach (Cell cell in player.Cells) {
			Vector2I cellPosition = tileMap.LocalToMap(cell.Position);
			if (cellPosition == tileMapPosition) {
				return false;
			}
		}
		return true;
	}
}
