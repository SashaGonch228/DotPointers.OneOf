using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace DotPointers.OneOf.Generator
{
	[Generator]
	public class OneOfGenerator : IIncrementalGenerator
	{
		private const string AttrName = "DotPointers.OneOf.GenerateOneOfAttribute";

		private static readonly SymbolDisplayFormat TypeNameFormat = new(
			typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
			genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			var targets = context.SyntaxProvider.ForAttributeWithMetadataName(
				fullyQualifiedMetadataName: AttrName,
				predicate: static (node, _) => node is TypeDeclarationSyntax,
				transform: static (ctx, _) => GetModel(ctx))
				.Where(static x => x is not null)!;

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
				.Select(static (m, _) => m!.Namespace)
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
				i.Name == "IOneOf" && i.ContainingNamespace.ToDisplayString() == "DotPointers.OneOf");

			var attributeData = allAttributes.FirstOrDefault(a =>
				a.AttributeClass?.ToDisplayString() == AttrName);

			if (interfaceSymbol == null || attributeData == null) { return null; }

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
				fields = arg0.Values.Select(x => x.Value?.ToString() ?? "Unknown").ToImmutableArray();
			}

			if (fields.Length == 0)
			{
				fields = typeArguments.Select((t, i) => i < defaultFields.Length ? defaultFields[i] : t.Name).ToImmutableArray();
			}

			var allowEmpty = (bool)(attributeData.ConstructorArguments.ElementAtOrDefault(1).Value ?? true);
			var requestedLayout = (OneOfLayoutKind)(int)(attributeData.ConstructorArguments.ElementAtOrDefault(2).Value ?? 0);
			var kind = (
				(KindPosition)(attributeData.ConstructorArguments.ElementAtOrDefault(3).Value ?? 0),
				(KindSize)(attributeData.ConstructorArguments.ElementAtOrDefault(4).Value ?? 0)
			);
			var generateMetadata = (bool)(attributeData.ConstructorArguments.ElementAtOrDefault(5).Value ?? true);

			var types = typeArguments.Select(t => new TypeInfoModel(
				FullName: t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
				ShortName: t.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
				IsReferenceType: t.IsReferenceType,
				IsRefStruct: t.IsRefLikeType,
				HasReferences: !t.IsValueType || !t.IsUnmanagedType,
				IsInterface: t.TypeKind == TypeKind.Interface
			)).ToImmutableArray();

			var userMethods = symbol.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(m => m.MethodKind == MethodKind.Ordinary && !m.IsImplicitlyDeclared)
				.Select(m => new UserMethodModel(
					m.Name,
					m.Parameters.Select(p => p.Type.ToDisplayString(TypeNameFormat)).ToImmutableArray().AsEquatableArray()
				))
				.ToImmutableArray();

			return new GenerationModel(
				Namespace: symbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : symbol.ContainingNamespace.ToDisplayString(),
				Name: symbol.Name,
				FullName: symbol.ToDisplayString(TypeNameFormat),
				TypeKind: symbol.IsValueType ? (symbol.IsRefLikeType ? "ref struct" : "struct") : "class",
				AllowEmpty: allowEmpty,
				RequestedLayout: requestedLayout,
				IsGeneric: symbol.IsGenericType,
				IsRef: symbol.IsRefLikeType,
				TypeArgs: types.AsEquatableArray(),
				FieldNames: fields.AsEquatableArray(),
				UserFuncs: userMethods.AsEquatableArray(),
				SerializeOption: serialize,
				Kind: kind,
				GenerateMetadata: generateMetadata
			);
		}
	}

	public record GenerationModel(
		string Namespace,
		string Name,
		string FullName,
		string TypeKind,
		bool AllowEmpty,
		OneOfLayoutKind RequestedLayout,
		bool IsGeneric,
		bool IsRef,
		EquatableArray<TypeInfoModel> TypeArgs,
		EquatableArray<string> FieldNames,
		EquatableArray<UserMethodModel> UserFuncs,
		Serialization SerializeOption,
		(KindPosition Pos, KindSize Size) Kind,
		bool GenerateMetadata
	)
	{
		public string Generics => IsGeneric ? ('<' + FullName.Split('<')[1]) : string.Empty;
	}

	public record TypeInfoModel(string FullName, string ShortName, bool IsReferenceType, bool IsRefStruct, bool HasReferences, bool IsInterface)
	{
		public bool IsVoid => FullName == "global::DotPointers.OneOf.Void";
	}

	public record UserMethodModel(string Name, EquatableArray<string> Parameters);

	public enum OneOfLayoutKind : int
	{
		Auto = 0,
		Hybrid = 1,
		Composition = 2,
		ExplicitUnion = 3,
		Boxing = 4
	}

	[Flags]
	public enum Serialization : int
	{
		None = 0,
		SystemJson = 1,
		NewtonsoftJson = 2,
		MemoryPack = 4
	}

	public enum KindPosition { Before, After }
	public enum KindSize { Byte, Short, Int, Long }

	public readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T> where T : IEquatable<T>
	{
		private readonly ImmutableArray<T> _array;

		public EquatableArray(ImmutableArray<T> array)
		{
			_array = array;
		}

		public bool Equals(EquatableArray<T> other)
		{
			if (_array.Length != other._array.Length) { return false; }
			for (int i = 0; i < _array.Length; i++)
			{
				if (!_array[i].Equals(other._array[i])) { return false; }
			}
			return true;
		}

		public T this[int index] => _array[index];
		public int Length => _array.Length;
		public int IndexOf(T item) => _array.IndexOf(item);

		public override bool Equals(object? obj)
		{
			return obj is EquatableArray<T> other && Equals(other);
		}

		public override int GetHashCode()
		{
			if (_array.IsDefaultOrEmpty) { return 0; }

			unchecked
			{
				int hash = 17;
				foreach (var item in _array)
				{
					hash = (hash * 31) + (item?.GetHashCode() ?? 0);
				}
				return hash;
			}
		}

		public static implicit operator EquatableArray<T>(ImmutableArray<T> array) => new(array);
		public static implicit operator ImmutableArray<T>(EquatableArray<T> array) => array._array;

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)(_array.IsDefault ? Enumerable.Empty<T>() : _array)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public static class EquatableArrayExtensions
	{
		public static EquatableArray<T> AsEquatableArray<T>(this ImmutableArray<T> array) where T : IEquatable<T>
		{
			return new EquatableArray<T>(array);
		}
	}
}

namespace System.Runtime.CompilerServices
{
	internal static class IsExternalInit { }
}