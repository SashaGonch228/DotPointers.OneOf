namespace DotPointers.OneOf
{
	[GenerateOneOf(new[] { "Value" }, true, OneOfLayoutKind.Composition)]
	public readonly partial struct Option<T> : IOneOf<T>
	{
		public T? GetValueOrDefault() => IsValue ? Value : default;
		public T? GetValueOrDefault(T? defaultValue) => IsValue ? Value : defaultValue;
	}
}
