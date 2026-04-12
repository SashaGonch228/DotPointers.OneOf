using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(["True", "False", "Unknown"])]
	public readonly partial struct Tristate : IOneOf<Void, Void, Void>
	{
		public bool? Bool => Match(T => (bool?)true, F => (bool?)false, N => (bool?)null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Tristate(bool value) => value ? TrueValue() : FalseValue();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Tristate TrueValue() => new(default, OneOfTristate.True);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Tristate FalseValue() => new(default, OneOfTristate.False);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Tristate UnknownValue() => new(default, OneOfTristate.Unknown);
	}
}
