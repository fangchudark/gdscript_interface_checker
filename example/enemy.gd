# implements IDamageable
class_name Enemy
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
