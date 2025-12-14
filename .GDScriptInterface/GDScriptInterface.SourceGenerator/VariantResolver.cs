using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace GDScriptInterface.SourceGenerator;

/// <summary>
/// Provides methods to resolve the VariantType for a given type symbol in the context of Godot's Variant system.
/// </summary>
internal static class VariantResolver
{

    /// <summary>
    /// Attempts to resolve the VariantType and optionally the class name for a given type symbol.
    /// This method handles various type categories including basic managed types, arrays, Godot-specific collections,
    /// known Godot value types, and GodotObject-derived classes. If successful, it populates the <paramref name="propInfo"/>
    /// parameter with the resolved information.
    /// </summary>
    /// <param name="compilation">The compilation context used for type resolution.</param>
    /// <param name="type">The type symbol to resolve.</param>
    /// <param name="propInfo">The resolved GDScript property information, if successful.</param>
    /// <returns>True if the type could be resolved; otherwise, false.</returns>
    public static bool TryResolveVariantType(
        Compilation compilation,
        ITypeSymbol type,
        out GDScriptPropertyInfo propInfo
    )
    {
        propInfo = null!;

        var hint = PropertyHint.None;
        var hintString = "";
        var usage = PropertyUsageFlags.None;
        var className = "";
        VariantType variantType;

        // Handle the special case where the type is Godot.Variant
        var variantSymbol = compilation.GetTypeByMetadataName("Godot.Variant");

        if (variantSymbol != null &&
            SymbolEqualityComparer.Default.Equals(type, variantSymbol))
        {
            // Map Godot.Variant to VariantType.Nil with NilIsVariant usage flag
            propInfo = new GDScriptPropertyInfo(
                className,
                VariantType.Nil,
                hint,
                hintString,
                PropertyUsageFlags.NilIsVariant
            );
            return true;
        }

        // Handle enum types by mapping them to VariantType.Int
        if (type.TypeKind == TypeKind.Enum)
        {
            // All integer types map to Int in VariantType
            propInfo = new GDScriptPropertyInfo(
                className,
                VariantType.Int,
                hint,
                hintString,
                PropertyUsageFlags.ClassIsEnum
            );
            return true;
        }

        // Handle void type (only allowed for return values)
        if (type.SpecialType == SpecialType.System_Void)
        {
            variantType = VariantType.Nil; // Void maps to Nil in VariantType
            usage = PropertyUsageFlags.Default; // Default usage for return values

            propInfo = new GDScriptPropertyInfo(
                className,
                variantType,
                hint,
                hintString,
                usage
            );
            return true;
        }

        // Handle basic managed types (primitives like bool, string, float, etc.)
        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
                variantType = VariantType.Bool; // Boolean maps to Bool in VariantType
                propInfo = new GDScriptPropertyInfo(
                    className,
                    variantType,
                    hint,
                    hintString,
                    usage
                );
                return true;

            case SpecialType.System_String:
                variantType = VariantType.String; // String maps to String in VariantType
                propInfo = new GDScriptPropertyInfo(
                    className,
                    variantType,
                    hint,
                    hintString,
                    usage
                );
                return true;

            case SpecialType.System_Single:
            case SpecialType.System_Double:
                variantType = VariantType.Float; // Single/Double maps to Float in VariantType
                propInfo = new GDScriptPropertyInfo(
                    className,
                    variantType,
                    hint,
                    hintString,
                    usage
                );
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
                propInfo = new GDScriptPropertyInfo(
                    className,
                    variantType,
                    hint,
                    hintString,
                    usage
                );
                return true;

            case SpecialType.System_Decimal:
                return false; // Decimal is explicitly not supported
        }

        // Handle System.Array (T[])
        if (type is IArrayTypeSymbol array)
        {
             // Delegate array handling to helper method
            if (IsSupportedSystemArray(
                    compilation,
                    array,
                    out variantType,
                    out hint,
                    out hintString
                )
            )
            {
                propInfo = new GDScriptPropertyInfo(
                    className,
                    variantType,
                    hint,
                    hintString,
                    usage
                );
                return true;
            }
            return false; // Unsupported array type
        }

        // Handle Godot.Collections.Array / Dictionary
        if (type is INamedTypeSymbol generic)
        {
            var fullName = generic.ConstructedFrom.ToDisplayString();

            if (fullName == "Godot.Collections.Array")
            {
                // Unparameterized Array maps to VariantType.Array
                variantType = VariantType.Array;
                propInfo = new GDScriptPropertyInfo(
                    className,
                    variantType,
                    hint,
                    hintString,
                    usage
                );
                return true;
            }

            if (fullName == "Godot.Collections.Array<T>")
            {
                // Recursively resolve the type argument of the generic array
                if (
                    TryResolveVariantType(
                        compilation,
                        generic.TypeArguments[0],
                        out var genericPropInfo
                    )
                )
                {
                    if (genericPropInfo == null)
                        return false;

                    // Generic parameter is Variant, map to non-typed Array
                    if (genericPropInfo.Usage == PropertyUsageFlags.NilIsVariant)
                    {
                        propInfo = new GDScriptPropertyInfo(
                            "",
                            VariantType.Array,
                            PropertyHint.None,
                            "",
                            PropertyUsageFlags.None
                        );
                        return true;
                    }
                    
                    // Otherwise, create a Typed Array with a specific type hint
                    propInfo = new GDScriptPropertyInfo(
                        className,
                        VariantType.Array,
                        PropertyHint.ArrayType,
                        genericPropInfo.VariantType == VariantType.Object 
                            ? genericPropInfo.ClassName 
                            : genericPropInfo.VariantType.MapToGDScriptName(),
                        PropertyUsageFlags.None
                    );

                    return true;
                }

                return false; // Failed to resolve the generic type argument
            }

            if (fullName == "Godot.Collections.Dictionary")
            {
                // Unparameterized Dictionary maps to VariantType.Dictionary
                propInfo = new GDScriptPropertyInfo(
                    className,
                    VariantType.Dictionary,
                    hint,
                    hintString,
                    usage
                );
                return true;            
            }

            if (fullName == "Godot.Collections.Dictionary<TKey, TValue>")
            {
                // Resolve key and value types recursively
                bool keyValid = TryResolveVariantType(
                    compilation,
                    generic.TypeArguments[0],
                    out var keyPropInfo
                );

                if (!keyValid)
                    return false;

                bool valueValid = TryResolveVariantType(
                    compilation,
                    generic.TypeArguments[1],
                    out var valuePropInfo
                );

                if (!valueValid)
                    return false;

                // Both key and value are Variant, map to non-typed Dictionary
                if (keyPropInfo.Usage == PropertyUsageFlags.NilIsVariant && valuePropInfo.Usage == PropertyUsageFlags.NilIsVariant)
                {
                    propInfo = new GDScriptPropertyInfo(
                        "",
                        VariantType.Dictionary,
                        PropertyHint.None,
                        "",
                        PropertyUsageFlags.None
                    );
                    return true;
                }

                // Create a Typed Dictionary with specific key and value type hints
                var keyType = keyPropInfo.VariantType == VariantType.Object
                    ? keyPropInfo.ClassName
                    : keyPropInfo.VariantType.MapToGDScriptName();

                var valueType = valuePropInfo.VariantType == VariantType.Object 
                    ? valuePropInfo.ClassName 
                    : valuePropInfo.VariantType.MapToGDScriptName();

                propInfo = new GDScriptPropertyInfo(
                    className,
                    VariantType.Dictionary,
                    PropertyHint.DictionaryType,
                    $"{keyType};{valueType}",
                    PropertyUsageFlags.None
                );

                return true;
            }
        }

        // Handle known Godot value types using a helper method
        if (IsKnownGodotValueType(compilation, type, out variantType))
        {
            propInfo = new GDScriptPropertyInfo(
                className,
                variantType,
                hint,
                hintString,
                usage
            );
            return true;
        }

        // Handle GodotObject and derived classes
        if (InheritsFromGodotObject(compilation, type))
        {
            variantType = VariantType.Object; // All GodotObject-derived types map to VariantType.Object
            className = GetFirstGodotNamespaceAncestorName(type); // Store the class name for reference
            propInfo = new GDScriptPropertyInfo(
                className,
                variantType,
                hint,
                hintString,
                usage
            );
            return true;
        }

        return false; // Type cannot be resolved
    }

    /// <summary>
    /// Determines whether the provided array type is supported by Godot's Variant system.
    /// This method checks if the array type matches any of the supported primitive types, 
    /// whitelisted Godot struct types, or arrays of GodotObject-derived types.
    /// If the array is supported, the corresponding <see cref="VariantType"/> and optional hints are resolved.
    /// </summary>
    /// <param name="compilation">The compilation context used for type resolution.</param>
    /// <param name="array">The array type symbol to check.</param>
    /// <param name="variantType">The resolved VariantType, if the array is supported.</param>
    /// <param name="hint">The resolved PropertyHint, providing additional information about the array type.</param>
    /// <param name="hintString">The resolved hint string, used in conjunction with the PropertyHint.</param>
    /// <returns>True if the array type is supported; otherwise, false.</returns>
    private static bool IsSupportedSystemArray(
        Compilation compilation,
        IArrayTypeSymbol array,
        out VariantType variantType,
        out PropertyHint hint,
        out string hintString
    )
    {
        variantType = VariantType.Array;
        hint = PropertyHint.None;
        hintString = "";

        if (array.Rank != 1)
            return false; // Only single-dimensional arrays are supported

        var elem = array.ElementType;

        // Handle basic managed types (primitives like int, float, string, etc.)
        switch (elem.SpecialType)
        {
            case SpecialType.System_Int32:
                variantType = VariantType.PackedInt32Array; // Maps int[] to PackedInt32Array
                return true;
            case SpecialType.System_Int64:
                variantType = VariantType.PackedInt64Array; // Maps long[] to PackedInt64Array
                return true;
            case SpecialType.System_Byte:
                variantType = VariantType.PackedByteArray; // Maps byte[] to PackedByteArray
                return true;
            case SpecialType.System_Single:
                variantType = VariantType.PackedFloat32Array; // Maps float[] to PackedFloat32Array
                return true;
            case SpecialType.System_Double:
                variantType = VariantType.PackedFloat64Array; // Maps double[] to PackedFloat64Array
                return true;
            case SpecialType.System_String:
                variantType = VariantType.PackedStringArray; // Maps string[] to PackedStringArray
                return true;
        }

        // Helper function to check if the element type matches a specific Godot type by metadata name
        bool Match(string metadataName)
        {
            var symbol = compilation.GetTypeByMetadataName(metadataName);
            return symbol != null &&
                SymbolEqualityComparer.Default.Equals(elem, symbol); // Compare symbols for equality
        }

        // Handle whitelisted Godot struct types
        var isVector2Match = Match("Godot.Vector2");
        if (isVector2Match)
        {
            variantType = VariantType.PackedVector2Array; // Maps Vector2[] to PackedVector2Array
            return true;
        }

        var isVector3Match = Match("Godot.Vector3");
        if (isVector3Match)
        {
            variantType = VariantType.PackedVector3Array; // Maps Vector3[] to PackedVector3Array
            return true;
        }

        var isVector4Match = Match("Godot.Vector4");
        if (isVector4Match)
        {
            variantType = VariantType.PackedVector4Array; // Maps Vector4[] to PackedVector4Array
            return true;
        }

        var isColorMatch = Match("Godot.Color");
        if (isColorMatch)
        {
            variantType = VariantType.PackedColorArray; // Maps Color[] to PackedColorArray
            return true;
        }

        var isRidMatch = Match("Godot.Rid");
        if (isRidMatch)
        {
            hint = PropertyHint.ArrayType; // Maps Rid[] to Array[RID]
            hintString = "RID"; // Specifies the type as "RID"
            return true; 
        }

        var isStringNameMatch = Match("Godot.StringName");
        if (isStringNameMatch)
        {
            hint = PropertyHint.ArrayType; // Maps StringName[] to Array[StringName]
            hintString = "StringName"; // Specifies the type as "StringName"
            return true;
        }

        var isNodePathMatch = Match("Godot.NodePath");
        if (isNodePathMatch)
        {
            hint = PropertyHint.ArrayType; // Maps StringName[] to Array[StringName]
            hintString = "NodePath"; // Specifies the type as "NodePath"
            return true;
        }

       // Handle arrays of GodotObject-derived types
        if (InheritsFromGodotObject(compilation, elem))
        {
            // Indicates an array of GodotObject-derived types, e.g., GodotObject[] -> Array[Object], Node[] -> Array[Node]
            hint = PropertyHint.ArrayType; 
            hintString = GetFirstGodotNamespaceAncestorName(elem); // Retrieves the class name for reference
            return true;
        }

        return false; // Unsupported array type
    }

    /// <summary>
    /// Retrieves the name of the first ancestor type within the "Godot" namespace for the given type symbol.
    /// This method traverses the inheritance hierarchy of the provided type and checks each base type
    /// to determine if it belongs to the "Godot" namespace. If a match is found, the corresponding type name
    /// is mapped using <see cref="MapSpecialCSharpTypeName"/> to ensure compatibility with Godot's naming conventions.
    /// If no ancestor in the "Godot" namespace is found, the original type name is returned.
    /// </summary>
    /// <param name="type">The type symbol whose ancestor name is to be retrieved.</param>
    /// <returns>A string representing the mapped name of the first ancestor type in the "Godot" namespace,
    /// or the original type name if no such ancestor exists.</returns>
    private static string GetFirstGodotNamespaceAncestorName(ITypeSymbol type)
    {
        for (var current = type; current != null; current = current.BaseType)
        {
            var ns = current.ContainingNamespace;
            if (ns != null && ns.ToDisplayString() == "Godot")
                return MapSpecialCSharpTypeName(current.Name);
        }

        return type.Name;
    }

    /// <summary>
    /// Maps special C# type names to their corresponding GDScript naming conventions.
    /// This method is used to ensure compatibility between C# type names and Godot's internal naming standards.
    /// For example, "GodotObject" is mapped to "Object", and "Json" is mapped to "JSON".
    /// If the provided name does not match any predefined mappings, it is returned unchanged.
    /// </summary>
    /// <param name="name">The original C# type name to map.</param>
    /// <returns>A string representing the mapped Godot-specific type name, or the original name if no mapping exists.</returns>
    private static string MapSpecialCSharpTypeName(string name) => name switch
    {
        "GodotObject" => "Object",
        "ResourceUid" => "ResourceUID",
        "Json" => "JSON",
        "JniSingleton" => "JNISingleton",
        "Xrvrs" => "XRVRS",
        "JsonRpc" => "JSONRpc",
        "Generic6DofJoint3D" => "Generic6DOFJoint3D",
        "GpuParticles2D" => "GPUParticles2D",
        "CpuParticles2D" => "CPUParticles2D",
        "CpuParticles3D" => "CPUParticles3D",
        "CsgShape3D" => "CSGShape3D",
        "CsgCombiner3D" => "CSGCombiner3D",
        "CsgPrimitive3D" => "CSGPrimitive3D",
        "CsgBox3D" => "CSGBox3D",
        "CsgCylinder3D" => "CSGCylinder3D",
        "CsgMesh3D" => "CSGMesh3D",
        "CsgPolygon3D" => "CSGPolygon3D",
        "CsgSphere3D" => "CSGSphere3D",
        "CsgTorus3D" => "CSGTorus3D",
        "GpuParticles3D" => "GPUParticles3D",
        "GpuParticlesAttractor3D" => "GPUParticlesAttractor3D",
        "GpuParticlesAttractorBox3D" => "GPUParticlesAttractorBox3D",
        "GpuParticlesAttractorSphere3D" => "GPUParticlesAttractorSphere3D",
        "GpuParticlesAttractorVectorField3D" => "GPUParticlesAttractorVectorField3D",
        "GpuParticlesCollision3D" => "GPUParticlesCollision3D",
        "GpuParticlesCollisionBox3D" => "GPUParticlesCollisionBox3D",
        "GpuParticlesCollisionHeightField3D" => "GPUParticlesCollisionHeightField3D",
        "GpuParticlesCollisionSdf3D" => "GPUParticlesCollisionSdf3D",
        "GpuParticlesCollisionSphere3D" => "GPUParticlesCollisionSphere3D",
        "AesContext" => "AESContext",
        "DtlsServer" => "DTLSServer",
        "EditorSceneFormatImporterFbx2Gltf" => "EditorSceneFormatImporterFBX2GLTF",
        "EditorSceneFormatImporterGltf" => "EditorSceneFormatImporterGLTF",
        "EditorSceneFormatImporterUfbx" => "EditorSceneFormatImporterUFBX",
        "EncodedObjectAsId" => "EncodedObjectAsID",
        "GltfObjectModelProperty" => "GLTFObjectModelProperty",
        "HmacContext" => "HMACContext",
        "HttpClient" => "HTTPClient",
        "MultiplayerApi" => "MultiplayerAPI",
        "MultiplayerApiExtension" => "MultiplayerAPIExtension",
        "OpenXRApiExtension" => "OpenXRAPIExtension",
        "WebRtcMultiplayerPeer" => "WebRTCMultiplayerPeer",
        "PacketPeerDtls" => "PacketPeerDTLS",
        "PacketPeerUdp" => "PacketPeerUDP",
        "WebRtcDataChannel" => "WebRTCDataChannel",
        "WebRtcDataChannelExtension" => "WebRTCDataChannelExtension",
        "PckPacker" => "PCKPacker",
        "AudioStreamWav" => "AudioStreamWAV",
        "GltfAccessor" => "GLTFAccessor",
        "GltfAnimation" => "GLTFAccessor",
        "GltfBufferView" => "GLTFBufferView",
        "GltfCamera" => "GLTFCamera",
        "GltfDocument" => "GLTFDocument",
        "FbxDocument" => "FBXDocument",
        "GltfDocumentExtension" => "GLTFDocumentExtension",
        "GltfDocumentExtensionConvertImporterMesh" => "GLTFDocumentExtensionConvertImporterMesh",
        "GltfLight" => "GLTFLight",
        "GltfMesh" => "GLTFMesh",
        "GltfNode" => "GLTFNode",
        "GltfPhysicsBody" => "GLTFPhysicsBody",
        "GltfPhysicsShape" => "GLTFPhysicsShape",
        "GltfSkeleton" => "GLTFSkeleton",
        "GltfSkin" => "GLTFSkin",
        "GltfSpecGloss" => "GLTFSpecGloss",
        "GltfState" => "GLTFState",
        "FbxState" => "FBXState",
        "GltfTexture" => "GLTFTexture",
        "GltfTextureSampler" => "GLTFTextureSampler",
        "InputEventMidi" => "InputEventMIDI",
        "OpenXripBindingModifier" => "OpenXRIPBindingModifier",
        "RDShaderSpirV" => "RDShaderSPIRV",
        "SkeletonModification2DCcdik" => "SkeletonModification2DCCDIK",
        "SkeletonModification2DFabrik" => "SkeletonModification2DFABRIK",
        "CurveXyzTexture" => "CurveXYZTexture",
        "DpiTexture" => "DPITexture",
        "Texture2Drd" => "Texture2DRD",
        "Texture3Drd" => "Texture3DRD",
        "ResourceImporterCsvTranslation" => "ResourceImporterCSVTranslation",
        "ResourceImporterObj" => "ResourceImporterOBJ",
        "ResourceImporterSvg" => "ResourceImporterSVG",
        "ResourceImporterWav" => "ResourceImporterWAV",
        "StreamPeerGZip" => "StreamPeerGZIP",
        "StreamPeerTcp" => "StreamPeerTCP",
        "StreamPeerTls" => "StreamPeerTLS",
        "TcpServer" => "TCPServer",
        "TlsOptions" => "TLsOptions",
        "UdpServer" => "UDPServer",
        "Upnp" => "UPNP",
        "UpnpDevice" => "UPNPDevice",
        "WebRtcPeerConnection" => "WebRTCPeerConnection",
        "WebRtcPeerConnectionExtension" => "WebRTCPeerConnectionExtension",
        "XmlParser" => "XMLParser",
        "ZipPacker" => "ZIPPacker",
        "ZipReader" => "ZIPReader",
        _ => name
    };

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
            Match("Godot.Signal", VariantType.Signal);

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