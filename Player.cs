using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Player : Node2D {	

	[Export]
	public PackedScene CellScene { get; set; }

	private Vector2I direction = Vector2I.Zero;

	private readonly List<Cell> segments = new List<Cell>();

	public IEnumerable<Cell> Cells {
		get {
			return segments;
		}
	}

	public override void _Ready() {
		Cell cell = CellScene.Instantiate<Cell>();
		cell.Color = new Color(0x0043150); // TODO refactor - magic number
		cell.ZIndex = 3; // TODO refactor - magic number
		segments.Add(cell);
		this.AddChild(cell);

		this.direction = Vector2I.Right;
	}

	public override void _Process(double delta) {
		if (Input.IsActionPressed("UP") && (this.direction != Vector2I.Down || this.segments.Count == 1)) {
			this.direction = Vector2I.Up;
		} else if (Input.IsActionPressed("DOWN") && (this.direction != Vector2I.Up || this.segments.Count == 1)) {
			this.direction = Vector2I.Down;
		} else if (Input.IsActionPressed("LEFT") && (this.direction != Vector2I.Right || this.segments.Count == 1)) {
			this.direction = Vector2I.Left;
		} else if (Input.IsActionPressed("RIGHT") && (this.direction != Vector2I.Left || this.segments.Count == 1)) {
			this.direction = Vector2I.Right;
		}
	}

	public void SetPosition(TileMapLayer tileMap, Vector2I tileMapPosition) {
		foreach (Cell cell in this.segments) {
			cell.SetPosition(tileMap, tileMapPosition);
		}
	}

	public void Tick(TileMapLayer tileMap) {
		Vector2I direction = this.direction;
		Vector2 previousPosition = Vector2.Zero;
		bool first = true;
		int growCount = 0;
		foreach (Cell cell in this.segments) {
			Vector2 initialPosition = cell.Position;
			if (first) {
				// head movement, detect collision
				Vector2I targetPosition = cell.NextPosition(tileMap, direction);
				if (Utils.Tiles.TileContainsCell(tileMap, targetPosition)) {
					EmitSignal(SignalName.OnPlayerDied);

				} else if (Utils.Tiles.TileContainsFood(tileMap, targetPosition)) {
					// clear the food
					Utils.Tiles.ClearTile(tileMap, targetPosition);
					// grow the snake
					growCount++;
					cell.MovePosition(tileMap, direction);
				} else {
					cell.MovePosition(tileMap, direction);
				}
			} else {
				// tail movement
				direction = DetermineDirection(previousPosition, cell);
				cell.MovePosition(tileMap, direction);
			}
			first = false;
			previousPosition = initialPosition;
		}
		for (int i = 0; i < growCount; i++) {
			Cell cell = CellScene.Instantiate<Cell>();
			cell.Position = previousPosition;
			segments.Add(cell);
			this.AddChild(cell);
		}
	}

	private Vector2I DetermineDirection(Vector2 previous, Cell current) {
		Vector2 delta = previous - current.Position;
		if (delta.X > 0) {
			return Vector2I.Right;
		}
		if (delta.X < 0) {
			return Vector2I.Left;
		}
		if (delta.Y > 0) {
			return Vector2I.Down;
		}
		if (delta.Y < 0) {
			return Vector2I.Up;
		}
		throw new Exception(string.Format("Could not determine direction prev %s cur %s", previous, current));
	}

	[Signal]
	public delegate void OnPlayerDiedEventHandler();
}
