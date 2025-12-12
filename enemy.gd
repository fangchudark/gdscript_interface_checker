# implements IDamageable, IRespawnable
extends CharacterBody2D

var is_dead: bool:
	get:
		return _is_dead

var _is_dead: bool = false
var _health: int = 100

func take_damage(damage: int) -> void:
	if _is_dead:
		return
	_health -= damage
	if _health <= 0:
		_is_dead = true
		print("Enemy is dead")
	else:
		print("Enemy took damage, remaining health: ", _health)

func get_is_dead() -> bool:
	return _is_dead

func respawn(possible_spawns: Array[Marker2D]) -> Marker2D:
	var spawn_point: Marker2D = possible_spawns.pick_random()
	self.reparent(spawn_point)
	_health = 100
	_is_dead = false
	return spawn_point
