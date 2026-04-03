using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static DotPointers.OneOf.Generator.GenerationModel;

namespace DotPointers.OneOf.Generator
{
	[Generator]
	public class OneOfGenerator : IIncrementalGenerator
	{
		private const string AttrName = "DotPointers.OneOf.GenerateOneOfAttribute";
		

		private static readonly SymbolDisplayFormat TypeNameFormat = new SymbolDisplayFormat( 
			typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
			genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
//#if DEBUG
//			if (!System.Diagnostics.Debugger.IsAttached)
//			{
//				System.Diagnostics.Debugger.Launch();
//			}
//#endif

			var targets = context.SyntaxProvider.ForAttributeWithMetadataName(
				fullyQualifiedMetadataName: AttrName,
				predicate: static (node, _) => node is TypeDeclarationSyntax,
				transform: static (ctx, _) => GetModel(ctx))
				.Where(static x => x != null)
				.WithComparer(GenerationModelComparer.Instance)!;

			context.RegisterSourceOutput(targets, static (spc, model) =>
			{
				if (model == null) { return; }
				var source = OneOfSourceGenerator.GenerateSource(model);
				var ns = string.IsNullOrEmpty(model.Namespace) ? "Global" : model.Namespace;
				var path = $"{ns}.{model.FullName.Replace('<', '(').Replace('>', ')')}";

				spc.AddSource($"{path}.g.cs", SourceText.From(source, Encoding.UTF8));

				if (model.SerializeOption.HasFlag(Serialization.SystemJson))
				{
					spc.AddSource($"{path}.SystemJson.g.cs", SourceText.From(OneOfSourceGenerator.GenerateSystemJsonSource(model), Encoding.UTF8));
				}

				if (model.SerializeOption.HasFlag(Serialization.NewtonsoftJson))
				{
					spc.AddSource($"{path}.NewtonsoftJson.g.cs", SourceText.From(OneOfSourceGenerator.GenerateNewtonsoftJsonSource(model), Encoding.UTF8));
				}

				if (model.SerializeOption.HasFlag(Serialization.MemoryPack))
				{
					spc.AddSource($"{path}.MemoryPack.g.cs", SourceText.From(OneOfSourceGenerator.GenerateMemoryPackSource(model), Encoding.UTF8));
				}
			});

			var uniqueNamespaces = targets
				.Select(static (m, _) => m?.Namespace)
				.Collect()
				.SelectMany(static (namespaces, _) => namespaces.Distinct());

			context.RegisterSourceOutput(uniqueNamespaces, static (spc, ns) =>
			{
				var fileName = string.IsNullOrEmpty(ns) ? "Global" : ns;
				spc.AddSource($"{fileName}.OneOfThrowHelper.g.cs", SourceText.From(OneOfSourceGenerator.GenerateThrowHelper(ns), Encoding.UTF8));
			});
		}

		private static readonly string[] defaultFields = { "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth" };

		private static GenerationModel? GetModel(GeneratorAttributeSyntaxContext ctx)
		{
			var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
			var allAttributes = symbol.GetAttributes();

			var interfaceSymbol = symbol.AllInterfaces.FirstOrDefault(i =>
			{
				return i.Name == "IOneOf" && i.ContainingNamespace.ToDisplayString() == "DotPointers.OneOf";
			});

			var attributeData = allAttributes.FirstOrDefault(a =>
			{
				return a.AttributeClass?.ToDisplayString() == AttrName;
			});

			if (interfaceSymbol == null || attributeData == null)
			{
				return null;
			}

			Serialization serialize = 0;

			foreach (var attr in allAttributes)
			{
				string? attrFullName = attr.AttributeClass?.ToDisplayString();

				if (attrFullName == "DotPointers.OneOf.GenerateSystemJsonSupportAttribute")
				{
					serialize |= Serialization.SystemJson;
				}
				else if (attrFullName == "DotPointers.OneOf.GenerateNewtonsoftJsonSupportAttribute")
				{
					serialize |= Serialization.NewtonsoftJson;
				}
				else if (attrFullName == "DotPointers.OneOf.GenerateMemoryPackSupportAttribute")
				{
					serialize |= Serialization.MemoryPack;
				}
			}

			var typeArguments = interfaceSymbol.TypeArguments;
			var fields = ImmutableArray<string>.Empty;

			var arg0 = attributeData.ConstructorArguments.ElementAtOrDefault(0);
			if (!arg0.IsNull && arg0.Kind == TypedConstantKind.Array && !arg0.Values.IsDefaultOrEmpty)
			{
				fields = arg0.Values.Select(x =>
				{
					return x.Value?.ToString() ?? "Unknown";
				}).ToImmutableArray();
			}

			if (fields.Length == 0)
			{
				fields = typeArguments.Select((t, i) =>
				{
					if (i < defaultFields.Length)
					{
						return defaultFields[i];
					}
					return t.Name;
				}).ToImmutableArray();
			}

			var allowEmpty = (bool)(attributeData.ConstructorArguments.ElementAtOrDefault(1).Value ?? true);
			var requestedLayout = (OneOfLayoutKind)(int)(attributeData.ConstructorArguments.ElementAtOrDefault(2).Value ?? 0);

			var types = typeArguments.Select(t =>
			{
				return new TypeInfoModel(
					fullName: t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
					shortName: t.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
					isReferenceType: t.IsReferenceType,
					isRefStruct: t.IsRefLikeType
				);
			}).ToImmutableArray();

			var userMethods = symbol.GetMembers()
			.OfType<IMethodSymbol>()
			.Where(m => m.MethodKind == MethodKind.Ordinary && !m.IsImplicitlyDeclared)
			.Select(m => (
				m.Name,
				m.Parameters.Select(p => p.Type.ToDisplayString(TypeNameFormat)).ToImmutableArray()
			))
			.ToImmutableArray();


			return new GenerationModel(
				@namespace: symbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : symbol.ContainingNamespace.ToDisplayString(),
				name: symbol.Name,
				fullName: symbol.ToDisplayString(TypeNameFormat),
				typeKind: symbol.IsValueType ? (symbol.IsRefLikeType ? "ref struct" : "struct") : "class",
				allowEmpty: allowEmpty,
				requestedLayout: requestedLayout,
				isGeneric: symbol.IsGenericType,
				isRef: symbol.IsRefLikeType,
				typeArgs: types,
				fieldNames: fields,
				userFuncs: userMethods,
				serializeOption: serialize
			);
		}
	}

	public class GenerationModel
	{
		public GenerationModel(string @namespace, string name, string fullName, string typeKind, bool allowEmpty, OneOfLayoutKind requestedLayout, bool isGeneric, bool isRef, ImmutableArray<TypeInfoModel> typeArgs, ImmutableArray<string> fieldNames, ImmutableArray<(string, ImmutableArray<string>)> userFuncs, Serialization serializeOption)
		{
			Namespace = @namespace;
			Name = name;
			FullName = fullName;
			TypeKind = typeKind;
			AllowEmpty = allowEmpty;
			RequestedLayout = requestedLayout;
			IsGeneric = isGeneric;
			IsRef = isRef;
			TypeArgs = typeArgs;
			FieldNames = fieldNames;
			UserFuncs = userFuncs;
			SerializeOption = serializeOption;
		}

		public string Namespace { get; }
		public string Name { get; }
		public string FullName { get; }
		public string TypeKind { get; }
		public bool AllowEmpty { get; }
		public OneOfLayoutKind RequestedLayout { get; }
		public bool IsGeneric { get; }
		public bool IsRef { get; }
		public ImmutableArray<TypeInfoModel> TypeArgs { get; }
		public ImmutableArray<string> FieldNames { get; }
		public ImmutableArray<(string, ImmutableArray<string>)> UserFuncs { get; }
		public Serialization SerializeOption { get; }

		public class TypeInfoModel
		{
			public TypeInfoModel(string fullName, string shortName, bool isReferenceType, bool isRefStruct)
			{
				FullName = fullName;
				ShortName = shortName;
				IsReferenceType = isReferenceType;
				IsRefStruct = isRefStruct;
			}
			public string FullName { get; }
			public string ShortName { get; }
			public bool IsReferenceType { get; }
			public bool IsRefStruct { get; }
		}
	}

	internal class GenerationModelComparer : IEqualityComparer<GenerationModel?>
	{
		public static readonly GenerationModelComparer Instance = new();
		public bool Equals(GenerationModel? x, GenerationModel? y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			return x.Namespace == y.Namespace &&
				   x.Name == y.Name &&
				   x.FullName == y.FullName &&
				   x.TypeKind == y.TypeKind &&
				   x.AllowEmpty == y.AllowEmpty &&
				   x.RequestedLayout == y.RequestedLayout &&
				   x.IsGeneric == y.IsGeneric &&
				   x.IsRef == y.IsRef &&
				   x.FieldNames.SequenceEqual(y.FieldNames) &&
				   x.TypeArgs.SequenceEqual(y.TypeArgs, TypeInfoModelComparer.Instance) &&
				   x.UserFuncs.SequenceEqual(y.UserFuncs) &&
				   x.SerializeOption == y.SerializeOption;
		}
		public int GetHashCode(GenerationModel? obj)
		{
			if (obj == null)
			{
				return 0;
			}
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + (obj.FullName?.GetHashCode() ?? 0);
				hash = hash * 23 + (int)obj.RequestedLayout;
				return hash;
			}
		}
	}

	internal class TypeInfoModelComparer : IEqualityComparer<TypeInfoModel>
	{
		public static readonly TypeInfoModelComparer Instance = new();
		public bool Equals(TypeInfoModel x, TypeInfoModel y)
		{
			return x.FullName == y.FullName && x.IsReferenceType == y.IsReferenceType && x.IsRefStruct == y.IsRefStruct;
		}
		public int GetHashCode(TypeInfoModel obj)
		{
			return obj.FullName?.GetHashCode() ?? 0;
		}
	}

	public enum OneOfLayoutKind : int
	{
		ExplicitUnion = 0,
		Composition = 1,
		Boxing = 2
	}

	[Flags]
	public enum Serialization : int
	{
		None = 0,
		SystemJson = 1,
		NewtonsoftJson = 2,
		MemoryPack = 4
	}
}