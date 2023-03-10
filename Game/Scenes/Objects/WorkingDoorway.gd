extends Node3D

@export_range(1.0, 360.0) var max_wait_time := 45.0
@export_range(1.0, 360.0) var min_wait_time := 15.0

@export var is_open := false
@export var is_busy := false

@onready var anim := $AnimationPlayer
@onready var auto_close := $AutoCloseTimer

func _interact() -> void:
	if is_busy:
		return

	if is_open:
		anim.play("Close")
	else:
		anim.play("Open")
		var time = randf_range(min_wait_time, max_wait_time)
		auto_close.start(time)

func _on_auto_close_timer_timeout() -> void:
	if is_busy:
		return
	anim.play("Close")

func _on_door_rigid_body_on_request_open() -> void:
	if not is_open:
		_interact()
