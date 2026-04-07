using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Order;
using OneOf;
using System;
using System.Runtime.CompilerServices;

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
	private const int CreateBatchSize = 100;

	private HeavyIdExplicit[] _explicitArray = null!;
	private HeavyIdComposition[] _compositionArray = null!;
	private HeavyIdBoxing[] _boxingArray = null!;
	private OneOf<Guid, long, int>[] _oneOfArray = null!;

	private readonly Guid _testGuid = Guid.NewGuid();
	private readonly long _testLong = 42L;
	private readonly int _testInt = 123;

	[GlobalSetup]
	public void Setup()
	{
		_explicitArray = new HeavyIdExplicit[Count];
		_compositionArray = new HeavyIdComposition[Count];
		_boxingArray = new HeavyIdBoxing[Count];
		_oneOfArray = new OneOf<Guid, long, int>[Count];

		var rnd = new Random(42);
		for (int i = 0; i < Count; i++)
		{
			int type = rnd.Next(0, 3);
			switch (type)
			{
				case 0:
					_explicitArray[i] = _testGuid;
					_compositionArray[i] = _testGuid;
					_boxingArray[i] = _testGuid;
					_oneOfArray[i] = _testGuid;
					break;
				case 1:
					_explicitArray[i] = _testLong;
					_compositionArray[i] = _testLong;
					_boxingArray[i] = _testLong;
					_oneOfArray[i] = _testLong;
					break;
				case 2:
					_explicitArray[i] = _testInt;
					_compositionArray[i] = _testInt;
					_boxingArray[i] = _testInt;
					_oneOfArray[i] = _testInt;
					break;
			}
		}
	}

	[Benchmark(Baseline = true)]
	[BenchmarkCategory("Access")]
	public long Match_OneOf_Massive()
	{
		long sum = 0;
		var source = _oneOfArray;
		for (int i = 0; i < source.Length; i++)
		{
			sum += source[i].Match(g => g.GetHashCode(), l => (int)(l % 1000), val => val);
		}
		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Access")]
	public long Match_Explicit_Massive()
	{
		long sum = 0;
		var source = _explicitArray;
		for (int i = 0; i < source.Length; i++)
		{
			sum += source[i].Match(g => g.GetHashCode(), l => (int)(l % 1000), val => val);
		}
		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Access")]
	public long DirectSwitch_Explicit_Massive()
	{
		long sum = 0;
		var source = _explicitArray;
		for (int i = 0; i < source.Length; i++)
		{
			ref readonly var item = ref source[i];
			sum += item.Kind switch
			{
				HeavyIdExplicit.OneOfHeavyIdExplicit.First => item.First.GetHashCode(),
				HeavyIdExplicit.OneOfHeavyIdExplicit.Second => (int)(item.Second % 1000),
				HeavyIdExplicit.OneOfHeavyIdExplicit.Third => item.Third,
				_ => 0
			};
		}
		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Access")]
	public long Match_Composition_Massive()
	{
		long sum = 0;
		var source = _compositionArray;
		for (int i = 0; i < source.Length; i++)
		{
			sum += source[i].Match(g => g.GetHashCode(), l => (int)(l % 1000), val => val);
		}
		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Access")]
	public long Match_Boxing_Massive()
	{
		long sum = 0;
		var source = _boxingArray;
		for (int i = 0; i < source.Length; i++)
		{
			sum += source[i].Match(g => g.GetHashCode(), l => (int)(l % 1000), val => val);
		}
		return sum;
	}

	[Benchmark]
	[BenchmarkCategory("Creation")]
	public OneOf<Guid, long, int>[] Create_OneOf()
	{
		var arr = new OneOf<Guid, long, int>[CreateBatchSize];
		for (int i = 0; i < CreateBatchSize; i++)
			arr[i] = _testGuid;
		return arr;
	}

	[Benchmark]
	[BenchmarkCategory("Creation")]
	public HeavyIdExplicit[] Create_Explicit()
	{
		var arr = new HeavyIdExplicit[CreateBatchSize];
		for (int i = 0; i < CreateBatchSize; i++)
			arr[i] = _testGuid;
		return arr;
	}

	[Benchmark]
	[BenchmarkCategory("Creation")]
	public HeavyIdBoxing[] Create_Boxing()
	{
		var arr = new HeavyIdBoxing[CreateBatchSize];
		for (int i = 0; i < CreateBatchSize; i++)
			arr[i] = _testGuid;
		return arr;
	}

	[Benchmark]
	[BenchmarkCategory("Composition")]
	public HeavyIdComposition[] Create_Composition()
	{
		var arr = new HeavyIdComposition[CreateBatchSize];
		for (int i = 0; i < CreateBatchSize; i++)
			arr[i] = _testGuid;
		return arr;
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