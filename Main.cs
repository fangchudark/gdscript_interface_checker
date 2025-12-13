using GDScriptInterfaceChecker;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Control
{

	public override void _Ready()
	{
		var script = GD.Load<GDScript>("res://example/enemy.gd");

		List<GDScriptMethodInfo> @interface = [
			// take_damage(int damage) -> void
			new(
				Name: "take_damage",
				Args: [
					new(
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
				Return: new(
					Name: "",
					ClassName: "",
					Type: Variant.Type.Nil,
					Hint: PropertyHint.None,
					HintString: "",
					Usage: PropertyUsageFlags.Default
				)
			),

			// property getter
			new(
				Name: "@is_dead_getter",
				Args: [],
				DefaultArgs: [],
				Flags: MethodFlags.Normal,
				Return: new(
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
	}

}