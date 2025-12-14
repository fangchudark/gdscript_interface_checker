namespace GDScriptInterface.SourceGenerator;

/// <summary>
/// Represents information about a GDScript property, including its class name, Variant type,
/// property hint, hint string, and usage flags. This record is used during code generation
/// to describe how a property should be represented in GDScript.
/// </summary>
internal record GDScriptPropertyInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GDScriptPropertyInfo"/> record.
    /// </summary>
    /// <param name="className">The name of the class associated with the property, if applicable.</param>
    /// <param name="variantType">The Variant type of the property.</param>
    /// <param name="hint">The property hint that provides additional information about the property's type or behavior.</param>
    /// <param name="hintString">A string associated with the property hint, providing further details (e.g., type names for arrays or dictionaries).</param>
    /// <param name="usage">The usage flags that describe how the property should be handled in GDScript.</param>
    public GDScriptPropertyInfo(string className, VariantType variantType, PropertyHint hint, string hintString, PropertyUsageFlags usage)
    {
        ClassName = className;
        VariantType = variantType;
        Hint = hint;
        HintString = hintString;
        Usage = usage;
    }

    /// <summary>
    /// Gets the name of the class associated with the property.
    /// This is typically used for GodotObject-derived types or other class-based properties.
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    /// Gets the Variant type of the property.
    /// This defines how the property is represented in Godot's Variant system.
    /// </summary>
    public VariantType VariantType { get; }

    /// <summary>
    /// Gets the property hint that provides additional information about the property's type or behavior.
    /// For example, this can indicate whether the property is a typed array or dictionary.
    /// </summary>
    public PropertyHint Hint { get; }

    /// <summary>
    /// Gets the hint string associated with the property hint.
    /// This string provides further details, such as the element type for arrays or key/value types for dictionaries.
    /// </summary>
    public string HintString { get; }

    /// <summary>
    /// Gets the usage flags that describe how the property should be handled in GDScript.
    /// These flags provide context for special cases, such as enum types or void methods.
    /// </summary>
    public PropertyUsageFlags Usage { get; }
}