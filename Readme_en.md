# CheckInterface

CheckInterface is a sample project based on Godot 4.4 (which supports GDScript and C#) that demonstrates how to check whether a GDScript script implements a specified set of interface methods in Godot. This project is suitable for checking dynamically at runtime whether a script conforms to a certain interface specification.

## Main features

- **Interface checking** : Use `GDSInterfaceChecker.is_implemented_interface(...)` (for GDScript) or `IsImplementedInterface(...)` (for C#) to check whether a given GDScript implementation matches a specified set of method signatures.
- **Supports property getter/setter checks**: can detect property accessor methods (such as `get_xxx`/ `set_xxx`).
- Checks for matching method name, number of parameters, types of the parameters, and the return value.
- Define an Interface as a normal GDScript class with stubbed methods.

## Directory structure

This repo has interface checking implementations in GDScript and C#, and a sample Godot project for running them.

For the GDScript implementation, 3 files are required:
- `gds_interface_checker.gd` / `GDSInterfaceChecker`: GDScript implementation of `is_implemented_interface`.
- `gds_method_info.gd` / `GDSMethodInfo`: Helper for constructing and storing interface validation rules.
- `gds_interface.gd` / `GDSInterface`: Base class for user-defined Interfaces.

For the C# implementation:
- `Main.cs`: Contains `IsImplementedInterface(...)` and example usage.
- `IDamageable.cs`: Example interface written in C#.
- `Enemy.cs`: Example C# class that implements the IDamageable interface.

Godot project files:
- `main.tscn`: The main scene, with attached `main.gd`.
- `main.gd`: Demonstrates the GDScript implementation. Validates `enemy.gd` and `enemy_broken.gd` against some interface rules and prints the result.
- `i_damageable.gd`: Example interface written in GDScript.
- `i_respawnable.gd`: Example interface written in GDScript.
- `enemy.gd`: Example GDScript class that implements correct versions of the IDamageable and IRespawnable interfaces.
- `enemy_broken.gd`: Implements broken versions of the IDamageable and IRespawnable interfaces.
- `project.godot`, associated metadata files.

## Usage instructions

Check an interface in GDScript:
```gdscript
var script = load("res://enemy.gd")
var i_damageable = load("res://i_damageable.gd") 

# Validate with an existing Interface class
var interface = GDSInterfaceChecker.construct_interface_from_script(i_damageable)
print(GDSInterfaceChecker.is_implemented_interface(script, interface)) # Outputs true/false

# Construct interface rules at runtime
var rules : Array[GDSMethodInfo] = [
    GDSMethodInfo.new("take_damage", 1, [""], [TYPE_INT], "", TYPE_NIL, [], PROPERTY_USAGE_DEFAULT),
    GDSMethodInfo.new("get_is_dead", 0, [], [], "", TYPE_BOOL, ["@is_dead_getter"])
]
print(GDSInterfaceChecker.is_implemented_interface(script, rules)) # Outputs true/false
```

Check an interface in C#:
```csharp
var script = GD.Load<GDScript>("res://enemy.gd");

// Construct interface rules at runtime
List<GDScriptMethodInfo> @interface = [
    new("take_damage", 1, [""], [Variant.Type.Int], "", Variant.Type.Nil, ReturnFlags: PropertyUsageFlags.Default),
    new("get_is_dead", 0, [], [], "", Variant.Type.Bool, ["@is_dead_getter"]),
];
GD.Print(IsImplementedInterface(script, @interface)); // Outputs true/false
```

### Interface definition examples

Define an interface in GDScript:
```gdscript
# i_damageable.gd
# A user-defined Interface should extend GDSInterface.
class_name IDamageable extends GDSInterface

func take_damage(damage: int) -> void:
    return

func get_is_dead() -> bool:
    return false

```

Define an interface in C#:
```csharp
// IDamageable.cs
public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(int damage);
}
```

## Applicable scenarios

- Dynamic plug-in system
- Script hot reloading and interface compliance verification
- Interface consistency checks during cross-language script interoperability

## Dependencies

- Godot 4.4+
- .NET 8.0（only if using C# with Godot)

## Running this demo

1. Open the project using the Godot editor.
2. Run `main.tscn`.
3. The console will output results of the interface validation.

## License

MIT
