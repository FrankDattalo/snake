using Godot;
using System;

public partial class Player : Node2D
{	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 dir = Vector2.Zero;
		System.Console.WriteLine("This is new");
		if (Input.IsActionPressed("UP")) {
			dir = Vector2.Up;
		} else if (Input.IsActionPressed("DOWN")) {
			dir = Vector2.Down;
		} else if (Input.IsActionPressed("LEFT")) {
			dir = Vector2.Left;
		} else if (Input.IsActionPressed("RIGHT")) {
			dir = Vector2.Right;
		}
		this.Position += dir * 10;
	}
}
