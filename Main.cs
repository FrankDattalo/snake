using Godot;
using System;

public partial class Main : Node2D {

	[Export]
	public PackedScene FoodScene { get; set; }

	private TileMapLayer tileMap;

	private Player player;

	private Vector2 viewportSize;

	private Control menuUi;
	private Control gameUi;
	private Timer movementTimer;
	private Timer foodTimer;

	private Label scoreCounter;

	private int highScore = 0;

	private int activeFood = 0;

	private int MAX_ACTIVE_FOOD_COUNT = 10;

	public override void _Ready() {
		viewportSize = GetViewportRect().Size;

		tileMap = GetNode<TileMapLayer>("MainLayer");
		player = GetNode<Player>("Player");
		menuUi = GetNode<Control>("MenuUI");
		gameUi = GetNode<Control>("GameUI");
		movementTimer = GetNode<Timer>("MovementTimer");
		foodTimer = GetNode<Timer>("FoodTimer");

		scoreCounter = GetNode<Label>("GameUI/ScoreCounter");

		gameUi.Visible = true;
		menuUi.Visible = true;
	}

	private Vector2I RandomPosition() {
		Vector2 position = new Vector2(
			GD.Randf() * viewportSize.X,
			GD.Randf() * viewportSize.Y);

		return tileMap.LocalToMap(position);
	}

	private void GameOver() {
		menuUi.Visible = true;
		gameUi.Visible = true;
		movementTimer.Stop();
		foodTimer.Stop();
	}

	private void NewGame() {
		menuUi.Visible = false;
		gameUi.Visible = true;

		Vector2I[] dirs = new Vector2I[]{ Vector2I.Left, Vector2I.Right, Vector2I.Up, Vector2I.Down };
		player.PendingDirection = dirs[GD.Randi() % dirs.Length];
		player.OnGameStart(tileMap, RandomPosition());

		GetTree().CallGroup("AllFood", Node.MethodName.QueueFree);
		activeFood = 0;

		movementTimer.Start();
		foodTimer.Start();
	}

	private void QuitGame() {
		GetTree().Quit();
	}

	private void OnMovementTick() {
		this.player.Tick(tileMap);
		int segmentCount = this.player.SegmentCount;
		this.highScore = Math.Max(this.highScore, segmentCount);
		this.scoreCounter.Text = $"Score: {segmentCount}\nHigh: {highScore}";
	}

	private void OnFoodTick() {
		if (activeFood >= MAX_ACTIVE_FOOD_COUNT) {
			return;
		}
		activeFood++;
		Food food = FoodScene.Instantiate<Food>();
		food.SetPosition(tileMap, RandomPosition());
		food.Main = this;
		food.Player = player;
		AddChild(food);
	}

	public void OnFoodCollision() {
		activeFood--;
	}

	private void OnKillZonesAreaEntered(Area2D node) {
		if (player.IsAncestorOf(node)) {
			GameOver();
		}
	}
}
