using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GDScriptInterface.SourceGenerator;

/// <summary>
/// Provides methods to resolve and validate interfaces for generating GDScript interface mappings.
/// </summary>
internal static class InterfaceResolver
{
    /// <summary>
    /// Attempts to resolve the target interface and build a type map for the given class symbol.
    /// </summary>
    /// <param name="context">The source production context used for reporting diagnostics.</param>
    /// <param name="compilation">The compilation context used for type resolution.</param>
    /// <param name="classSymbol">The class symbol to analyze.</param>
    /// <param name="iface">The resolved interface symbol, if successful.</param>
    /// <param name="propInfo">The resolved GDScript property information.</param>
    /// <returns>True if the interface was successfully resolved; otherwise, false.</returns>
    public static bool TryResolveTargetInterface(
        SourceProductionContext context,
        Compilation compilation,
        INamedTypeSymbol classSymbol,
        out INamedTypeSymbol iface,
        out Dictionary<ITypeSymbol, GDScriptPropertyInfo> propInfo)
    {
        iface = null!;
        propInfo = []; // Initialize an empty type map

        // Find the attribute marking the class for interface mapping
        var attr = classSymbol.GetAttributes()
            .FirstOrDefault(a =>
                a.AttributeClass?.Name ==
                "GenerateGDScriptInterfaceMappingAttribute");

        if (attr is null)
            return false; // No matching attribute found

        // Validate the attribute's constructor arguments
        if (attr.ConstructorArguments.Length != 1 ||
            attr.ConstructorArguments[0].Value is not INamedTypeSymbol ifaceSymbol)
        {
            ErrorReporter.ReportInvalidAttributeUsage(
                context,
                classSymbol,
                "Attribute requires a single interface type parameter");
            return false; // Invalid attribute usage
        }

        // Ensure the provided type is an interface
        if (ifaceSymbol.TypeKind != TypeKind.Interface)
        {
            ErrorReporter.ReportInvalidAttributeUsage(
                context,
                classSymbol,
                $"Type '{ifaceSymbol.Name}' is not an interface");
            return false; // Provided type is not an interface
        }

        iface = ifaceSymbol;

        // Validate the interface and build the type map
        if (!ValidateInterface(context, compilation, iface, out propInfo))
            return false;

        return true; // Successfully resolved the interface
    }

    /// <summary>
    /// Validates the given interface and ensures all its members are compatible with the Variant system.
    /// </summary>
    /// <param name="context">The source production context used for reporting diagnostics.</param>
    /// <param name="compilation">The compilation context used for type resolution.</param>
    /// <param name="iface">The interface symbol to validate.</param>
    /// <param name="propInfo">The resolved GDScript property information.</param>
    /// <returns>True if the interface is valid; otherwise, false.</returns>
    private static bool ValidateInterface(
        SourceProductionContext context,
        Compilation compilation,
        INamedTypeSymbol iface,
        out Dictionary<ITypeSymbol, GDScriptPropertyInfo> propInfo
    )
    {
        var localPropInfo = new Dictionary<ITypeSymbol, GDScriptPropertyInfo>(SymbolEqualityComparer.Default);
        
        // Generic interfaces are not supported
        if (iface.IsGenericType)
        {
            ErrorReporter.ReportGenericInterfaceNotSupported(context, iface);
            propInfo = [];
            return false;
        }
    
        // Helper function to ensure a type is resolved and added to the type map
        bool Ensure(ITypeSymbol t, ISymbol symbol)
        {
            if (t == null) return true; // Skip null types
            if (localPropInfo.ContainsKey(t)) return true; // Skip already-resolved types

            if (VariantResolver.TryResolveVariantType(compilation, t, out var propInfo))
            {
                localPropInfo[t] = propInfo; // Add resolved propInfo to the map
                return true;
            }

            ErrorReporter.ReportInvalidType(context, iface, t, symbol); // Report invalid type
            return false;
        }

        // Process method parameters and return types
        foreach (var method in iface.GetMembers().OfType<IMethodSymbol>())
        {
            // Generic methods are not supported
            if (method.IsGenericMethod)
            {
                ErrorReporter.ReportGenericMethodNotSupported(context, iface, method);
                propInfo = [];
                return false;
            }
        
            if (method.MethodKind != MethodKind.Ordinary)
                continue; // Skip non-ordinary methods (e.g., constructors)

            foreach (var param in method.Parameters)
            {
                if (!Ensure(param.Type, param))
                {
                    propInfo = [];
                    return false;
                }
            }

            if (!Ensure(method.ReturnType, method))
            {
                propInfo = [];
                return false;
            }
        }

        // Process property types and index parameters
        foreach (var prop in iface.GetMembers().OfType<IPropertySymbol>())
        {
            // Indexers are not supported
            if (prop.IsIndexer)
            {
                ErrorReporter.ReportIndexerNotSupported(context, iface, prop);
                propInfo = [];
                return false;
            }
    
            if (!Ensure(prop.Type, prop))
            {
                propInfo = [];
                return false;
            }
        }

        propInfo = localPropInfo;
        return true;
    }
}