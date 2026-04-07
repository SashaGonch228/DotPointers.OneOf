using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using OneOf;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Order;

namespace Benchmarks;

public static class Program
{
	public static void Main()
	{
		var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator);
		BenchmarkRunner.Run<OneOfProductionBenchmark>(config);
	}
}

[HideColumns("Error", "StdDev", "Median", "RatioSD")]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[DisassemblyDiagnoser(maxDepth: 3)]
public class OneOfProductionBenchmark
{
	private const int Count = 100_000;

	private HeavyIdExplicit[] _explicitArray = null!;
	private HeavyIdComposition[] _compositionArray = null!;
	private HeavyIdBoxing[] _boxingArray = null!;
	private OneOf<Guid, long, int>[] _oneOfArray = null!;

	private Guid _testGuid = Guid.NewGuid();
	private long _testLong = 42L;
	private int _testInt = 123;

	[GlobalSetup]
	public void Setup()
	{
		_explicitArray = new HeavyIdExplicit[Count];
		_compositionArray = new HeavyIdComposition[Count];
		_boxingArray = new HeavyIdBoxing[Count];
		_oneOfArray = new OneOf<Guid, long, int>[Count];

		Console.WriteLine($"[Metadata] Size of Explicit: {Unsafe.SizeOf<HeavyIdExplicit>()} bytes");
		Console.WriteLine($"[Metadata] Size of Composition: {Unsafe.SizeOf<HeavyIdComposition>()} bytes");
		Console.WriteLine($"[Metadata] Size of OneOf: {Unsafe.SizeOf<OneOf<Guid, long, int>>()} bytes");

		var rnd = new Random(42);
		for (int i = 0; i < Count; i++)
		{
			int type = rnd.Next(0, 3);
			if (type == 0)
			{
				_explicitArray[i] = _testGuid;
				_compositionArray[i] = _testGuid;
				_boxingArray[i] = _testGuid;
				_oneOfArray[i] = _testGuid;
			}
			else if (type == 1)
			{
				_explicitArray[i] = _testLong;
				_compositionArray[i] = _testLong;
				_boxingArray[i] = _testLong;
				_oneOfArray[i] = _testLong;
			}
			else
			{
				_explicitArray[i] = _testInt;
				_compositionArray[i] = _testInt;
				_boxingArray[i] = _testInt;
				_oneOfArray[i] = _testInt;
			}
		}
	}

	[Benchmark(Baseline = true)] [BenchmarkCategory("Access")]
	public long Match_OneOf_Massive()
	{
		long sum = 0;
		for (int i = 0; i < _oneOfArray.Length; i++)
		{
			OneOf<Guid, long, int> item = _oneOfArray[i];
			sum += item.Match(g => g.GetHashCode(), l => (int)(l % 1000), i => i);
		}

		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Access")]
	public long Match_Explicit_Massive()
	{
		long sum = 0;
		for (int i = 0; i < _explicitArray.Length; i++)
		{
			HeavyIdExplicit item = _explicitArray[i];
			sum += item.Match(g => g.GetHashCode(), l => (int)(l % 1000), i => i);
		}

		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Access")]
	public long Match_Composition_Massive()
	{
		long sum = 0;
		for (int i = 0; i < _compositionArray.Length; i++)
		{
			HeavyIdComposition item = _compositionArray[i];
			sum += item.Match(g => g.GetHashCode(), l => (int)(l % 1000), i => i);
		}

		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Access")]
	public long Match_Boxing_Massive()
	{
		long sum = 0;
		for (int i = 0; i < _boxingArray.Length; i++)
		{
			HeavyIdBoxing item = _boxingArray[i];
			sum += item.Match(g => g.GetHashCode(), l => (int)(l % 1000), i => i);
		}

		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Access")]
	public long DirectSwitch_Explicit_Massive()
	{
		long sum = 0;
		for (int i = 0; i < _explicitArray.Length; i++)
		{
			HeavyIdExplicit item = _explicitArray[i];
			sum += item.Index switch
			{
				0 => item.First.GetHashCode(),
				1 => (int)(item.Second % 1000),
				2 => item.Third,
				_ => 0
			};
		}
		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Creation")]
	public void Create_OneOf()
	{
		var arr = new OneOf<Guid, long, int>[100];
		for (int i = 0; i < 100; i++) arr[i] = _testGuid;
	}

	[Benchmark]
	[BenchmarkCategory("Creation")]
	public void Create_Explicit()
	{
		var arr = new HeavyIdExplicit[100];
		for (int i = 0; i < 100; i++) arr[i] = _testGuid;
	}

	[Benchmark]
	[BenchmarkCategory("Creation")]
	public void Create_Boxing()
	{
		var arr = new HeavyIdBoxing[100];
		for (int i = 0; i < 100; i++) arr[i] = _testGuid;
	}
}

[ShortRunJob]
[MemoryDiagnoser]
[DisassemblyDiagnoser(printSource: true)]
public class HeavyUnionAccessBenchmark
{
	private readonly Guid _guid = Guid.NewGuid();
	private readonly string _string = "42";

	private HeavyIdExplicit _exp;
	private OneOf<Guid, long, int> _oneOf;

	private HeavyIdExplicitC _expMixed;
	private HeavyIdBoxingC _boxMixed;

	[GlobalSetup]
	public void Setup()
	{
		_exp = new HeavyIdExplicit(_guid);
		_oneOf = OneOf<Guid, long, int>.FromT0(_guid);

		_expMixed = new HeavyIdExplicitC(_string);
		_boxMixed = new HeavyIdBoxingC(_string);
	}

	[Benchmark(Baseline = true)]
	public bool IsFirst_OneOf() => _oneOf.IsT0;

	[Benchmark]
	public bool IsFirst_Explicit() => _exp.IsFirst;

	[Benchmark]
	public bool IsFourth_Explicit_Mixed() => _expMixed.IsFourth;

	[Benchmark]
	public bool IsFourth_Boxing_Mixed() => _boxMixed.IsFourth;


	[Benchmark]
	public Guid GetValue_OneOf() => _oneOf.AsT0;

	[Benchmark]
	public Guid GetValue_Explicit() => _exp.First;

	[Benchmark]
	public string GetValue_Explicit_Mixed() => _expMixed.Fourth;

	[Benchmark]
	public string GetValue_Boxing_Mixed() => _boxMixed.Fourth;

	[Benchmark]
	public int Match_OneOf()
		=> _oneOf.Match(g => g.GetHashCode(), l => (int)l, i => i);

	[Benchmark]
	public int Match_Explicit()
		=> _exp.Match(g => g.GetHashCode(), l => (int)l, i => i);

	[Benchmark]
	public int Match_Boxing_Mixed()
		=> _boxMixed.Match(g => g.GetHashCode(), l => (int)l, i => i, s => s.Length);
}

[ShortRunJob]
[MemoryDiagnoser]
[DisassemblyDiagnoser(printSource: true)]
public class HeavyUnionBenchmark
{
	private readonly Guid _guid = Guid.NewGuid();
	private readonly string _string = "42";

	[Benchmark(Baseline = true)]
	public OneOf<Guid, long, int> OneOf_Pure_Create() => OneOf<Guid, long, int>.FromT0(_guid);

	[Benchmark]
	public HeavyIdExplicit Explicit_Pure_Create() => new HeavyIdExplicit(_guid);

	[Benchmark]
	public HeavyIdComposition Composition_Pure_Create() => new HeavyIdComposition(_guid);

	[Benchmark]
	public HeavyIdBoxing Boxing_Pure_Create() => new HeavyIdBoxing(_guid);

	[Benchmark]
	public OneOf<Guid, long, int, string> OneOf_Mixed_Create() => OneOf<Guid, long, int, string>.FromT3(_string);

	[Benchmark]
	public HeavyIdExplicitC Explicit_Mixed_Create() => new HeavyIdExplicitC(_string);

	[Benchmark]
	public HeavyIdCompositionC Composition_Mixed_Create() => new HeavyIdCompositionC(_string);

	[Benchmark]
	public HeavyIdBoxingC Boxing_Mixed_Create() => new HeavyIdBoxingC(_string);
}

[DotPointers.OneOf.GenerateOneOf(Layout: DotPointers.OneOf.OneOfLayoutKind.ExplicitUnion)]
public readonly partial struct HeavyIdExplicit : DotPointers.OneOf.IOneOf<Guid, long, int>;

[DotPointers.OneOf.GenerateOneOf(Layout: DotPointers.OneOf.OneOfLayoutKind.Composition)]
public readonly partial struct HeavyIdComposition : DotPointers.OneOf.IOneOf<Guid, long, int>;

[DotPointers.OneOf.GenerateOneOf(Layout: DotPointers.OneOf.OneOfLayoutKind.Boxing)]
public readonly partial struct HeavyIdBoxing : DotPointers.OneOf.IOneOf<Guid, long, int>;


[DotPointers.OneOf.GenerateOneOf(Layout: DotPointers.OneOf.OneOfLayoutKind.ExplicitUnion)]
public readonly partial struct HeavyIdExplicitC : DotPointers.OneOf.IOneOf<Guid, long, int, string>;

[DotPointers.OneOf.GenerateOneOf(Layout: DotPointers.OneOf.OneOfLayoutKind.Composition)]
public readonly partial struct HeavyIdCompositionC : DotPointers.OneOf.IOneOf<Guid, long, int, string>;

[DotPointers.OneOf.GenerateOneOf(Layout: DotPointers.OneOf.OneOfLayoutKind.Boxing)]
public readonly partial struct HeavyIdBoxingC : DotPointers.OneOf.IOneOf<Guid, long, int, string>;