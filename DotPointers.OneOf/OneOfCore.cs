using System;

namespace DotPointers.OneOf
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class GenerateOneOfAttribute : Attribute
	{
		public GenerateOneOfAttribute() { }
#pragma warning disable RCS1163
#pragma warning disable IDE0060
		public GenerateOneOfAttribute(string[]? FieldNames = null, bool AllowEmpty = false, OneOfLayoutKind Layout = OneOfLayoutKind.Auto, KindPosition KindPos = KindPosition.Before, KindSize KindSize = KindSize.Int) { }
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

	public interface IOneOf
	{
		public object? BoxValue { get; }
		public int Index { get; }
	}

	public interface IOneOf<T0> : IOneOf { }
	public interface IOneOf<T0, T1> : IOneOf { }
	public interface IOneOf<T0, T1, T2> : IOneOf { }
	public interface IOneOf<T0, T1, T2, T3> : IOneOf { }
	public interface IOneOf<T0, T1, T2, T3, T4> : IOneOf { }
	public interface IOneOf<T0, T1, T2, T3, T4, T5> : IOneOf { }
	public interface IOneOf<T0, T1, T2, T3, T4, T5, T6> : IOneOf { }
	public interface IOneOf<T0, T1, T2, T3, T4, T5, T6, T7> : IOneOf { }

	public enum OneOfLayoutKind : int
	{
		Auto = 0,
		Hybrid = 1,
		Composition = 2,
		ExplicitUnion = 3,
		Boxing = 4
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

	public enum KindPosition { Before, After }
	public enum KindSize { Byte, Short, Int, Long }
}