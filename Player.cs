using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Node2D {	

	[Export]
	public PackedScene CellScene { get; set; }

	private Main main;
	private Vector2I committedDirection = Vector2I.Right;

	private readonly List<Cell> segments = new List<Cell>();
	private Cell head;
	private int pendingGrowth = 0;

	private readonly Queue<Vector2I> queuedInput = new Queue<Vector2I>();

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
		QueuePendingDirection(dirs[GD.Randi() % dirs.Length]);
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

		if (Input.IsActionJustPressed("UP")) {
			QueuePendingDirection(Vector2I.Up);
		}

		if (Input.IsActionJustPressed("DOWN")) {
			QueuePendingDirection(Vector2I.Down);
		}

		if (Input.IsActionJustPressed("LEFT")) {
			QueuePendingDirection(Vector2I.Left);
		}

		if (Input.IsActionJustPressed("RIGHT")) {
			QueuePendingDirection(Vector2I.Right);
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
		committedDirection = TakePendingDirection();
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

	private void QueuePendingDirection(Vector2I dir) {
		queuedInput.Enqueue(dir);
	}

	private Vector2I TakePendingDirection() {
		if (queuedInput.Count == 0) {
			return committedDirection;
		}
		Vector2I desiredDirection = queuedInput.Dequeue();
		bool moveValid = segments.Count == 1 ||
			(desiredDirection == Vector2I.Left && committedDirection != Vector2I.Right) ||
			(desiredDirection == Vector2I.Right && committedDirection != Vector2I.Left) ||
			(desiredDirection == Vector2I.Up && committedDirection != Vector2I.Down) ||
			(desiredDirection == Vector2I.Down && committedDirection != Vector2I.Up);
		if (!moveValid) {
			return committedDirection;
		}
		return desiredDirection;
	}
}
