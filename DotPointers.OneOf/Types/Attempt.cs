using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(["Value", "Task"])]
	public readonly partial struct Attempt<T> : IOneOf<T, Task<T>>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<T> GetValueAsync() => IsValue ? ValueForce : await TaskForce;
	}
}
