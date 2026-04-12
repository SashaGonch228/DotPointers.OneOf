namespace DotPointers.OneOf
{
	/// <summary>
	/// Represents the absence of a value. A classic marker for compatibility with void-like logic.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Void;

	/// <summary>
	/// A type with a single value. Used to indicate the completion of an operation that returns no data.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Unit;

	/// <summary>
	/// A semantic marker representing a selection of "all" elements or an unbounded range.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct All;

	/// <summary>
	/// Signals that an error has occurred. Used when the fact of failure is more important than its cause.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Error;

	/// <summary>
	/// A generic-based error marker. Allows categorizing failures at the type system level.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Error<T>;

	/// <summary>
	/// A semantic representation of a logical "False". Makes Match expressions more readable than using raw booleans.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct False;

	/// <summary>
	/// Indicates the potential presence of a value. Often used as a base marker for Option-like logic.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Maybe;

	/// <summary>
	/// A marker for a negative response, refusal, or "No" state.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct No;

	/// <summary>
	/// Represents the total absence of data. A core component of the Option pattern.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct None;

	/// <summary>
	/// A specialized marker for cases where a request is valid, but the target object was not found.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct NotFound;

	/// <summary>
	/// Indicates the presence of specific data within the context of optional types.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Some;

	/// <summary>
	/// Signals the successful completion of an operation without returning any additional data.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Success;

	/// <summary>
	/// A generic-based success marker. Useful for logically separating successful outcomes of different types.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Success<T>;

	/// <summary>
	/// A semantic representation of a logical "True". Makes Match branching self-documenting.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct True;

	/// <summary>
	/// Represents an indeterminate state or a default value that hasn't been initialized yet.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Unknown;

	/// <summary>
	/// A marker for an affirmative response, consent, or "Yes" state.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct Yes;

	/// <summary>
	/// Represents the "I Don't Know" state. Used for cases where the system is uncertain or logic reaches an impasse.
	/// <para><i>Note: This is a zero size type and does not occupy space in the union's memory.</i></para>
	/// </summary>
	[VoidType]
	public readonly struct IDK;
}