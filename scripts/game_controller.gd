extends Node

## Shows the win screen when the sphere lands on the plane (detected via signal).

@onready var _win_screen: Control = $"../UI/WinScreen"


func show_win_screen() -> void:
	_win_screen.visible = true
