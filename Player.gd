extends Node2D

func _ready() -> void:
	self.position = Vector2(0, 0)

func _process(_delta: float) -> void:
	var dir : Vector2 = Vector2.ZERO
	if Input.is_action_pressed("UP"):
		dir = Vector2.UP
	elif Input.is_action_pressed("DOWN"):
		dir = Vector2.DOWN
	elif Input.is_action_pressed("LEFT"):
		dir = Vector2.LEFT
	elif Input.is_action_pressed("RIGHT"):
		dir = Vector2.RIGHT
	self.position += dir * 10