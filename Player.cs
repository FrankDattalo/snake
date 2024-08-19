using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Node2D {	

	[Export]
	public PackedScene CellScene { get; set; }

	public Vector2I PendingDirection { get; set; } = Vector2I.Right;
	private Vector2I committedDirection = Vector2I.Zero;

	private readonly List<Cell> segments = new List<Cell>();
	private Cell head;
	private int pendingGrowth = 0;

	public IEnumerable<Cell> Cells {
		get {
			return segments;
		}
	}

	public override void _Ready() {
		head = CellScene.Instantiate<Cell>();
		head.Player = this;
		head.Color = new Color(0x0043150); // TODO refactor - magic number
		head.ZIndex = 3; // TODO refactor - magic number
		segments.Add(head);
		this.AddChild(head);
	}

	public override void _Process(double delta) {
		if (Input.IsActionPressed("UP") && (committedDirection != Vector2I.Down || this.segments.Count == 1)) {
			PendingDirection = Vector2I.Up;
		} else if (Input.IsActionPressed("DOWN") && (committedDirection != Vector2I.Up || this.segments.Count == 1)) {
			PendingDirection = Vector2I.Down;
		} else if (Input.IsActionPressed("LEFT") && (committedDirection != Vector2I.Right || this.segments.Count == 1)) {
			PendingDirection = Vector2I.Left;
		} else if (Input.IsActionPressed("RIGHT") && (committedDirection != Vector2I.Left || this.segments.Count == 1)) {
			PendingDirection = Vector2I.Right;
		}
	}

	public void DropTail() {
		segments.RemoveAll((Cell cell) => {
			if (cell != head) {
				cell.QueueFree();
				return true;
			} else {
				return false;
			}
		});
	}

	public void SetPosition(TileMapLayer tileMap, Vector2I tileMapPosition) {
		foreach (Cell cell in this.segments) {
			cell.SetPosition(tileMap, tileMapPosition);
		}
	}

	public void Tick(TileMapLayer tileMap) {
		committedDirection = PendingDirection;
		Vector2I? previousPosition = null;
		foreach (Cell cell in this.segments) {
			cell.HasMoved = true;
			Vector2I initialPosition = tileMap.LocalToMap(cell.Position);
			if (previousPosition == null) {
				// head movement
				Vector2I targetPosition = cell.NextPosition(tileMap, committedDirection);
				cell.SetPosition(tileMap, targetPosition);
			} else {
				// tail movement
				cell.SetPosition(tileMap, (Vector2I) previousPosition);
			}
			previousPosition = initialPosition;
		}
		if (pendingGrowth > 0 && segments.Last().HasMoved) {
			Cell cell = CellScene.Instantiate<Cell>();
			cell.Player = this;
			cell.Position = tileMap.MapToLocal((Vector2I) previousPosition);
			segments.Add(cell);
			this.AddChild(cell);
			pendingGrowth--;
		}
	}

	public void OnFoodCollision() {
		pendingGrowth++;
	}

	public void OnSelfCollision() {
		EmitSignal(SignalName.OnPlayerDied);
	}

	[Signal]
	public delegate void OnPlayerDiedEventHandler();
}
