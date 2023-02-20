extends Node3D

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
		auto_close.start()
	await anim.animation_finished
	is_open = !is_open



func _on_auto_close_timer_timeout() -> void:
	if anim.is_playing():
		return
	anim.play_backwards("Open")
	await anim.animation_finished
	is_open = !is_open
	
	
