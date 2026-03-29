using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DotPointers.OneOf.Generator
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class OneOfNoDefaultAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "DP0001";
		private const string Category = "Usage";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
			DiagnosticId,
			"Invalid creation of OneOf type",
			"Type '{0}' does not allow 'default' and 'new()'",
			Category,
			DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
			context.RegisterSyntaxNodeAction(AnalyzeImplicitObjectCreation, SyntaxKind.ImplicitObjectCreationExpression);
			context.RegisterSyntaxNodeAction(AnalyzeDefaultLiteral, SyntaxKind.DefaultLiteralExpression);
			context.RegisterSyntaxNodeAction(AnalyzeDefaultExpression, SyntaxKind.DefaultExpression);
		}

		private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
		{
			var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

			if (objectCreation.ArgumentList != null && objectCreation.ArgumentList.Arguments.Count > 0)
				return;

			CheckType(context, objectCreation);
		}

		private void AnalyzeImplicitObjectCreation(SyntaxNodeAnalysisContext context)
		{
			var implicitCreation = (ImplicitObjectCreationExpressionSyntax)context.Node;

			if (implicitCreation.ArgumentList != null && implicitCreation.ArgumentList.Arguments.Count > 0)
				return;

			CheckType(context, implicitCreation);
		}

		private void AnalyzeDefaultLiteral(SyntaxNodeAnalysisContext context)
		{
			CheckType(context, context.Node);
		}

		private void AnalyzeDefaultExpression(SyntaxNodeAnalysisContext context)
		{
			CheckType(context, context.Node);
		}

		private void CheckType(SyntaxNodeAnalysisContext context, SyntaxNode node)
		{
			var typeInfo = context.SemanticModel.GetTypeInfo(node);
			var symbol = typeInfo.Type as INamedTypeSymbol;

			if (symbol == null) return;

			foreach (var attr in symbol.GetAttributes())
			{
				if (attr.AttributeClass?.ToDisplayString() == "DotPointers.GenerateOneOfAttribute")
				{
					bool allowEmpty = true;

					if (attr.ConstructorArguments.Length >= 3)
					{
						var arg = attr.ConstructorArguments[2];
						if (arg.Value is bool val) allowEmpty = val;
					}

					foreach (var named in attr.NamedArguments)
					{
						if (named.Key == "allowEmpty" && named.Value.Value is bool val)
						{
							allowEmpty = val;
						}
					}

					if (!allowEmpty)
					{
						context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation(), symbol.Name));
					}
					break;
				}
			}
		}
	}
}