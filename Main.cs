using System.Collections.Generic;
using GDScriptInterfaceChecker;
using Godot;

public partial class Main : Control
{
	public override void _Ready()
	{
		var script = GD.Load<GDScript>("res://example/enemy.gd");
		/*
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
		*/

		// For C#, you can use source generators to generate GDScriptMethodInfo
		List<GDScriptMethodInfo> @interface = [IDamageableGDScriptMapping.IsDeadGetter, IDamageableGDScriptMapping.TakeDamage];
		GD.Print(InterfaceChecker.IsImplementedInterface(script,@interface));
	}

}