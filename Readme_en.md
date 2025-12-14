# CheckInterface

CheckInterface is a sample project based on Godot 4.4 (which supports GDScript and C#) that demonstrates how to check whether a GDScript script implements a specified set of interface methods in Godot. This project is suitable for checking dynamically at runtime whether a script conforms to a certain interface specification.

## Main functions

- **Interface checking** : Use `is_implemented_interface` (for GDScript) or `IsImplementedInterface` (for C#) to check whether a given GDScript implementation implements a specified set of method signatures.
- **Supports property getter/setter checks**: can detect property accessor methods (such as `get_xxx`/ `set_xxx`).
- Checks for matching method name, number of parameters, types of the parameters, and the return value.

## Directory Structure

This repo has CheckInterface implementations in GDScript and C#, and a sample Godot project for running them.

For the GDScript implementation, 2 files are required:
- [`gds_interface_checker.gd`](src/gds_interface_checker.gd) / [`GDSInterfaceChecker`](src/gds_interface_checker.gd): GDScript implementation of `is_implemented_interface`.
- [`gds_method_info.gd`](src/gds_method_info.gd) / [`GDSMethodInfo`](src/gds_method_info.gd): Helper for constructing and storing interface validation rules.

For the C# implementation:
- [`GDScriptInterface`](.GDScriptInterface) An implementation of a source code generator used to assist in building GDScript interface mappings
- [`Main.cs`](Main.cs)：Contains `InterfaceChecker.IsImplementedInterface()` and example usage.
- [`IDamageable.cs`](example/IDamageable.cs)：Example interface written in C#.
- [`Enemy.cs`](example/Enemy.cs) ：Example C# class that implements the IDamageable interface.

Godot project files:
- `main.tscn`：The main scene, with attached `main.gd`.
- [`main.gd`](main.gd): Demonstrates the GDScript implementation. Validates `enemy.gd` against some interface rules and prints the result.
- [`enemy.gd`](enemy.gd): Example GDScript that implements the IDamageable interface (see [`IDamageable.cs`](example/IDamageable.cs)).
- `project.godot`, associated metadata files.


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

`Main.cs`

```csharp
var script = GD.Load<GDScript>("res://example/enemy.gd");
List<GDScriptMethodInfo> @interface = [
    // take_damage(int damage) -> void
    new GDScriptMethodInfo(
        Name: "take_damage",
        Args: [
            new GDScriptPropertyInfo(
                Name: "damage", // Interface checks will not compare parameter names
                ClassName: "",
                Type: Variant.Type.Int,
                Hint: PropertyHint.None,
                HintString: "",
                Usage: PropertyUsageFlags.None
            )
        ],
        DefaultArgs: [],
        Flags: MethodFlags.Normal,
        Return: new GDScriptPropertyInfo(
            Name: "",
            ClassName: "",
            Type: Variant.Type.Nil,
            Hint: PropertyHint.None,
            HintString: "",
            Usage: PropertyUsageFlags.Default
        )
    ),

    // property getter
    new GDScriptMethodInfo(
        Name: "@is_dead_getter",
        Args: [],
        DefaultArgs: [],
        Flags: MethodFlags.Normal,
        Return: new GDScriptPropertyInfo(
            Name: "",
            ClassName: "",
            Type: Variant.Type.Bool,
            Hint: PropertyHint.None,
            HintString: "",
            Usage: PropertyUsageFlags.None
        )
    )
];
GD.Print(InterfaceChecker.IsImplementedInterface(script, @interface));
```

#### Source Generator

C# Support automatic mapping using source code generator
- Add attribute to the static class definition `[GenerateGDScriptInterfaceMapping(typeof(IMyInterface))]` （see [example/IDamageable.cs](example/IDamageable.cs)）。
- After compilation, the source generator will generate code in a static class with the same name (for example `IDamageableGDScriptMapping`) generate mapping constants.

[GenerateGDScriptInterfaceMapping](.GDScriptInterface/GDScriptInterface.Abstractions/GenerateGDScriptInterfaceMappingAttribute.cs) The use of the attribute requires meeting the following conditions:

- Can only be used with static partial classes(`static partial class`)
- The type provided at construction must be an interface
- The provided interface cannot be a generic interface.
- The provided interface cannot have generic methods
- The provided interface cannot contain any indexer
- The return values and parameters of all members in the provided interface must be variant-compatible types.

The source code generator is located at [.GDScriptInterface/GDScriptInterface.SourceGenerator](.GDScriptInterface/GDScriptInterface.SourceGenerator)。
If this generator is enabled in C# project, the corresponding GDScript interface mapping helper code will be automatically generated at compile time (no need to manually write the mapping array).

```csharp
public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(int damage);
}


[GenerateGDScriptInterfaceMapping(typeof(IDamageable))]
public static partial class IDamageableGDScriptMapping
{
    
}

// For C#, you can use source generators to generate GDScriptMethodInfo
List<GDScriptMethodInfo> @interface = [IDamageableGDScriptMapping.IsDeadGetter, IDamageableGDScriptMapping.TakeDamage];
GD.Print(InterfaceChecker.IsImplementedInterface(script,@interface));
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
