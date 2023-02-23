extends Node3D

@export_range(1.0, 360.0) var max_wait_time := 45.0
@export_range(1.0, 360.0) var min_wait_time := 15.0

var is_open := false
@onready var anim := $AnimationPlayer
@onready var auto_close := $AutoCloseTimer

func _interact() -> void:
	if anim.is_playing():
		return

	if is_open:
		anim.play_backwards("Open")
	else:
		anim.play("Open")
		var time = randf_range(min_wait_time, max_wait_time)
		auto_close.start(time)
	await anim.animation_finished
	is_open = !is_open



func _on_auto_close_timer_timeout() -> void:
	if anim.is_playing():
		return
	anim.play_backwards("Open")
	await anim.animation_finished
	is_open = false
	
	
