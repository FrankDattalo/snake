using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Player : Node2D {	

	[Export]
	public PackedScene CellScene { get; set; }

	[Export]
	public int StartingSegmentCount { get; set; } = 100;

	private Vector2I direction = Vector2I.Zero;

	private readonly List<Cell> segments = new List<Cell>();

	public override void _Ready() {
		Random random = Random.Shared;
		Color[] colors = new Color[] { Utils.Colors.YELLOW, Utils.Colors.BLUE, Utils.Colors.GREEN, Utils.Colors.RED, };

		for (int i = 0; i < this.StartingSegmentCount; i++) {
			Cell cell = CellScene.Instantiate<Cell>();
			cell.SetPosition(new Vector2I(-i, 0));
			cell.Color = colors[random.Next(colors.Length)];
			// cell.Color = new Color(random.Next(256), random.Next(256), random.Next(256));
			segments.Add(cell);
			// ensure godot tracks it
			this.AddChild(cell);
		}

		this.direction = Vector2I.Right;
	}

	public override void _Process(double delta) {
		if (Input.IsActionPressed("UP") && this.direction != Vector2I.Down) {
			this.direction = Vector2I.Up;
		} else if (Input.IsActionPressed("DOWN") && this.direction != Vector2I.Up) {
			this.direction = Vector2I.Down;
		} else if (Input.IsActionPressed("LEFT") && this.direction != Vector2I.Right) {
			this.direction = Vector2I.Left;
		} else if (Input.IsActionPressed("RIGHT") && this.direction != Vector2I.Left) {
			this.direction = Vector2I.Right;
		}
	}

	public void Tick() {
		Vector2I direction = this.direction;
		Vector2 previousPosition = Vector2.Zero;
		bool first = true;
		foreach (Cell cell in this.segments) {
			Vector2 initialPosition = cell.Position;
			if (!first) {
				direction = DetermineDirection(previousPosition, cell);
			}
			cell.MovePosition(direction);
			first = false;
			previousPosition = initialPosition;
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
}
