using System.Linq;
using Godot;

namespace GDScriptInterfaceChecker;

/// <summary>
/// Represents information about a GDScript method.
/// </summary>
/// <remarks>
/// This record class describes the metadata of a method in GDScript, including its name, arguments, default values, flags, internal ID, and return value.
/// The structure aligns with the dictionary Array returned by <see cref="GodotObject.GetMethodList"/>.
/// </remarks>
/// <param name="Name">
/// The name of the method. Corresponds to the "name" entry in the dictionary returned by <see cref="GodotObject.GetMethodList"/>.
/// </param>
/// <param name="Args">
/// An array of <see cref="GDScriptPropertyInfo"/> representing the method's arguments. 
/// Corresponds to the "args" entry in the dictionary returned by <see cref="GodotObject.GetMethodList"/>.
/// The format of each argument is similar to the result of <see cref="GodotObject.GetPropertyList"/>, but not all entries are used.
/// </param>
/// <param name="DefaultArgs">
/// An array of default argument values, represented as <see cref="Variant"/>. 
/// Corresponds to the "default_args" entry in the dictionary returned by <see cref="GodotObject.GetMethodList"/>.
/// </param>
/// <param name="Flags">
/// A combination of flags describing the method's behavior (see <see cref="MethodFlags"/>). 
/// Corresponds to the "flags" entry in the dictionary returned by <see cref="GodotObject.GetMethodList"/>.
/// </param>
/// <param name="Id">
/// The internal identifier of the method, represented as an integer. 
/// Corresponds to the "id" entry in the dictionary returned by <see cref="GodotObject.GetMethodList"/>.
/// </param>
/// <param name="Return">
/// Information about the method's return value, represented as <see cref="GDScriptPropertyInfo"/>. 
/// Corresponds to the "return" entry in the dictionary returned by <see cref="GodotObject.GetMethodList"/>.
/// The format is similar to the result of <see cref="GodotObject.GetPropertyList"/>.`, but not all entries are used.
/// </param>
public record class GDScriptMethodInfo(
    string Name,
    GDScriptPropertyInfo[] Args,
    Variant[] DefaultArgs,
    MethodFlags Flags,
    GDScriptPropertyInfo Return,
    int Id = 0
)
{
    /// <summary>
    /// Creates a new <see cref="GDScriptMethodInfo"/> instance from a Godot dictionary.
    /// </summary>
    /// <remarks>
    /// This method parses a dictionary (typically returned by <see cref="GodotObject.GetMethodList"/>) 
    /// and constructs a <see cref="GDScriptMethodInfo"/> object if all required keys are present.
    /// If any of the required keys are missing or the dictionary is null/empty, the method returns null.
    /// </remarks>
    /// <param name="dict">
    /// A Godot dictionary containing method metadata. The dictionary must include the following keys:
    /// <list type="bullet">
    /// <item><description>"name" - The method name as a string.</description></item>
    /// <item><description>"args" - An array of dictionaries representing the method's arguments. Each argument dictionary follows the format of <see cref="GodotObject.GetPropertyList"/>.</description></item>
    /// <item><description>"default_args" - An array of default argument values as variants.</description></item>
    /// <item><description>"flags" - The method flags as an integer (see <see cref="MethodFlags"/>).</description></item>
    /// <item><description>"id" - The internal identifier of the method as an integer.</description></item>
    /// <item><description>"return" - A dictionary representing the return value information, following the format of <see cref="GodotObject.GetPropertyList"/>.</description></item>
    /// </list>
    /// </param>
    /// <returns>
    /// A new <see cref="GDScriptMethodInfo"/> instance if the dictionary contains all required keys; otherwise, null.
    /// </returns>
    public static GDScriptMethodInfo FromDictionary(Godot.Collections.Dictionary dict)
    {
        // Return null if the dictionary is null or empty
        if (dict == null || dict.Count == 0)
            return null;

        // Validate and extract the "name" key
        if (!dict.TryGetValue("name", out var name))
            return null;

        // Validate and extract the "args" key
        if (!dict.TryGetValue("args", out var args))
            return null;

        // Validate and extract the "default_args" key
        if (!dict.TryGetValue("default_args", out var defaultArgs))
            return null;

        // Validate and extract the "flags" key
        if (!dict.TryGetValue("flags", out var flags))
            return null;

        // Validate and extract the "id" key
        if (!dict.TryGetValue("id", out var id))
            return null;

        // Validate and extract the "return" key
        if (!dict.TryGetValue("return", out var returnInfo))
            return null;

        // Parse the method name
        var methodName = name.AsString();

        // Parse the method arguments
        var methodArgs = args.AsGodotArray<Godot.Collections.Dictionary>();
        var argsInfos = methodArgs
            .Select(GDScriptPropertyInfo.FromDictionary)
            .ToArray();

        // Parse the default arguments
        var methodDefaultArgs = defaultArgs.AsGodotArray().ToArray();

        // Parse the method flags
        var methodFlags = flags.As<MethodFlags>();

        // Parse the method ID
        var methodId = id.AsInt32();

        // Parse the return value information
        var methodReturn = GDScriptPropertyInfo.FromDictionary(returnInfo.AsGodotDictionary());

        // Construct and return a new GDScriptMethodInfo instance
        return new GDScriptMethodInfo(
            methodName,
            argsInfos,
            methodDefaultArgs,
            methodFlags,
            methodReturn,
            methodId
        );
    }

    public override string ToString()
    {
        return $"Name: {Name}, Args: ( {string.Join<GDScriptPropertyInfo>(',', Args)} ), DefaultArgs: {string.Join(',', DefaultArgs)}, Flags: {Flags}, Id: {Id}, Return: {Return}";
    }
}