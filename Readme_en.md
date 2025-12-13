# CheckInterface

CheckInterface is a sample project based on Godot 4.4 (which supports GDScript and C#) that demonstrates how to check whether a GDScript script implements a specified set of interface methods in Godot. This project is suitable for checking dynamically at runtime whether a script conforms to a certain interface specification.

## Main functions

- **Interface checking** : Use `IsImplementedInterface` for C# or `is_implemented_interface` for GDScript to check whether a given GDScript implementation implements a specified set of method signatures.
- **Supports property getter/setter checks**: can detect property accessor methods (such as `get_xxx`/ `set_xxx`).
- Checks for matching method name, number of parameters, types of the parameters, and the return value.

## Directory Structure

This repo provides the interface inspection logic, and a sample Godot project for running it.

- `main.tscn`：The main scene, with attached `main.gd`.
- `main.gd`: GDScript entry point. Contains the interface inspection logic in GDScript.
- `Main.cs`：C# entry point. Contains the interface inspection logic in C#.
- `enemy.gd` Example GDScript that implements the IDamageable interface.
- `Enemy.cs`：Example C# class that implements the IDamageable interface.
- `IDamageable.cs`：Example of IDamageable interface written in C#.

- Other Godot project configuration files.

## Usage instructions

### Check an interface from GDScript

`main.gd`:

```gdscript
var script = load("res://enemy.gd")

# Construct interface rules at runtime
var interface : Array[MethodInfo] = [
    MethodInfo.new("take_damage", 1, [""], [TYPE_INT], "", TYPE_NIL, [], PROPERTY_USAGE_DEFAULT),
    MethodInfo.new("get_is_dead", 0, [], [], "", TYPE_BOOL, ["@is_dead_getter"])
]
print(is_implemented_interface(script, interface)) # Outputs true/false
```

### Check an interface from C#

`Main.cs`:

```csharp
var script = GD.Load<GDScript>("res://enemy.gd");

// Construct interface rules at runtime
List<GDScriptMethodInfo> @interface = [
    new("take_damage", 1, [""], [Variant.Type.Int], "", Variant.Type.Nil, ReturnFlags: PropertyUsageFlags.Default),
    new("get_is_dead", 0, [], [], "", Variant.Type.Bool, ["@is_dead_getter"]),
];
GD.Print(IsImplementedInterface(script, @interface)); // Outputs true/false
```

### Interface definition example

`IDamageable.cs`:

```csharp
public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(int damage);
}
```
`enemy.gd` implements GDScript versions of these methods (`is_dead`, `take_damage`).

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
3. The console will output the interface inspection results.

## License

MIT
