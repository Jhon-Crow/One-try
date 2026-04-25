extends Area3D

## Fires when the sphere enters this trigger area (placed on the plane).

signal object_entered


func _on_body_entered(_body: Node3D) -> void:
	emit_signal("object_entered")
