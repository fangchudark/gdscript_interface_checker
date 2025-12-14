using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace GDScriptInterface.SourceGenerator;

// This class is responsible for generating interface mappings during the compilation process.
[Generator]
internal class InterfaceMappingGenerator : IIncrementalGenerator
{
    // Initializes the generator with the given context.
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Filters and retrieves static class symbols from the syntax tree.
        var classSymbols = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is ClassDeclarationSyntax cds &&
                    cds.Modifiers.Any(SyntaxKind.StaticKeyword), // Predicate to check if the class is static.
                transform: static (ctx, _) =>
                {
                    var classDecl = (ClassDeclarationSyntax)ctx.Node; // Cast the node to ClassDeclarationSyntax.
                    return ctx.SemanticModel.GetDeclaredSymbol(classDecl); // Retrieve the declared symbol for the class.
                })
            .Where(static s => s is not null) // Filter out null symbols.
            .Select((s, _) => s!); // Select non-null symbols.

        // Combines the compilation with the collected class symbols.
        var compilationAndClasses =
            context.CompilationProvider.Combine(classSymbols.Collect());

        // Registers a source output action that generates code based on the collected data.
        context.RegisterSourceOutput(compilationAndClasses, (spc, item) =>
        {
            var compilation = item.Left; // The current compilation.
            var classes = item.Right; // The collected class symbols.

            foreach (var classSymbol in classes)
            {
                // Attempts to resolve the target interface for the class symbol.
                if (!InterfaceResolver.TryResolveTargetInterface(
                        spc,
                        compilation,
                        classSymbol,
                        out var iface,
                        out var typeMap))
                    continue; // Skip if the interface resolution fails.

                // Generates the interface mapping code for the resolved interface.
                CodeBuilder.GenerateInterfaceMapping(
                    spc,
                    classSymbol,
                    iface,
                    typeMap
                );
            }
        });
    }
}