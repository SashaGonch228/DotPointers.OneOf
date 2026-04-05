using System;
using System.Collections.Generic;
using System.Text;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(["Content", "NotFound", "Failure"])]
	public readonly partial struct Response<T> : IOneOf<T, Void, Exception>
	{
		public bool HasContent => IsContent;

		public static Response<T> Empty => new(default(Void));
	}
}
