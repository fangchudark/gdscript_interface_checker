# This example class implementation is broken.
#   - It is supposed to implement IDamageable, but get_is_dead() must return a bool.
#   - It is supposed to implement IRespawnable, but respawn() has incorrect signature.
extends CharacterBody2D

enum ENUM_STATUS {ALIVE, DEAD}

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

# Broken implementation; should return a bool.
func get_is_dead() -> ENUM_STATUS:
	return ENUM_STATUS.DEAD if _is_dead else ENUM_STATUS.ALIVE

# Broken implementation; should accept Array[Marker2D], and return a selected Marker2D
func respawn(possible_spawns: Array[SpawnPoint]) -> CharacterBody2D:
	var spawn_point: SpawnPoint = possible_spawns.pick_random()
	var copy: CharacterBody2D = self.duplicate() as CharacterBody2D
	copy.position = spawn_point.position
	return copy

class SpawnPoint extends Node:
	var label: String = "Alien Hive"
