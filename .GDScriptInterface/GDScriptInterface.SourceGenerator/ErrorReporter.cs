using System;
using System.Linq;
using Microsoft.CodeAnalysis;

using static GDScriptInterface.SourceGenerator.Analyzer.Diagnostics;

namespace GDScriptInterface.SourceGenerator;

/// <summary>
/// Provides methods for reporting diagnostics (errors) during source generation.
/// This class is responsible for creating and reporting diagnostic messages 
/// based on invalid usage of attributes, types, or unsupported features.
/// </summary>
internal static class ErrorReporter
{
    /// <summary>
    /// Reports an error when an invalid type is used in the context of an interface member.
    /// </summary>
    /// <param name="context">The source production context used to report the diagnostic.</param>
    /// <param name="iface">The named type symbol representing the interface.</param>
    /// <param name="invalidType">The type symbol that is considered invalid.</param>
    /// <param name="member">The symbol representing the member where the invalid type was used.</param>
    public static void ReportInvalidType(
        SourceProductionContext context,
        INamedTypeSymbol iface,
        ITypeSymbol invalidType,
        ISymbol member)
    {
        var diagnostic = Diagnostic.Create(
            InvalidVariantTypeDescriptor, // Descriptor for invalid variant type diagnostics.
            member.Locations.FirstOrDefault(), // Location of the member in the source code.
            new object[]
            {
                iface.Name, // Name of the interface.
                invalidType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), // Display string of the invalid type.
                member.Name // Name of the member.
            }
        );

        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Reports an error when an attribute is used incorrectly.
    /// </summary>
    /// <param name="context">The source production context used to report the diagnostic.</param>
    /// <param name="target">The symbol where the invalid attribute usage occurred.</param>
    /// <param name="message">A custom message describing the error.</param>
    public static void ReportInvalidAttributeUsage(SourceProductionContext context, ISymbol target, string message)
    {
        var diagnostic = Diagnostic.Create(
            descriptor: InvalidAttributeUsageDescriptor, // Descriptor for invalid attribute usage diagnostics.
            location: target.Locations.FirstOrDefault(), // Location of the target symbol in the source code.
            messageArgs: new object[] { message } // Custom error message.
        );

        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Reports an error when an indexer is used in a context where it is not supported.
    /// </summary>
    /// <param name="context">The source production context used to report the diagnostic.</param>
    /// <param name="iface">The named type symbol representing the interface.</param>
    /// <param name="prop">The property symbol representing the indexer.</param>
    public static void ReportIndexerNotSupported(SourceProductionContext context, INamedTypeSymbol iface, IPropertySymbol prop)
    {
        var diagnostic = Diagnostic.Create(
            descriptor: IndexerNotSupportedDescriptor, // Descriptor for indexer not supported diagnostics.
            location: prop.Locations.FirstOrDefault(), // Location of the property in the source code.
            messageArgs: new object[]
            {
                iface.Name, // Name of the interface.
                prop.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) // Display string of the property.
            }
        );

        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Reports an error when a generic interface is used in a context where it is not supported.
    /// </summary>
    /// <param name="context">The source production context used to report the diagnostic.</param>
    /// <param name="iface">The named type symbol representing the generic interface.</param>
    public static void ReportGenericInterfaceNotSupported(SourceProductionContext context, INamedTypeSymbol iface)
    {
        var diagnostic = Diagnostic.Create(
            GenericInterfaceNotSupported, // Descriptor for generic interface not supported diagnostics.
            iface.Locations.FirstOrDefault(), // Location of the interface in the source code.
            iface.Name // Name of the interface.
        );
        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Reports an error when a generic method is used in a context where it is not supported.
    /// </summary>
    /// <param name="context">The source production context used to report the diagnostic.</param>
    /// <param name="iface">The named type symbol representing the interface containing the method.</param>
    /// <param name="method">The method symbol representing the generic method.</param>
    public static void ReportGenericMethodNotSupported(SourceProductionContext context, INamedTypeSymbol iface, IMethodSymbol method)
    {
        var diagnostic = Diagnostic.Create(
            GenericMethodNotSupported, // Descriptor for generic method not supported diagnostics.
            method.Locations.FirstOrDefault(), // Location of the method in the source code.
            method.Name, // Name of the method.
            iface.Name // Name of the interface.
        );
        context.ReportDiagnostic(diagnostic);
    }
}