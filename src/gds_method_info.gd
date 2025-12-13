class_name GDSMethodInfo extends RefCounted

var name: String
var args_count: int
var args_class_name: Array[String]
var args_type: Array[Variant.Type]
var return_class_name: String
var return_type: Variant.Type
var aliases : Array[String] = []
var return_flags : PropertyUsageFlags = -1
var return_hint : PropertyHint = -1
var return_hint_string : String = ""
var args_flags : Array[PropertyUsageFlags] = []
var args_hints : Array[PropertyHint] = []
var args_hints_strings : Array[String] = []

func _init(
	name: String,
	args_count: int,
	args_class_name: Array[String],
	args_type: Array[Variant.Type],
	return_class_name: String,
	return_type: Variant.Type,
	aliases: Array[String] = [],
	return_flags: PropertyUsageFlags = -1,
	return_hint: PropertyHint = -1,
	return_hint_string: String = "",
	args_flags: Array[PropertyUsageFlags] = [],
	args_hints: Array[PropertyHint] = [],
	args_hints_strings: Array[String] = []
) -> void:
		self.name = name
		self.args_count = args_count
		self.args_class_name = args_class_name
		self.args_type = args_type
		self.return_class_name = return_class_name
		self.return_type = return_type
		self.aliases = aliases
		self.return_flags = return_flags
		self.return_hint = return_hint
		self.return_hint_string = return_hint_string
		self.args_flags = args_flags
		self.args_hints = args_hints
		self.args_hints_strings = args_hints_strings

static func void_method(
	name: String,
	args_count: int,
	args_class_name: Array[String],
	args_type: Array[Variant.Type],
	aliases: Array[String] = [],
	args_flags: Array[PropertyUsageFlags] = [],
	args_hints: Array[PropertyHint] = [],
	args_hints_strings: Array[String] = []
) -> GDSMethodInfo:
	return GDSMethodInfo.new(
		name,
		args_count,
		args_class_name,
		args_type,
		"",
		TYPE_NIL,
		aliases,
		PROPERTY_USAGE_DEFAULT,
		-1,
		"",
		args_flags,
		args_hints,
		args_hints_strings
	)

static func variant_method(
	name: String,
	args_count: int,
	args_class_name: Array[String],
	args_type: Array[Variant.Type],
	aliases: Array[String] = [],
	args_flags: Array[PropertyUsageFlags] = [],
	args_hints: Array[PropertyHint] = [],
	args_hints_strings: Array[String] = []
) -> GDSMethodInfo:
	return GDSMethodInfo.new(
		name,
		args_count,
		args_class_name,
		args_type,
		"",
		TYPE_NIL,
		aliases,
		PROPERTY_USAGE_NIL_IS_VARIANT,
		-1,
		"",
		args_flags,
		args_hints,
		args_hints_strings
	)

static func parameterless_void_method(name: String, aliases: Array[String] = []) -> GDSMethodInfo:
	return void_method(name, 0, [], [], aliases)

static func enum_method(
	name: String,
	args_count: int,
	args_class_name: Array[String],
	args_type: Array[Variant.Type],
	enum_name: String,
	aliases: Array[String] = [],
	args_flags: Array[PropertyUsageFlags] = [],
	args_hints: Array[PropertyHint] = [],
	args_hints_strings: Array[String] = []
) -> GDSMethodInfo:
	return GDSMethodInfo.new(
		name,
		args_count,
		args_class_name,
		args_type,
		enum_name,
		TYPE_INT,
		aliases,
		PROPERTY_USAGE_CLASS_IS_ENUM,
		-1,
		"",
		args_flags,
		args_hints,
		args_hints_strings
	)

static func typed_array_method(
	name: String,
	args_count: int,
	args_class_name: Array[String],
	args_type: Array[Variant.Type],
	array_type: String,
	aliases: Array[String] = [],
	args_flags: Array[PropertyUsageFlags] = [],
	args_hints: Array[PropertyHint] = [],
	args_hints_strings: Array[String] = []
) -> GDSMethodInfo:
	return GDSMethodInfo.new(
		name,
		args_count,
		args_class_name,
		args_type,
		"",
		TYPE_ARRAY,
		aliases,
		-1,
		PROPERTY_HINT_ARRAY_TYPE,
		array_type,
		args_flags,
		args_hints,
		args_hints_strings
	)

static func typed_dictionary_method(
	name: String,
	args_count: int,
	args_class_name: Array[String],
	args_type: Array[Variant.Type],
	key_type: String,
	value_type: String,
	aliases: Array[String] = [],
	args_flags: Array[PropertyUsageFlags] = [],
	args_hints: Array[PropertyHint] = [],
	args_hints_strings: Array[String] = []
) -> GDSMethodInfo:
	return GDSMethodInfo.new(
		name,
		args_count,
		args_class_name,
		args_type,
		"",
		TYPE_DICTIONARY,
		aliases,
		-1,
		PROPERTY_HINT_DICTIONARY_TYPE,
		key_type + ";" + value_type,
		args_flags,
		args_hints,
		args_hints_strings
	)

enum PropertyKind {
	NORMAL,
	ENUM,
	VARIANT,
	TYPED_ARRAY,
	TYPED_DICTIONARY
}

static func getter(
	property_name: String,
	property_class_name: String,
	property_type: Variant.Type,
	property_kind: PropertyKind = PropertyKind.NORMAL,
	array_type: String = "",
	key_type: String = "",
	value_type: String = ""
) -> GDSMethodInfo:
	match property_kind:
		PropertyKind.TYPED_ARRAY:
			return typed_array_method(
				"get_" + property_name,
				0,
				[],
				[],
				array_type,
				["@" + property_name + "_getter"]
			)
		PropertyKind.TYPED_DICTIONARY:
			return typed_dictionary_method(
				"get_" + property_name,
				0,
				[],
				[],
				key_type,
				value_type,
				["@" + property_name + "_getter"]
			)
		PropertyKind.ENUM:
			return enum_method(
				"get_" + property_name,
				0,
				[],
				[],
				property_class_name,
				["@" + property_name + "_getter"]
			)
		PropertyKind.VARIANT:
			return variant_method(
				"get_" + property_name,
				0,
				[],
				[],
				["@" + property_name + "_getter"]
			)
		_:
			return GDSMethodInfo.new(
				"get_" + property_name,
				0,
				[],
				[],
				property_class_name,
				property_type,
				["@" + property_name + "_getter"]
			)




static func setter(
	property_name: String,
	property_class_name: String,
	property_type: Variant.Type,
	property_kind: PropertyKind,
	array_type: String = "",
	key_type: String = "",
	value_type: String = ""
) -> GDSMethodInfo:
	return GDSMethodInfo.void_method(
		"set_" + property_name,
		1,
		[property_class_name],
		[property_type],
		["@" + property_name + "_setter"],
		[PROPERTY_USAGE_NIL_IS_VARIANT] if property_kind == PropertyKind.VARIANT else [PROPERTY_USAGE_CLASS_IS_ENUM] if property_kind == PropertyKind.ENUM else [],
		[PROPERTY_HINT_ARRAY_TYPE] if property_kind == PropertyKind.TYPED_ARRAY else [PROPERTY_HINT_DICTIONARY_TYPE] if property_kind == PropertyKind.TYPED_DICTIONARY else [],
		[array_type] if property_kind == PropertyKind.TYPED_ARRAY else [key_type + ";" + value_type] if property_kind == PropertyKind.TYPED_DICTIONARY else []
	)
