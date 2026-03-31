using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using OneOf;
using System;
using static Benchmarks.HeavyIdExplicit;

namespace Benchmarks;

public static class Program
{
	public static void Main()
	{
		var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator);
		BenchmarkRunner.Run<OneOfProductionBenchmark>(config);
		//BenchmarkRunner.Run<HeavyUnionAccessBenchmark>(config);
		//BenchmarkRunner.Run<HeavyUnionBenchmark>(config); 
	}
}

[HideColumns("Error", "StdDev", "Median", "RatioSD")]
[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 3)]
public class OneOfProductionBenchmark
{
	private const int Count = 100_000;
	private HeavyIdExplicit[] _explicitArray;
	private HeavyIdComposition[] _compositionArray;
	private OneOf<Guid, long, int>[] _oneOfArray;

	[GlobalSetup]
	public void Setup()
	{
		_explicitArray = new HeavyIdExplicit[Count];
		_compositionArray = new HeavyIdComposition[Count];
		_oneOfArray = new OneOf<Guid, long, int>[Count];

		var rnd = new Random(42);
		for (int i = 0; i < Count; i++)
		{
			int type = rnd.Next(0, 3);
			if (type == 0)
			{
				var g = Guid.NewGuid();
				_explicitArray[i] = g;
				_compositionArray[i] = g;
				_oneOfArray[i] = g;
			}
			else if (type == 1)
			{
				long l = rnd.NextInt64();
				_explicitArray[i] = l;
				_compositionArray[i] = l;
				_oneOfArray[i] = l;
			}
			else
			{
				int val = rnd.Next();
				_explicitArray[i] = val;
				_compositionArray[i] = val;
				_oneOfArray[i] = val;
			}
		}
	}

	[Benchmark(Baseline = true)]
	public long Match_OneOf_Massive()
	{
		long sum = 0;
		for (int i = 0; i < _oneOfArray.Length; i++)
		{
			OneOf<Guid, long, int> item = _oneOfArray[i];
			sum += item.Match(
				g => g.GetHashCode(),
				l => (int)(l % 1000),
				i => i
			);
		}
		return sum;
	}

	[Benchmark]
	public long Match_Explicit_Massive()
	{
		long sum = 0;
		for (int i = 0; i < _explicitArray.Length; i++)
		{
			HeavyIdExplicit item = _explicitArray[i];
			sum += item.Match(
				g => g.GetHashCode(),
				l => (int)(l % 1000),
				i => i
			);
		}
		return sum;
	}

	[Benchmark]
	public long Match_Composition_Massive()
	{
		long sum = 0;
		for (int i = 0; i < _compositionArray.Length; i++)
		{
			HeavyIdComposition item = _compositionArray[i];
			sum += item.Match(
				g => g.GetHashCode(),
				l => (int)(l % 1000),
				i => i
			);
		}
		return sum;
	}

	[Benchmark]
	public long DirectSwitch_Explicit_Massive()
	{
		long sum = 0;
		for (int i = 0; i < _explicitArray.Length; i++)
		{
			HeavyIdExplicit item = _explicitArray[i];
			sum += item.Kind switch
			{
				OneOfHeavyIdExplicit.First => item.First.GetHashCode(),
				OneOfHeavyIdExplicit.Second => (int)(item.Second % 1000),
				OneOfHeavyIdExplicit.Third => item.Third,
				_ => 0
			};
		}
		return sum;
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