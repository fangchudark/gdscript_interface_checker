using Microsoft.CodeAnalysis;

namespace GDScriptInterface.SourceGenerator.Analyzer;

/// <summary>
/// Provides diagnostic descriptors for errors and warnings related to GDScript interface generation.
/// Each descriptor defines a unique error code, message format, and metadata for reporting issues.
/// </summary>
internal static class Diagnostics
{
    /// <summary>
    /// Diagnostic descriptor for invalid Variant-compatible types in interfaces.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidVariantTypeDescriptor =
        new DiagnosticDescriptor(
            id: "GDINTF001",
            title: "Invalid Variant-compatible type",
            messageFormat:
                "Interface '{0}' uses type '{1}' in member '{2}', which is not compatible with Godot Variant. " +
                "This interface cannot be used with [GenerateGDScriptInterfaceMapping].",
            category: "GDScriptInterfaceGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "This rule ensures that only Variant-compatible types are used in interfaces."
        );

    /// <summary>
    /// Diagnostic descriptor for invalid usage of attributes in the context of GDScript interface generation.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidAttributeUsageDescriptor =
        new DiagnosticDescriptor(
            id: "GDINTF002",
            title: "Invalid attribute usage",
            messageFormat: "{0}",
            category: "GDScriptInterfaceGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "This rule ensures that attributes are used correctly in the context of GDScript interface generation."
       );

    /// <summary>
    /// Diagnostic descriptor for invalid usage of [GenerateGDScriptInterfaceMapping] on non-static or non-partial classes.
    /// </summary>
    public static readonly DiagnosticDescriptor MustBeStaticPartialClass =
        new DiagnosticDescriptor(
            id: "GDINTF003",
            title: "Invalid GenerateGDScriptInterfaceMapping usage",
            messageFormat:
                "[GenerateGDScriptInterfaceMapping] can only be applied to a static partial class",
            category: "GDScriptInterfaceGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "This rule ensures that [GenerateGDScriptInterfaceMapping] is applied to a static partial class."
        );

    /// <summary>
    /// Diagnostic descriptor for invalid usage of [GenerateGDScriptInterfaceMapping] on non-static or non-partial classes.
    /// </summary>
    public static readonly DiagnosticDescriptor IndexerNotSupportedDescriptor =
        new DiagnosticDescriptor(
            id: "GDINTF004",
            title: "Indexer not supported",
            messageFormat:
                "Interface '{0}' contains an indexer '{1}', which is not supported by GDScript interface generation",
            category: "GDScriptInterfaceGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "This rule ensures that indexers are not used in interfaces intended for GDScript."
        );

    /// <summary>
    /// Diagnostic descriptor for unsupported generic interfaces.
    /// </summary>
    public static readonly DiagnosticDescriptor GenericInterfaceNotSupported =
        new DiagnosticDescriptor(
            id: "GDINTF005",
            title: "Generic interface not supported",
            messageFormat:
                "Interface '{0}' has generic type parameters, which are not supported by GDScript interface generation",
            category: "GDScriptInterfaceGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
         );

    /// <summary>
    /// Diagnostic descriptor for unsupported generic methods in interfaces.
    /// </summary>
    public static readonly DiagnosticDescriptor GenericMethodNotSupported =
        new DiagnosticDescriptor(
            id: "GDINTF006",
            title: "Generic method not supported",
            messageFormat:
                "Method '{0}' in interface '{1}' has generic type parameters, which are not supported by GDScript interface generation",
            category: "GDScriptInterfaceGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

}