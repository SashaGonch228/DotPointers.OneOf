using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(["Value", "Failures"], false)]
	public readonly partial struct Validation<T, E> : IOneOf<T, E[]>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Validation<T, E> Success(T value) => new(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Validation<T, E> Fail(IEnumerable<E> errors) => new(errors.ToArray());
	}
}