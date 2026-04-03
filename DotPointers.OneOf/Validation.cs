using System.Collections.Generic;
using System.Linq;

namespace DotPointers.OneOf
{
	[GenerateOneOf(new[] { "Value", "Failures" }, false)]
	public readonly partial struct Validation<T, E> : IOneOf<T, E[]>
	{
		public static Validation<T, E> Success(T value) => new(value);
		public static Validation<T, E> Fail(IEnumerable<E> errors) => new(errors.ToArray());
	}
}