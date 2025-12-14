namespace GDScriptInterface.SourceGenerator;

/// <summary>
/// Enum representing the various types supported by GDScript's Variant type.
/// Each value corresponds to a specific data type in GDScript.
/// </summary>
internal enum VariantType
{
    Nil = 0,
    Bool = 1,
    Int = 2,
    Float = 3,
    String = 4,
    Vector2 = 5,
    Vector2I = 6,
    Rect2 = 7,
    Rect2I = 8,
    Vector3 = 9,
    Vector3I = 10,
    Transform2D = 11,
    Vector4 = 12,
    Vector4I = 13,
    Plane = 14,
    Quaternion = 15,
    Aabb = 16,
    Basis = 17,
    Transform3D = 18,
    Projection = 19,
    Color = 20,
    StringName = 21,
    NodePath = 22,
    Rid = 23,
    Object = 24,
    Callable = 25,
    Signal = 26,
    Dictionary = 27,
    Array = 28,
    PackedByteArray = 29,
    PackedInt32Array = 30,
    PackedInt64Array = 31,
    PackedFloat32Array = 32,
    PackedFloat64Array = 33,
    PackedStringArray = 34,
    PackedVector2Array = 35,
    PackedVector3Array = 36,
    PackedColorArray = 37,
    PackedVector4Array = 38,
}

internal static class VariantTypeExtensions
{
    /// <summary>
    /// Converts the given VariantType to its string representation.
    /// The format is "Variant.Type.{variantType}".
    /// </summary>
    /// <param name="variantType">The VariantType to convert.</param>
    /// <returns>A string representation of the VariantType.</returns>
    public static string ToString(this VariantType variantType)
        => $"Variant.Type.{variantType}";

    /// <summary>
    /// Maps the given <see cref="VariantType"/> to its corresponding GDScript type name.
    /// This method provides a string representation of the GDScript type that matches the VariantType.
    /// For example, <see cref="VariantType.Bool"/> maps to "bool", and <see cref="VariantType.Vector2I"/> maps to "Vector2i".
    /// If the VariantType does not have a specific mapping, it falls back to the default string representation.
    /// </summary>
    /// <param name="variantType">The <see cref="VariantType"/> to map.</param>
    /// <returns>A string representing the GDScript type name for the given VariantType.</returns>
    public static string MapToGDScriptName(this VariantType variantType) => variantType switch
    {
        VariantType.Nil => "Variant",
        VariantType.Bool => "bool",
        VariantType.Int => "int",
        VariantType.Float => "float",
        VariantType.String => "string",
        VariantType.Vector2I => "Vector2i",
        VariantType.Vector3I => "Vector3i",
        VariantType.Vector4I => "Vector4i",
        VariantType.Rect2I => "Rect2i",
        VariantType.Aabb => "AABB",
        VariantType.Rid => "RID",
        _ => variantType.ToString()
    };
}