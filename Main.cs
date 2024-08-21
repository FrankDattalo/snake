using Godot;
using System;

public partial class Main : Node2D {

	[Export]
	public PackedScene FoodScene { get; set; }

	public TileMapLayer TileMap { get; set; }

	private Player player;

	private Vector2 viewportSize;

	private Control menuUi;
	private Control gameUi;
	private Timer foodTimer;

	private Label scoreCounter;

	private int highScore = 0;

	private int activeFood = 0;

	private int MAX_ACTIVE_FOOD_COUNT = 1000;

	public bool GameStarted { get; private set; } = false;

	public override void _Ready() {
		viewportSize = GetViewportRect().Size;

		TileMap = GetNode<TileMapLayer>("MainLayer");
		player = GetNode<Player>("Player");
		menuUi = GetNode<Control>("MenuUI");
		gameUi = GetNode<Control>("GameUI");
		foodTimer = GetNode<Timer>("FoodTimer");

		scoreCounter = GetNode<Label>("GameUI/ScoreCounter");

		gameUi.Visible = true;
		menuUi.Visible = true;

		player.Initialize(this);
	}

	private Vector2I RandomPosition() {
		Vector2 position = new Vector2(
			GD.Randf() * viewportSize.X,
			GD.Randf() * viewportSize.Y);

		return TileMap.LocalToMap(position);
	}

	private void GameOver() {
		menuUi.Visible = true;
		gameUi.Visible = true;
		foodTimer.Stop();
		GameStarted = false;
	}

	private void NewGame() {
		menuUi.Visible = false;
		gameUi.Visible = true;

		player.OnGameStart(RandomPosition());

		GetTree().CallGroup("AllFood", Node.MethodName.QueueFree);
		activeFood = 0;

		foodTimer.Start();

		GameStarted = true;
	}

	private void QuitGame() {
		GetTree().Quit();
	}

	override public void _Process(double delta) {
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
		food.SetPosition(TileMap, RandomPosition());
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
