extends Node

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var script = load("res://enemy.gd")
	var broken_script = load("res://enemy_broken.gd")
	var i_damageable_script = load("res://i_damageable.gd")
	var i_respawnable_script = load("res://i_respawnable.gd")

	var i_damageable = GDSInterfaceChecker.construct_interface_from_script(i_damageable_script)
	var i_respawnable = GDSInterfaceChecker.construct_interface_from_script(i_respawnable_script)

	var adhoc_rules: Array[GDSMethodInfo] = [
		GDSMethodInfo.new("take_damage", 1, [""], [TYPE_INT], "", TYPE_NIL, [], PROPERTY_USAGE_DEFAULT),
		# GDSIMethodInfo.void_method("take_damage", 1, [""], [TYPE_INT]),
		GDSMethodInfo.new("get_is_dead", 0, [], [], "", TYPE_BOOL, ["@is_dead_getter"]),
		# GDSIMethodInfo.getter("is_dead", "", TYPE_BOOL)
	]

	print("Checking %s..." % script.resource_path)
	print("    i_damageable:  ", GDSInterfaceChecker.is_implemented_interface(script, i_damageable))
	print("    i_respawnable: ", GDSInterfaceChecker.is_implemented_interface(script, i_respawnable))
	print("    Adhoc rules:   ", GDSInterfaceChecker.is_implemented_interface(script, adhoc_rules))
	print("")
	print("Checking %s..." % broken_script.resource_path)
	print("    i_damageable:  ", GDSInterfaceChecker.is_implemented_interface(broken_script, i_damageable))
	print("    i_respawnable: ", GDSInterfaceChecker.is_implemented_interface(broken_script, i_respawnable))
	print("    Adhoc rules:   ", GDSInterfaceChecker.is_implemented_interface(broken_script, adhoc_rules))
	print("")

	print("All done! Exiting...")
	get_tree().quit()
