using Microsoft.CodeAnalysis;

namespace GDScriptInterface.SourceGenerator;

/// <summary>
/// Provides methods to resolve the VariantType for a given type symbol in the context of Godot's Variant system.
/// </summary>
internal static class VariantResolver
{
    /// <summary>
    /// Attempts to resolve the VariantType and optionally the class name for a given type symbol.
    /// </summary>
    /// <param name="compilation">The compilation context used for type resolution.</param>
    /// <param name="type">The type symbol to resolve.</param>
    /// <param name="variantType">The resolved VariantType, if successful.</param>
    /// <param name="className">The resolved class name (if applicable).</param>
    /// <returns>True if the type could be resolved; otherwise, false.</returns>
    public static bool TryResolveVariantType(
        Compilation compilation,
        ITypeSymbol type,
        out VariantType variantType,
        out string className
    )
    {
        className = "";
        variantType = VariantType.Nil;

        // Handle void type (only allowed for return values)
        if (type.SpecialType == SpecialType.System_Void)
        {
            variantType = VariantType.Nil; // Void maps to Nil in VariantType
            return true;
        }

        // Handle basic managed types
        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
                variantType = VariantType.Bool; // Boolean maps to Bool in VariantType
                return true;

            case SpecialType.System_String:
                variantType = VariantType.String; // String maps to String in VariantType
                return true;

            case SpecialType.System_Single:
            case SpecialType.System_Double:
                variantType = VariantType.Float; // Single/Double maps to Float in VariantType
                return true;

            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
                variantType = VariantType.Int; // All integer types map to Int in VariantType
                return true;

            case SpecialType.System_Decimal:
                return false; // Decimal is explicitly not supported
        }

        // Handle System.Array (T[])
        if (type is IArrayTypeSymbol array)
        {
            return IsSupportedSystemArray(
                compilation,
                array,
                out variantType,
                out className
            ); // Delegate array handling to helper method
        }

        // Handle Godot.Collections.Array / Dictionary
        if (type is INamedTypeSymbol generic)
        {
            var fullName = generic.ConstructedFrom.ToDisplayString();

            if (fullName == "Godot.Collections.Array")
            {
                if (generic.TypeArguments.Length == 0)
                {
                    variantType = VariantType.Array; // Unparameterized Array maps to VariantType.Array
                    return true;
                }

                return TryResolveVariantType(
                    compilation,
                    generic.TypeArguments[0],
                    out variantType,
                    out className
                ); // Recursively resolve the type argument
            }

            if (fullName == "Godot.Collections.Dictionary")
            {
                if (generic.TypeArguments.Length != 2)
                    return false; // Dictionary must have exactly two type arguments

                return
                    TryResolveVariantType(compilation, generic.TypeArguments[0], out _, out _) &&
                    TryResolveVariantType(compilation, generic.TypeArguments[1], out _, out _); // Validate both key and value types
            }
        }

        // Handle known Godot value types
        if (IsKnownGodotValueType(compilation, type, out variantType))
            return true; // Use helper method to check for known Godot value types

        // Handle GodotObject and derived classes
        if (InheritsFromGodotObject(compilation, type))
        {
            variantType = VariantType.Object; // All GodotObject-derived types map to VariantType.Object
            className = type.Name; // Store the class name for reference
            return true;
        }

        return false; // Type cannot be resolved
    }

    /// <summary>
    /// Determines whether the provided array type is supported by the Variant system.
    /// </summary>
    /// <param name="compilation">The compilation context used for type resolution.</param>
    /// <param name="array">The array type symbol to check.</param>
    /// <param name="variantType">The resolved VariantType, if supported.</param>
    /// <param name="className">The resolved class name (if applicable).</param>
    /// <returns>True if the array type is supported; otherwise, false.</returns>
    private static bool IsSupportedSystemArray(
        Compilation compilation,
        IArrayTypeSymbol array,
        out VariantType variantType,
        out string className
    )
    {
        variantType = VariantType.Array;
        className = "";

        if (array.Rank != 1)
            return false; // Only single-dimensional arrays are supported

        var elem = array.ElementType;

        // Handle basic managed types
        switch (elem.SpecialType)
        {
            case SpecialType.System_Int32:
                variantType = VariantType.PackedInt32Array;
                return true;
            case SpecialType.System_Int64:
                variantType = VariantType.PackedInt64Array;
                return true;
            case SpecialType.System_Byte:
                variantType = VariantType.PackedByteArray;
                return true;
            case SpecialType.System_Single:
                variantType = VariantType.PackedFloat32Array;
                return true;
            case SpecialType.System_Double:
                variantType = VariantType.PackedFloat64Array;
                return true;
            case SpecialType.System_String:
                variantType = VariantType.PackedStringArray;
                return true; // Supported primitive types in arrays
        }

        // Handle whitelisted Godot struct types
        bool Match(string metadataName)
        {
            var symbol = compilation.GetTypeByMetadataName(metadataName);
            return symbol != null &&
                SymbolEqualityComparer.Default.Equals(elem, symbol); // Check if element matches a specific type
        }

        if (Match("Godot.Vector2") ||
            Match("Godot.Vector3") ||
            Match("Godot.Vector4") ||
            Match("Godot.Color") ||
            Match("Godot.Rid") ||
            Match("Godot.StringName") ||
            Match("Godot.NodePath"))
        {
            return true; // Whitelisted Godot types are supported
        }

        // Handle arrays of GodotObject-derived types
        if (InheritsFromGodotObject(compilation, elem))
        {
            className = elem.Name; // Store the class name for reference
            return true;
        }

        return false; // Unsupported array type
    }

    /// <summary>
    /// Determines whether the provided type is a known Godot value type.
    /// </summary>
    /// <param name="compilation">The compilation context used for type resolution.</param>
    /// <param name="type">The type symbol to check.</param>
    /// <param name="variantType">The resolved VariantType, if found.</param>
    /// <returns>True if the type is a known Godot value type; otherwise, false.</returns>
    private static bool IsKnownGodotValueType(
        Compilation compilation,
        ITypeSymbol type,
        out VariantType variantType
    )
    {
        VariantType found = VariantType.Nil;

        bool Match(string metadataName, VariantType vt)
        {
            var symbol = compilation.GetTypeByMetadataName(metadataName);
            if (symbol == null) return false;

            if (SymbolEqualityComparer.Default.Equals(type, symbol))
            {
                found = vt; // Assign the matching VariantType
                return true;
            }

            return false;
        }

        var result =
            Match("Godot.Vector2", VariantType.Vector2) ||
            Match("Godot.Vector2I", VariantType.Vector2I) ||
            Match("Godot.Rect2", VariantType.Rect2) ||
            Match("Godot.Rect2I", VariantType.Rect2I) ||
            Match("Godot.Vector3", VariantType.Vector3) ||
            Match("Godot.Vector3I", VariantType.Vector3I) ||
            Match("Godot.Transform2D", VariantType.Transform2D) ||
            Match("Godot.Vector4", VariantType.Vector4) ||
            Match("Godot.Vector4I", VariantType.Vector4I) ||
            Match("Godot.Plane", VariantType.Plane) ||
            Match("Godot.Quaternion", VariantType.Quaternion) ||
            Match("Godot.Aabb", VariantType.Aabb) ||
            Match("Godot.Basis", VariantType.Basis) ||
            Match("Godot.Transform3D", VariantType.Transform3D) ||
            Match("Godot.Projection", VariantType.Projection) ||
            Match("Godot.Color", VariantType.Color) ||
            Match("Godot.StringName", VariantType.StringName) ||
            Match("Godot.NodePath", VariantType.NodePath) ||
            Match("Godot.Rid", VariantType.Rid) ||
            Match("Godot.Callable", VariantType.Callable) ||
            Match("Godot.Signal", VariantType.Signal) ||
            Match("Godot.Variant", VariantType.Nil);

        variantType = found; // Set the resolved VariantType
        return result; // Return whether a match was found
    }


    /// <summary>
    /// Determines whether the provided type inherits from Godot.GodotObject.
    /// </summary>
    /// <param name="compilation">The compilation context used for type resolution.</param>
    /// <param name="type">The type symbol to check.</param>
    /// <returns>True if the type inherits from Godot.GodotObject; otherwise, false.</returns>
    private static bool InheritsFromGodotObject(Compilation compilation, ITypeSymbol type)
    {
        var godotObject =
            compilation.GetTypeByMetadataName("Godot.GodotObject");

        if (godotObject == null)
            return false; // Unable to resolve GodotObject type

        for (var current = type; current != null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, godotObject))
                return true; // Found GodotObject in the inheritance chain
        }

        return false; // No inheritance from GodotObject
    }

}