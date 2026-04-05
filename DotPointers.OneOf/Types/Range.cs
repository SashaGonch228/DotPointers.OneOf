using System;
using System.Runtime.CompilerServices;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(["Single", "Between", "Any", "None"], false)]
	public readonly partial struct Range : IOneOf<int, (int, int), Void, Void>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool InRange(int value) => Match(
			s => value == s,
			b => value >= b.Item1 && value <= b.Item2,
			static _ => true,
			static _ => false
		);
	}

	[GenerateOneOf(["Single", "Between", "Any", "None"], false)]
	public readonly partial struct Range<T> : IOneOf<T, (T Min, T Max), Void, Void> where T : struct, IComparable<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool InRange(T value) => Match(
			s => value.CompareTo(s) == 0,
			b => value.CompareTo(b.Min) >= 0 && value.CompareTo(b.Max) <= 0,
			static _ => true,
			static _ => false
		);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Range<T> SingleValue(T value) => new(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Range<T> BetweenValue(T min, T max) => new((min, max));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Range<T> AnyValue() => new(default(Void), OneOfRange.Any);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Range<T> NoneValue() => new(default(Void), OneOfRange.None);
	}
}
