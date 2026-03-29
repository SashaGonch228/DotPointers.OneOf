using System.Text.Json;
using DotPointers.OneOf;

namespace Dotpointers.OneOf.Samples;

public record UserProfile(string Name, int Level);
public record AnonymousUser(string SessionId);

[GenerateOneOf]
[GenerateSystemJsonSupport]
public partial struct SessionState : IOneOf<UserProfile, AnonymousUser>;

public static class JsonSerialization
{
	public static void Run()
	{
		// Implicit casting to the OneOf type
		SessionState state = new UserProfile("LeadDev", 99);

		// Serialize to JSON
		string json = JsonSerializer.Serialize(state);
		Console.WriteLine($"Serialized JSON: {json}");

		// Deserialize back
		var restored = JsonSerializer.Deserialize<SessionState>(json);
		Console.WriteLine($"Restored Kind: {restored.Kind}");
	}
}