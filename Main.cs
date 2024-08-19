using Godot;
using System;

public partial class Main : Node2D {

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
		player.PendingDirection = dirs[GD.Randi() % dirs.Length];
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
		Food food = FoodScene.Instantiate<Food>();
		food.SetPosition(tileMap, RandomPosition());
		food.Player = player;
		AddChild(food);
	}

	private void OnKillZonesAreaEntered(Area2D node) {
		if (player.IsAncestorOf(node)) {
			NewGame();
		}
	}
}
