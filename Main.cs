using Godot;
using System;

public partial class Main : Node2D {

	private static readonly int MAX_FOOD_ATTEMPTS = 10;

	[Export]
	public PackedScene FoodScene { get; set; }

	private TileMapLayer tileMap;

	private Player player;

	private Vector2 viewportSize;

	public override void _Ready() {
		viewportSize = GetViewportRect().Size;

		tileMap = GetNode<TileMapLayer>("MainLayer");
		player = GetNode<Player>("Player");

		NewGame();
	}

	private Vector2I RandomPosition() {
		Vector2 position = new Vector2(
			GD.Randf() * viewportSize.X,
			GD.Randf() * viewportSize.Y);

		return tileMap.LocalToMap(position);
	}

	private void NewGame() {
		player.SetPosition(tileMap, RandomPosition());
		Vector2I[] dirs = new Vector2I[]{ Vector2I.Left, Vector2I.Right, Vector2I.Up, Vector2I.Down };
		player.Direction = dirs[GD.Randi() % dirs.Length];
		player.DropTail();

		Timer movementTimer = GetNode<Timer>("MovementTimer");
		Timer foodTimer = GetNode<Timer>("FoodTimer");

		GetTree().CallGroup("AllFood", Node.MethodName.QueueFree);

		movementTimer.Start();
		foodTimer.Start();
	}

	private void OnMovementTick() {
		this.player.Tick(tileMap);
	}

	private void OnFoodTick() {
		for (int attempt = 0; attempt < MAX_FOOD_ATTEMPTS; attempt++) {

			Vector2I tileMapPosition = RandomPosition();

			if (IsTileEmpty(tileMapPosition)) {

				tileMap.SetCell(
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

	private void OnKillZonesAreaEntered(Area2D node) {
		if (player.IsAncestorOf(node)) {
			NewGame();
		}
	}
}
