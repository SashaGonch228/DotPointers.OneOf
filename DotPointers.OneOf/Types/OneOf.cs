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

	[GenerateOneOf(new string[] { "Left", "Right" }, false, OneOfLayoutKind.ExplicitUnion)]
	public readonly partial struct OneOfStruct<TL, TR> : IOneOf<TL, TR>
		where TL : unmanaged where TR : unmanaged
	{
		public OneOfStruct<TR, TL> Swap() => IsLeft ? Left : Right;
	}

	[GenerateOneOf(new string[] { "Left", "Middle", "Right" }, false, OneOfLayoutKind.ExplicitUnion)]
	public readonly partial struct OneOfStruct<TL, TM, TR> : IOneOf<TL, TM, TR>
		where TL : unmanaged where TM : unmanaged where TR : unmanaged;

	[GenerateOneOf(null, false, OneOfLayoutKind.ExplicitUnion)]
	public readonly partial struct OneOfStruct<T0, T1, T2, T3> : IOneOf<T0, T1, T2, T3>
		where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged;

	[GenerateOneOf(null, false, OneOfLayoutKind.ExplicitUnion)]
	public readonly partial struct OneOfStruct<T0, T1, T2, T3, T4> : IOneOf<T0, T1, T2, T3, T4>
		where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
		where T4 : unmanaged;

	[GenerateOneOf(null, false, OneOfLayoutKind.ExplicitUnion)]
	public readonly partial struct OneOfStruct<T0, T1, T2, T3, T4, T5> : IOneOf<T0, T1, T2, T3, T4, T5>
		where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
		where T4 : unmanaged where T5 : unmanaged;

	[GenerateOneOf(null, false, OneOfLayoutKind.ExplicitUnion)]
	public readonly partial struct OneOfStruct<T0, T1, T2, T3, T4, T5, T6> : IOneOf<T0, T1, T2, T3, T4, T5, T6>
		where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
		where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged;

	[GenerateOneOf(null, false, OneOfLayoutKind.ExplicitUnion)]
	public readonly partial struct OneOfStruct<T0, T1, T2, T3, T4, T5, T6, T7> : IOneOf<T0, T1, T2, T3, T4, T5, T6, T7>
		where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
		where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged where T7 : unmanaged;
}
