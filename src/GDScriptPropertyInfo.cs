using Godot;

namespace GDScriptInterfaceChecker;

/// <summary>
/// Represents information about a GDScript property.
/// </summary>
/// <remarks>
/// This record class is used to describe the metadata of a property in GDScript. 
/// It provides details about the property's characteristics, including its name, type, editor hints, and usage flags.
/// </remarks>
/// <param name="Name">
/// The name of the property. Corresponds to the "name" entry in the dictionary returned by <see cref="GodotObject.GetPropertyList"/>.
/// </param>
/// <param name="ClassName">
/// The class name of the property if the property is of <see cref="Variant.Type.Object"/> and inherits from a specific class; otherwise, it is an empty string. 
/// Corresponds to the "class_name" entry in the dictionary returned by <see cref="GodotObject.GetPropertyList"/>.
/// </param>
/// <param name="Type">
/// The type of the property, represented as an integer (see <see cref="Variant.Type"/>). 
/// Corresponds to the "type" entry in the dictionary returned by <see cref="GodotObject.GetPropertyList"/>.
/// </param>
/// <param name="Hint">
/// A value indicating how this property should be edited in the editor (see <see cref="PropertyHint"/>). 
/// Corresponds to the "hint" entry in the dictionary returned by <see cref="GodotObject.GetPropertyList"/>.
/// </param>
/// <param name="HintString">
/// Additional information that depends on the value of <paramref name="Hint"/> (see <see cref="PropertyHint"/>). 
/// Corresponds to the "hint_string" entry in the dictionary returned by <see cref="GodotObject.GetPropertyList"/>.
/// </param>
/// <param name="Usage">
/// A combination of flags describing the usage of the property (see <see cref="PropertyUsageFlags"/>). 
/// Corresponds to the "usage" entry in the dictionary returned by <see cref="GodotObject.GetPropertyList"/>.
/// </param>
/// <example>
/// In GDScript, all members of a class are treated as properties. However, in C# or GDExtension, 
/// explicit decorators or attributes must be used to mark class members as Godot properties.
/// </example>
public record class GDScriptPropertyInfo(
    string Name,
    string ClassName,
    Variant.Type Type,
    PropertyHint Hint,
    string HintString,
    PropertyUsageFlags Usage
)
{
    /// <summary>
    /// Checks whether the property is a variant or null.
    /// </summary>
    public bool IsVariantOrNull => Type == Variant.Type.Nil;

    /// <summary>
    /// Checks whether the property has the same type as another property.
    /// </summary>
    /// <param name="other">The other property to compare with.</param>
    /// <returns><c>true</c> if the properties have the same type; otherwise, <c>false</c>.</returns>
    public bool IsSameType(GDScriptPropertyInfo other)
        => Type == other.Type && ClassName == other.ClassName;

    /// <summary>
    /// Checks whether the property is an enum.
    /// </summary>
    public bool IsEnum => Type == Variant.Type.Int &&
        (Usage & PropertyUsageFlags.ClassIsEnum) != 0;

    /// <summary>
    /// Creates a new <see cref="GDScriptPropertyInfo"/> instance from a Godot dictionary.
    /// </summary>
    /// <remarks>
    /// This method parses a dictionary (typically returned by <see cref="GodotObject.GetPropertyList"/>) 
    /// and constructs a <see cref="GDScriptPropertyInfo"/> object if all required keys are present.
    /// If any of the required keys are missing or the dictionary is null/empty, the method returns null.
    /// </remarks>
    /// <param name="dict">
    /// A Godot dictionary containing property metadata. The dictionary must include the following keys:
    /// <list type="bullet">
    /// <item><description>"name" - The property name as a string.</description></item>
    /// <item><description>"class_name" - The class name as a string (empty if not applicable).</description></item>
    /// <item><description>"type" - The property type as an integer (see <see cref="Variant.Type"/>).</description></item>
    /// <item><description>"hint" - The editor hint as an integer (see <see cref="PropertyHint"/>).</description></item>
    /// <item><description>"hint_string" - Additional hint information as a string.</description></item>
    /// <item><description>"usage" - The property usage flags as an integer (see <see cref="PropertyUsageFlags"/>).</description></item>
    /// </list>
    /// </param>
    /// <returns>
    /// A new <see cref="GDScriptPropertyInfo"/> instance if the dictionary contains all required keys; otherwise, null.
    /// </returns>
    public static GDScriptPropertyInfo FromDictionary(Godot.Collections.Dictionary dict)
    {
        // Return null if the dictionary is null or empty
        if (dict == null || dict.Count == 0)
            return null;

        // Validate and extract the "name" key
        if (!dict.TryGetValue("name", out var name))
            return null;

        // Validate and extract the "class_name" key
        if (!dict.TryGetValue("class_name", out var className))
            return null;

        // Validate and extract the "type" key
        if (!dict.TryGetValue("type", out var type))
            return null;

        // Validate and extract the "hint" key
        if (!dict.TryGetValue("hint", out var hint))
            return null;

        // Validate and extract the "hint_string" key
        if (!dict.TryGetValue("hint_string", out var hintString))
            return null;

        // Validate and extract the "usage" key
        if (!dict.TryGetValue("usage", out var usage))
            return null;

        // Construct and return a new GDScriptPropertyInfo instance
        return new GDScriptPropertyInfo(
            name.AsString(),
            className.AsString(),
            type.As<Variant.Type>(),
            hint.As<PropertyHint>(),
            hintString.AsString(),
            usage.As<PropertyUsageFlags>()
        );
    }

    public override string ToString()
    {
        return $"Name: {Name}, ClassName: {ClassName}, Type: {Type}, Hint: {Hint}, HintString: {HintString}, Usage: {Usage}";
    }
}