using System;

namespace DotPointers.OneOf
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class GenerateOneOfAttribute : Attribute
	{
		public GenerateOneOfAttribute() { }
#pragma warning disable RCS1163
#pragma warning disable IDE0060
		public GenerateOneOfAttribute(string[]? FieldNames = null, bool AllowEmpty = false, OneOfLayoutKind Layout = OneOfLayoutKind.ExplicitUnion) { }
#pragma warning restore IDE0060
#pragma warning restore RCS1163
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class GenerateSystemJsonSupportAttribute : Attribute
	{
		public GenerateSystemJsonSupportAttribute() { }
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class GenerateNewtonsoftJsonSupportAttribute : Attribute
	{
		public GenerateNewtonsoftJsonSupportAttribute() { }
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class GenerateEfCoreSupportAttribute : Attribute
	{
		public GenerateEfCoreSupportAttribute() { }
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class GenerateMemoryPackSupportAttribute : Attribute
	{
		public GenerateMemoryPackSupportAttribute() { }
	}

	public interface IOneOf<T0> { }
	public interface IOneOf<T0, T1> { }
	public interface IOneOf<T0, T1, T2> { }
	public interface IOneOf<T0, T1, T2, T3> { }
	public interface IOneOf<T0, T1, T2, T3, T4> { }
	public interface IOneOf<T0, T1, T2, T3, T4, T5> { }
	public interface IOneOf<T0, T1, T2, T3, T4, T5, T6> { }
	public interface IOneOf<T0, T1, T2, T3, T4, T5, T6, T7> { }

	public enum OneOfLayoutKind : int
	{
		ExplicitUnion = 0,
		Composition = 1,
		Boxing = 2
	}

	public enum OneOfKind : int
	{
		Empty = 0,
		First = 1,
		Second = 2,
		Third = 3,
		Fourth = 4,
		Fifth = 5,
		Sixth = 6,
		Seventh = 7,
		Eighth = 8
	}
}