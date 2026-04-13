using DotPointers.OneOf;
using System;
using System.Collections.Generic;
using System.Text;
using MemoryPack;

namespace Tests
{
	public class MemoryPackTest
	{
		[Fact]
		public void Test()
		{
			Struct1 a1 = 42;
			Struct1 a2 = "42";
			Struct2<double> a3 = 3.1415;

			var s1 = MemoryPackSerializer.Serialize(a1);
			var s2 = MemoryPackSerializer.Serialize(a2);
			var s3 = MemoryPackSerializer.Serialize(a3);

			Assert.Equal(a1, MemoryPackSerializer.Deserialize<Struct1>(s1));
			Assert.Equal(a2, MemoryPackSerializer.Deserialize<Struct1>(s2));
			Assert.Equal(a3, MemoryPackSerializer.Deserialize<Struct2<double>>(s3));
		}
	}

	[GenerateOneOf, GenerateMemoryPack]
	public readonly partial struct Struct1 : IOneOf<int, string>;

	[GenerateOneOf, GenerateMemoryPack]
	public readonly partial struct Struct2<T> : IOneOf<T, string>;
}
