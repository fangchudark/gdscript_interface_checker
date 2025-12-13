extends Node

func _ready() -> void:
	var script = load("res://example/enemy.gd")

	var interface : Array[GDSMethodInfo] = [
		GDSMethodInfo.new("take_damage", 1, [""], [TYPE_INT], "", TYPE_NIL, [], PROPERTY_USAGE_DEFAULT),
		# MethodInfo.void_method("take_damage", 1, [""], [TYPE_INT]),
		GDSMethodInfo.new("get_is_dead", 0, [], [], "", TYPE_BOOL, ["@is_dead_getter"]),
		# MethodInfo.getter("is_dead", "", TYPE_BOOL)
	]

	print(GDSInterfaceChecker.is_implemented_interface(script, interface))
