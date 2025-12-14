using System.Collections.Generic;
using GDScriptInterfaceChecker;
using Godot;

public partial class Main : Control
{
	public override void _Ready()
	{
		var script = GD.Load<GDScript>("res://example/enemy.gd");
		// For C#, you can use source generators to generate GDScriptMethodInfo
		List<GDScriptMethodInfo> @interface = [IDamageableGDScriptMapping.IsDeadGetter, IDamageableGDScriptMapping.TakeDamage];
		GD.Print("GDScript implemented C# interface? : ", InterfaceChecker.IsImplementedInterface(script, @interface));

		var interfaceGDScript = GD.Load<GDScript>("res://example/i_damageable.gd");
		var gdScriptInterface = InterfaceChecker.ConstructInterfaceFromScript(interfaceGDScript);
		GD.Print("GDScript interface: ", string.Join(',', gdScriptInterface));
		GD.Print("GDScript implemented GDScript interface? : ", InterfaceChecker.IsImplementedInterface(script, gdScriptInterface));		

	}

}