using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(["Single", "Multiple", "None"])]
	public readonly partial struct Many<T> : IOneOf<T, IEnumerable<T>, Void>, IEnumerable<T>
	{
		public readonly T? FirstOrDefault => IsSingle ? SingleForce : (IsMultiple ? MultipleForce.FirstOrDefault() : default);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly IEnumerator<T> GetEnumerator()
		{
			if (IsSingle) { return new SingleEnumerable<T>(SingleForce).GetEnumerator(); }
			if (IsMultiple) { return MultipleForce.GetEnumerator(); }
			return Enumerable.Empty<T>().GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Many<T> From(T value) => new(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Many<T> From(IEnumerable<T> values) => new(values);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Many<T> Empty() => new(default(Void));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Many<T>(T[] values) => new(values);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Many<T>(List<T> values) => new(values);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly T[] ToArray()
		{
			if (IsSingle) { return [SingleForce]; }
			if (IsMultiple) { return MultipleForce.ToArray(); }
			return Array.Empty<T>();
		}
	}

	[GenerateOneOf(["Single", "Multiple"])]
	public readonly partial struct OneOrMany<T> : IOneOf<T, IEnumerable<T>>, IEnumerable<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetEnumerator()
		{
			if (IsSingle) return new SingleEnumerable<T>(SingleForce).GetEnumerator();
			return MultipleForce.GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] ToArray() => IsSingle ? [SingleForce] : MultipleForce.ToArray();
	}
}