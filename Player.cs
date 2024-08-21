using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Node2D {	

	[Export]
	public PackedScene CellScene { get; set; }

	private Vector2I PendingDirection { get; set; } = Vector2I.Right;
	private Main main;
	private Vector2I committedDirection = Vector2I.Zero;

	private readonly List<Cell> segments = new List<Cell>();
	private Cell head;
	private int pendingGrowth = 0;


	public int SegmentCount {
		get {
			return segments.Count;
		}
	}

	private bool Moving() {
		return segments.Any((s) => s.Moving);
	}

	public void Initialize(Main main) {
		this.main = main;
	}

	public void OnGameStart(Vector2I position) {
		Vector2I[] dirs = new Vector2I[]{ Vector2I.Left, Vector2I.Right, Vector2I.Up, Vector2I.Down };
		PendingDirection = dirs[GD.Randi() % dirs.Length];
		// TODO: handle this better
		ClearSegments();
		head = CellScene.Instantiate<Cell>();
		head.Player = this;
		//head.Color = new Color(0x0043150); // TODO refactor - magic number
		head.Color = new Color(0xffffffff); // TODO refactor - magic number
		head.IsHead = true;
		head.ZIndex = 3; // TODO refactor - magic number
		segments.Add(head);
		this.AddChild(head);
		head.SetPosition(main.TileMap, position);
	}

	public override void _Process(double delta) {
		if (!main.GameStarted) {
			return;
		}
		if (Input.IsActionPressed("UP") && (committedDirection != Vector2I.Down || this.segments.Count == 1)) {
			PendingDirection = Vector2I.Up;
		} else if (Input.IsActionPressed("DOWN") && (committedDirection != Vector2I.Up || this.segments.Count == 1)) {
			PendingDirection = Vector2I.Down;
		} else if (Input.IsActionPressed("LEFT") && (committedDirection != Vector2I.Right || this.segments.Count == 1)) {
			PendingDirection = Vector2I.Left;
		} else if (Input.IsActionPressed("RIGHT") && (committedDirection != Vector2I.Left || this.segments.Count == 1)) {
			PendingDirection = Vector2I.Right;
		}
		if (Moving()) {
			return;
		}
		Move();
	}

	public void ClearSegments() {
		foreach (Cell segment in segments) {
			segment.QueueFree();
		}
		segments.Clear();
		head = null;
	}

	private void Move() {
		TileMapLayer tileMap = main.TileMap;
		committedDirection = PendingDirection;
		Vector2I? previousPosition = null;
		foreach (Cell cell in this.segments) {
			Vector2I initialPosition = tileMap.LocalToMap(cell.Position);
			if (previousPosition == null) {
				// head movement
				cell.MovePosition(tileMap, committedDirection);
			} else {
				// tail movement
				Vector2I position = tileMap.LocalToMap(cell.Position);
				Vector2I direction = ((Vector2I) previousPosition) - position;
				cell.MovePosition(tileMap, direction);
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
