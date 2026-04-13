using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace DotPointers.OneOf
{
	public static class OneOfThrowHelper
	{
		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowInvalid<TTHROW>(int expected, int current) where TTHROW : struct, Enum
		{
			throw new InvalidOperationException($"Cannot access {(TTHROW)Enum.ToObject(typeof(TTHROW), expected)}. Current kind is {(TTHROW)Enum.ToObject(typeof(TTHROW), current)}");
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowEmpty(string typeName)
		{
			throw new InvalidOperationException($"{typeName} is empty or not initialized");
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static TTHROW ThrowEmpty<TTHROW>(string typeName)
		{
			throw new InvalidOperationException($"{typeName} is empty or not initialized");
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowNull(string typeName)
		{
			throw new NullReferenceException($"{typeName} cannot be initialized with null");
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowInvalidType(string typeName)
		{
			throw new NullReferenceException($"{typeName} cannot be initialized with null");
		}
	}
}