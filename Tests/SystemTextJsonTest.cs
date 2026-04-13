using DotPointers.OneOf;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Tests
{
	public class SystemTextJsonTest
	{
		[Fact]
		public void Test()
		{
			Struct3 a1 = 42;
			Struct3 a2 = "42";

			var s1 = JsonSerializer.Serialize(a1);
			var s2 = JsonSerializer.Serialize(a2);

			Assert.Equal(a1, JsonSerializer.Deserialize<Struct3>(s1));
			Assert.Equal(a2, JsonSerializer.Deserialize<Struct3>(s2));
		}
	}

	[GenerateOneOf, GenerateSystemJson]
	public readonly partial struct Struct3 : IOneOf<int, string>;
}
