class_name GDSInterfaceChecker extends RefCounted

static var IGNORED_METHOD_NAMES: Array[String] = [
	"_static_init",
	"_init",
	"_ready",
	"_process",
	"_physics_process",
	"_notification"
]

## Constructs GDSMethodInfo definitions using the provided `GDScript`.
## Define your desired interface as a script extending `GDSInterface`.
## Built-in methods of the script are automatically disregarded. So an interface
## will never enforce presence of `_init()`, `_ready()`, etc. in an implementer.
## The method implementations in the interface file should be simple one-liners:
## ```
## # Methods returning void can simply return or pass.
## func apply_damage(amount: int) -> void:
##     return
##
## # Methods with typed returns should return a dummy value or instance of that type.
## func is_dead() -> bool:
##     return false
##
## # Use `null` if possible.
## func get_hit_location(attack: Attack) -> BodyPart:
##     return null
##
## # Methods returning an Enum can return any of its defined values.
## func get_status() -> StatusEnum:
##     return StatusEnum.DEFAULT
## ```
static func construct_interface_from_script(script: GDScript) -> Array[GDSMethodInfo]:
	var method_info = script.get_script_method_list()

	# As of Godot 4.4, Array.map() cannot produce a Typed Array. So we make this generic
	# helper to pluck a specific key from Array[Dictionary]. Then we can convert it to the desired
	# Typed Array using the copy constructor: Array(map_extract.call(...), SOME_TYPE, "", null).
	# Validation logic in is_implemented_interface() expects unspecified flags to be -1, so pass
	# `zeroMeansNull = true` to honour that and convert any zeroes to -1.
	var map_extract: Callable = func(arr: Array[Dictionary], key: String, zeroMeansNull: bool = false) -> Array[Variant]:
		return arr.map(
			func(dict:Dictionary) -> Variant:
				if zeroMeansNull and dict[key] == 0:
					return -1
				return dict[key]
		)

	var interface_rules: Array[GDSMethodInfo] = []
	for method:Dictionary in method_info:
		if method["name"] in IGNORED_METHOD_NAMES:
			continue

		var method_name : String = method["name"]
		var args : Array = method["args"]
		var ret : Dictionary = method["return"]
		var method_rule: GDSMethodInfo = GDSMethodInfo.new(
			method_name,									# name
			args.size(),									# args_count
			Array(map_extract.call(args, "class_name"),  TYPE_STRING, "", null),       # args_class_name Array[String]
			Array(map_extract.call(args, "type"),        TYPE_INT, "", null),          # args_type       Array[(Enum)Variant.Type]
			ret["class_name"],							# return_class_name
			ret["type"],									# return_type
			[], 											# aliases #TODO should we predict aliases when using a file def?
			ret["usage"] if ret["usage"] != 0 else -1,	# return_flags, aka usage
			ret["hint"]  if ret["hint"]  != 0 else -1,	# return_hint
			ret["hint_string"],							# return_hint_string
			Array(map_extract.call(args, "usage", true), TYPE_INT, "", null), # args_flags, aka usage Array[(Enum)PropertyUsageFlags]
			Array(map_extract.call(args, "hint", true),  TYPE_INT, "", null), # args_hints            Array[(Enum)PropertyHint]
			Array(map_extract.call(args, "hint_string"), TYPE_STRING, "", null)  # args_hints_strings Array[String]
		)
		interface_rules.push_back(method_rule)
	return interface_rules

static func is_implemented_interface(script: GDScript, interface: Array[GDSMethodInfo]) -> bool:
	var method_info = script.get_script_method_list()

	return interface.all(func(req:GDSMethodInfo): return method_info.any(func(m:Dictionary):
		var method_name : String = m["name"]
		var args : Array = m["args"]
		var ret : Dictionary = m["return"]

		var valid_names = req.aliases.duplicate()
		valid_names.append(req.name)
		if not valid_names.has(method_name):
			return false

		if args.size() != req.args_count:
			return false

		for i in args.size():
			if (args[i]["type"] != req.args_type[i] or
				args[i]["class_name"] != req.args_class_name[i]
			):
				return false

			if req.args_flags.size() > i and req.args_flags[i] != -1:
				if (args[i]["usage"] & req.args_flags[i]) != req.args_flags[i]:
					return false

			if req.args_hints.size() > i and req.args_hints[i] != -1:
				if args[i]["hint"] != req.args_hints[i]:
					return false

			if req.args_hints_strings.size() > i and req.args_hints_strings[i] != "":
				if args[i]["hint_string"] != req.args_hints_strings[i]:
					return false

		if (ret["type"] != req.return_type or
			ret["class_name"] != req.return_class_name
		):
			return false

		if req.return_flags != -1 and (ret["usage"] & req.return_flags) != req.return_flags:
			return false

		if req.return_hint != -1 and ret["hint"] != req.return_hint:
			return false

		if req.return_hint_string != "" and ret["hint_string"] != req.return_hint_string:
			return false

		return true
	))
