using System;
using System.Runtime.CompilerServices;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(["Single", "Between", "All", "None"], false)]
	public readonly partial struct Range : IOneOf<int, (int, int), All, None>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool InRange(int value) => Match(
			single => value == single,
			between => value >= between.Item1 && value <= between.Item2,
			all => true,
			none => false
		);
	}

	[GenerateOneOf(["Single", "Between", "All", "None"], false)]
	public readonly partial struct Range<T> : IOneOf<T, (T Min, T Max), All, None> where T : struct, IComparable<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool InRange(T value) => Match(
			single => value.CompareTo(single) == 0,
			between => value.CompareTo(between.Min) >= 0 && value.CompareTo(between.Max) <= 0,
			all => true,
			none => false
		);
	}
}
