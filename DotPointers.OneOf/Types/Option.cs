using System.Runtime.CompilerServices;

namespace DotPointers.OneOf.Types
{
	[GenerateOneOf(["Value"], true)]
	public readonly partial struct Option<T> : IOneOf<T>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T? GetValueOrDefault() => IsValue ? Value : default;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T? GetValueOrDefault(T? defaultValue) => IsValue ? Value : defaultValue;
	}
}
