namespace GDScriptInterface.SourceGenerator;

/// <summary>
/// Defines flags that describe the usage of a property in the context of Godot's Variant system.
/// These flags are used to provide additional information about how a property should be handled
/// during code generation or type resolution.
/// </summary>
internal enum PropertyUsageFlags
{
    /// <summary>
    /// Indicates no specific usage flags. Typically used for parameters or properties without special behavior.
    /// </summary>
    None = 0, // for parameter

    /// <summary>
    /// Indicates that the property represents an enumeration type.
    /// For enum types, the generator maps them to the integer type (<c>int</c>) in GDScript without class name
    /// </summary>
    ClassIsEnum = 65536, // enum type. for enum type, we generator to int type

    /// <summary>
    /// Indicates that the property is a Variant type.
    /// This flag is used to represent types that can hold any value, such as Godot's <c>Variant</c>.
    /// </summary>
    NilIsVariant = 131072, // for variant type

    /// <summary>
    /// Indicates the default usage flags, typically applied to void methods or return values.
    /// </summary>
    Default = 6 // for void method
}

internal static class PropertyUsageFlagsExtensions
{
    /// <summary>
    /// Converts the given PropertyUsageFlags to its string representation.
    /// The format is "PropertyUsageFlags.{usage}".
    /// </summary>
    /// <param name="usage">The PropertyUsageFlags to convert.</param>
    /// <returns>A string representation of the PropertyUsageFlags.</returns>
    public static string ToString(this PropertyUsageFlags usage)
        => $"PropertyUsageFlags.{usage}";
}