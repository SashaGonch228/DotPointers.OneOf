using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using OneOf;

namespace Benchmarks;

public static class Program
{
	public static void Main()
	{
		var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator);
		BenchmarkRunner.Run<HeavyUnionAccessBenchmark>(config);
		//BenchmarkRunner.Run<HeavyUnionBenchmark>(config);
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