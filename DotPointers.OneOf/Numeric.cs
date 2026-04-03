using System;
using System.Globalization;

namespace DotPointers.OneOf
{
	[GenerateOneOf(new[] { "Number", "Text" }, false)]
	public readonly partial struct Numeric : IOneOf<double, string>
	{
		public bool IsParsed => IsNumber || (IsText && double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _));

		public double GetValue() => Match(
			static n => n,
			static t => double.TryParse(t, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
				? result
				: throw new FormatException($"Cannot parse '{t}' as numeric (double).")
		);

		public bool TryGetValue(out double value)
		{
			if (IsNumber) { value = Number; return true; }
			return double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
		}

		public static implicit operator Numeric(int value) => new(value);
	}

	[GenerateOneOf(new[] { "Number", "Text" }, false)]
	public readonly partial struct IntNumeric : IOneOf<int, string>
	{
		public bool IsParsed => IsNumber || (IsText && int.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _));

		public int GetValue() => Match(
			static n => n,
			static t => int.TryParse(t, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
				? result
				: throw new FormatException($"Cannot parse '{t}' as numeric (double).")
		);

		public bool TryGetValue(out int value)
		{
			if (IsNumber) { value = Number; return true; }
			return int.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
		}
	}
}