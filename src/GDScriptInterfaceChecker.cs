using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GDScriptInterfaceChecker;

public static class InterfaceChecker
{
    /// <summary>
    /// Determines whether a given GDScript implements a specified interface.
    /// </summary>
    /// <remarks>
    /// This method checks if all methods defined in the interface are implemented by the provided GDScript.
    /// The comparison includes method names, return types, the number of arguments, and argument types.
    /// </remarks>
    /// <param name="script">
    /// The <see cref="GDScript"/> instance to check for interface implementation.
    /// </param>
    /// <param name="interface">
    /// A list of <see cref="GDScriptMethodInfo"/> representing the methods defined in the interface.
    /// Each method specifies the required name, return type, and argument details.
    /// </param>
    /// <returns>
    /// <c>true</c> if the script implements all methods in the interface; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsImplementedInterface(GDScript script, List<GDScriptMethodInfo> @interface)
    {
        // Retrieve the list of methods defined in the script
        var methodInfo = script.GetScriptMethodList();

        // Convert the raw dictionary array into a list of GDScriptMethodInfo objects
        var methods = methodInfo.Select(GDScriptMethodInfo.FromDictionary).ToList();

        // Check if all methods in the interface are implemented by the script
        return @interface.All(req =>
            methods.Any(m =>
            {                                
                // Compare method names
                if (req.Name != m.Name) return false;

                // Compare the number of arguments
                if (req.Args.Length != m.Args.Length) return false;

                // Compare return types
                if (!req.Return.IsSameType(m.Return)) 
                    return false;

                // Compare argument types one by one
                for (int i = 0; i < req.Args.Length; i++)
                {
                    var reqArg = req.Args[i];
                    var interfaceArg = m.Args[i];

                    if (!reqArg.IsSameType(interfaceArg))
                        return false;
                }

                // If all checks pass, the method matches the interface requirement
                return true;
            })
        );
    }
}