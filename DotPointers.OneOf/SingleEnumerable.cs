using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DotPointers.OneOf
{
	/// <summary>
	/// Zero-allocation enumerable for a single value.
	/// </summary>
	public readonly struct SingleEnumerable<T> : IEnumerable<T>
	{
		private readonly T _value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SingleEnumerable(T value) => _value = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator() => new(_value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public struct Enumerator : IEnumerator<T>
		{
			private readonly T _value;
			private int _state;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(T value)
			{
				_value = value;
				_state = 0;
			}

			public readonly T Current => _value;
			readonly object? IEnumerator.Current => _value;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (_state == 0)
				{
					_state = 1;
					return true;
				}
				return false;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset() => _state = 0;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public readonly void Dispose() { }
		}
	}
}