using DotPointers.OneOf;

namespace Dotpointers.OneOf.Samples;

// Using multiple strings but with semantic names
[GenerateOneOf(["SourceFile", "DestinationFile", "CompressionLevel"])]
public partial struct ArchiveConfig : IOneOf<string, string, int>;

public static class NamedFields
{
	public static void Run()
	{
		// Must specify Kind when types are ambiguous
		var config = new ArchiveConfig("data.bin", ArchiveConfig.OneOfArchiveConfig.SourceFile);

		config.Switch(
			src => Console.WriteLine($"Source: {src}"),
			dest => Console.WriteLine($"Dest: {dest}"),
			level => Console.WriteLine($"Level: {level}")
		);

		// TryGet example
		if (config.TryGetSourceFile(out var source))
		{
			Console.WriteLine($"Processing source file: {source}");
		}
	}
}