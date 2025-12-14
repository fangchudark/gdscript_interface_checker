namespace GDScriptInterface.SourceGenerator;

/// <summary>
/// Defines hints that provide additional information about a property's type or behavior
/// in the context of Godot's Variant system. These hints are used during code generation
/// to specify how certain types should be handled, particularly for arrays and dictionaries.
/// </summary>
internal enum PropertyHint
{
    /// <summary>
    /// Indicates no specific hint. This is the default value when no additional information is needed.
    /// </summary>
    None = 0, // default

    /// <summary>
    /// Indicates that the property represents a typed array.
    /// This hint is used to specify the element type of the array during code generation.
    /// </summary>
    ArrayType = 31, // for typed array

    /// <summary>
    /// Indicates that the property represents a typed dictionary.
    /// This hint is used to specify the key and value types of the dictionary during code generation.
    /// </summary>
    DictionaryType = 38 // for typed dictionary
}

internal static class PropertyHintExtensions
{
    /// <summary>
    /// Converts the given PropertyHint to its string representation.
    /// The format is "PropertyHint.{usage}".
    /// </summary>
    /// <param name="usage">The PropertyHint to convert.</param>
    /// <returns>A string representation of the PropertyHint.</returns>
    public static string ToString(this PropertyHint hint)
        => $"PropertyHint.{hint}";
}