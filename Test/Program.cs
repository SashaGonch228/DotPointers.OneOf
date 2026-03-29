//using DotPointers;
//using MemoryPack;
//using MessagePack;
//using Newtonsoft.Json;
//using System.Runtime.CompilerServices;
//using System.Text.Json;

//namespace DotPointers.Samples;

//[GenerateOneOf]
//public ref partial struct MyStruct<T> : IOneOf<T, string> { }

//[GenerateOneOf(Layout: OneOfLayoutKind.ExplicitUnion, AllowEmpty: false)]
//[GenerateSystemJsonSupport]
//[GenerateNewtonsoftJsonSupport]
//[GenerateMemoryPackSupport]
//public partial class MyStruct2 : IOneOf<long, int, short, byte, string, int> { }

//public static class Program
//{
//	public static void Main()
//	{
//		MyStruct2 a1 = new(10);
//		MyStruct2 b1 = new(20);
//		MyStruct2 c1 = new(20344444444444444L);

//		var str = System.Text.Json.JsonSerializer.Serialize(a1);
//		var aa1 = System.Text.Json.JsonSerializer.Deserialize<MyStruct2>(str);
//		Console.WriteLine($"({str.Length}) {str} => {aa1}");

//		var serializer = new Newtonsoft.Json.JsonSerializer();
//		serializer.Serialize(Console.Out, a1); Console.WriteLine();

//		var bytes = MemoryPackSerializer.Serialize(a1);
//		Console.WriteLine($"({bytes.Length}) {MemoryPackSerializer.Deserialize<MyStruct2>(bytes)}");

//		Console.WriteLine();

//		Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(a1.AsTuple, new JsonSerializerOptions { IncludeFields = true }));
//		new Newtonsoft.Json.JsonSerializer().Serialize(Console.Out, a1.AsTuple); Console.WriteLine();
//		Console.WriteLine(MessagePackSerializer.Serialize(a1.AsTuple));
//		Console.WriteLine(MemoryPackSerializer.Serialize(a1.AsTuple));
//	}
//}