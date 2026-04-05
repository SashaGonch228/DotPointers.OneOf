using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace DotPointers.OneOf.Generator
{

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class OneOfDeclarationAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticIdNested = "DP0002";
		public const string DiagnosticIdNotPartial = "DP0003";
		public const string DiagnosticIdNotReadonly = "DP0004";

		internal static readonly DiagnosticDescriptor NestedError = new(
			DiagnosticIdNested,
			"Type cannot be nested",
			"The type '{0}' must be a top-level declaration to use [GenerateOneOf]",
			"Design",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		internal static readonly DiagnosticDescriptor NotPartialError = new(
			DiagnosticIdNotPartial,
			"Type must be partial",
			"The type '{0}' must be declared as partial to support source generation",
			"Design",
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		internal static readonly DiagnosticDescriptor NotReadonlyWarning = new(
			DiagnosticIdNotReadonly,
			"Struct should be readonly",
			"The struct '{0}' should be declared as 'readonly' for better performance",
			"Performance",
			DiagnosticSeverity.Warning,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(NestedError, NotPartialError, NotReadonlyWarning);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();
			context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
		}

		private static void AnalyzeSymbol(SymbolAnalysisContext context)
		{
			var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

			var hasAttribute = namedTypeSymbol.GetAttributes().Any(static ad => ad.AttributeClass?.Name == "GenerateOneOfAttribute");

			if (!hasAttribute) { return; }

			if (namedTypeSymbol.ContainingType != null)
			{
				context.ReportDiagnostic(Diagnostic.Create(NestedError, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
			}

			var syntaxReferences = namedTypeSymbol.DeclaringSyntaxReferences;
			foreach (var reference in syntaxReferences)
			{
				if (reference.GetSyntax() is Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax node && !node.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
				{
					context.ReportDiagnostic(Diagnostic.Create(NotPartialError, node.Identifier.GetLocation(), namedTypeSymbol.Name));
				}
			}

			if (namedTypeSymbol.TypeKind == TypeKind.Struct && !namedTypeSymbol.IsReadOnly)
			{
				context.ReportDiagnostic(Diagnostic.Create(NotReadonlyWarning, namedTypeSymbol.Locations[0], namedTypeSymbol.Name));
			}
		}
	}
}