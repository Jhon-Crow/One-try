extends Node3D

## Root script on the PlayerMannequin scene.
##
## Responsibilities in this iteration:
##   - Play the idle animation loop when the game starts.
##   - Expose a clean hook for future locomotion / combat controllers.

@export var rotation_speed: float = 10.0

@onready var _anim_player: AnimationPlayer = $AnimationPlayer


func _ready() -> void:
	_anim_player.play("idle")


## Called by the future locomotion controller to face a direction.
## No-op in the mannequin/idle-only state.
func face_direction(world_direction: Vector3) -> void:
	if world_direction.length_squared() < 0.001:
		return
	var target_basis := Basis.looking_at(world_direction, Vector3.UP)
	var target_quat := target_basis.get_rotation_quaternion()
	var current_quat := Quaternion(global_basis)
	global_basis = Basis(current_quat.slerp(target_quat, get_process_delta_time() * rotation_speed))
