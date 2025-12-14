using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace GDScriptInterface.SourceGenerator.Analyzer;

/// <summary>
/// A diagnostic analyzer that ensures classes marked with the [GenerateGDScriptInterfaceMappingAttribute]
/// are declared as both static and partial.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class GenerateMappingUsageAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the supported diagnostics for this analyzer.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [Diagnostics.MustBeStaticPartialClass]; // Returns the diagnostic descriptor for invalid class declarations.

    /// <summary>
    /// Initializes the analyzer and registers actions to analyze syntax nodes.
    /// </summary>
    /// <param name="context">The analysis context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None); // Disable analysis of generated code.
        context.EnableConcurrentExecution(); // Enable concurrent execution for performance optimization.

        context.RegisterSyntaxNodeAction(
            AnalyzeClass, // Registers the action to analyze class declarations.
            SyntaxKind.ClassDeclaration
        );
    }

    /// <summary>
    /// Analyzes a class declaration to ensure it meets the requirements for the [GenerateGDScriptInterfaceMappingAttribute]
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node; // Cast the node to a class declaration syntax.

        if (classDecl.AttributeLists.Count == 0)
            return; // Skip if the class has no attributes.

        var symbol = context.SemanticModel.GetDeclaredSymbol(classDecl);
        if (symbol is null)
            return; // Skip if the symbol cannot be retrieved.

        // Check if the class has the [GenerateGDScriptInterfaceMappingAttribute]
        var hasAttr = symbol.GetAttributes()
            .Any(a => a.AttributeClass?.Name == "GenerateGDScriptInterfaceMappingAttribute");

        if (!hasAttr)
            return; // Skip if the attribute is not present.

        bool isStatic = classDecl.Modifiers.Any(SyntaxKind.StaticKeyword); // Check if the class is static.
        bool isPartial = classDecl.Modifiers.Any(SyntaxKind.PartialKeyword); // Check if the class is partial.

        if (isStatic && isPartial)
            return; // The class meets the requirements; no diagnostic needed.

        // Report a diagnostic if the class is not both static and partial.
        context.ReportDiagnostic(
            Diagnostic.Create(
                Diagnostics.MustBeStaticPartialClass, // Descriptor for the diagnostic.
                classDecl.Identifier.GetLocation() // Location of the class identifier in the source code.
            )
        );
    }
}
