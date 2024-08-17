using Godot;
using System;

public partial class Main : Node2D {

	[Export]
	public PackedScene FoodScene { get; set; }

	private Player player;

	private Vector2I viewportSize;

	private Random random = Random.Shared;

	public override void _Ready() {
		this.player = GetNode<Player>("Player");

		Timer movementTimer = GetNode<Timer>("MovementTimer");
		Timer foodTimer = GetNode<Timer>("FoodTimer");

		Vector2 viewportSizeFl = GetViewportRect().Size;
		viewportSize = new Vector2I((int) viewportSizeFl.X, (int) viewportSizeFl.Y);

		movementTimer.Start();
		foodTimer.Start();
	}

	private void OnMovementTick() {
		this.player.Tick();
	}

	private void OnFoodTick() {
		Console.WriteLine("Food tick");
		int xMax = viewportSize.X / 20;
		int yMax = viewportSize.Y / 20;
		Vector2I pos = new Vector2I(random.Next(xMax), random.Next(yMax));

		Food food = FoodScene.Instantiate<Food>();
		food.Position = pos;
	}
}
