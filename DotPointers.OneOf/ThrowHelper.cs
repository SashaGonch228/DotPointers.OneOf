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
		public static void ThrowInvalid<T>(int expected, int current) where T : struct, Enum
		{
			throw new InvalidOperationException($"Cannot access {(T)(object)expected}. Current kind is {(T)(object)current}");
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowEmpty(string typeName)
		{
			throw new InvalidOperationException($"{typeName} is empty or not initialized");
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static T ThrowEmpty<T>(string typeName)
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