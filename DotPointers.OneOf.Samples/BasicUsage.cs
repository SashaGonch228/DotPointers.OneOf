using DotPointers.OneOf;

namespace Dotpointers.OneOf.Samples;

public record Move(int X, int Y);
public record Attack(string Target, int Damage);

[GenerateOneOf([nameof(Move), nameof(Attack), "Idle"], AllowEmpty: false, OneOfLayoutKind.Boxing)]
public partial struct PlayerAction : IOneOf<Move, Attack, DotPointers.OneOf.Void>; // Using Void (enpty struct)

public static class BasicUsage
{
	public static void Run()
	{
		// Implicit casting to the OneOf type
		PlayerAction action = new Attack("Goblin", 15);

		// Using Switch for logic branching
		action.Switch(
			move => Console.WriteLine($"Moving to {move.X}, {move.Y}"),
			attack => Console.WriteLine($"Attacking {attack.Target} for {attack.Damage} HP"),
			idle => Console.WriteLine("Doing nothing...")
		);

		// Direct property access
		if (action.IsAttack)
		{
			Console.WriteLine($"Current target: {action.Attack.Target}");
		}
	}
}