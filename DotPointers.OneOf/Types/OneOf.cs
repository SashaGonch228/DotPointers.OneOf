using System.Collections.Generic;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(new string[] { "Left", "Right" }, false)]
	public readonly partial struct OneOf<TL, TR> : IOneOf<TL, TR>
	{
		public OneOfBox<TR, TL> Swap() => IsLeft ? Left : Right;
	}

	[GenerateOneOf(new string[] { "Left", "Middle", "Right" }, false)]
	public readonly partial struct OneOf<TL, TM, TR> : IOneOf<TL, TM, TR>;

	[GenerateOneOf(null, false)]
	public readonly partial struct OneOf<T0, T1, T2, T3> : IOneOf<T0, T1, T2, T3>;

	[GenerateOneOf(null, false)]
	public readonly partial struct OneOf<T0, T1, T2, T3, T4> : IOneOf<T0, T1, T2, T3, T4>;

	[GenerateOneOf(null, false)]
	public readonly partial struct OneOf<T0, T1, T2, T3, T4, T5> : IOneOf<T0, T1, T2, T3, T4, T5>;

	[GenerateOneOf(null, false)]
	public readonly partial struct OneOf<T0, T1, T2, T3, T4, T5, T6> : IOneOf<T0, T1, T2, T3, T4, T5, T6>;

	[GenerateOneOf(null, false)]
	public readonly partial struct OneOf<T0, T1, T2, T3, T4, T5, T6, T7> : IOneOf<T0, T1, T2, T3, T4, T5, T6, T7>;


	[GenerateOneOf(new string[] { "Left", "Right" }, false, OneOfLayoutKind.Boxing)]
	public readonly partial struct OneOfBox<TL, TR> : IOneOf<TL, TR>
	{
		public OneOfBox<TR, TL> Swap() => IsLeft ? Left : Right;
	}
}
