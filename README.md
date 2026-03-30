# DotPointers.OneOf

DotPointers.OneOf is a **high-performance** Source Generator for C# designed to create **memory-efficient** Discriminated Unions.

By leveraging modern C# features and low-level memory management, it provides a powerful alternative to traditional polymorphism and manual type checking, ensuring maximum performance with zero runtime overhead.

### 💎 Key Features

- Zero-Allocation: Utilizes LayoutKind.Explicit to minimize memory footprint by overlaying fields in memory.

- Compile-Time Safety: Generates all boilerplate code during compilation, providing instant feedback in your IDE.

- Performance-First: Uses Unsafe and MethodImplOptions.AggressiveInlining to ensure the generated code is as fast as hand-written logic.

- Comprehensive API: Automatically generates Match, Switch, TryPick, Map, and Async methods (ValueTask support).

- Serialization API: You can genetate System.Text.Json, Newtonsoft.Json and MemoryPack serializators via just add attributes 

### 🚀 Quick Start

Define your union by implementing the IOneOf interface and marking the partial struct with the [GenerateOneOf] attribute.

**Definition**
```csharp
// Choose field names and layout kind
[GenerateOneOf(["Single", "Between", "Any", "None"], layout: OneOfLayoutKind.ExplicitUnion)]
public readonly partial struct Range : IOneOf<int, (int, int), Void, Void>
{
    // Extend your union with custom domain logic
    public bool InRange(int value) => Match(
        s => value == s,
        b => value >= b.Item1 && value <= b.Item2,
        static _ => true,
        static _ => false
    );
}
```


**Usage**

```csharp
// Implicit conversion from underlying types
Range range = (10, 20); 

range.Switch(
    s => Console.WriteLine($"Point: {s}"),
    b => Console.WriteLine($"Interval: {b.Item1} to {b.Item2}"),
    _ => Console.WriteLine("Matches everything"),
    _ => Console.WriteLine("Empty range")
);
```

*More samples in DotPointers.OneOf.Samples*


### 🛠 Layout Strategies

You can control how the data is stored in memory via OneOfLayoutKind. Choosing the right strategy is key for performance and memory usage.

#### 1. Explicit *(max(sizeof(TValue)) + sizeof(IntPtr) + 4)*

The most efficient way for value types. All value types are overlaid in a single Explicit struct (sharing the same memory space), while reference types are stored in a separate field.

**Pros:** Minimum memory footprint, alloc free.

**Cons:** Does not work with Generics (due to CLR limitations with FieldOffset).

```csharp
[StructLayout(LayoutKind.Explicit)]
internal struct __DataUnion_Range 
{
    [FieldOffset(0)] public long _v0;
    [FieldOffset(0)] public int _v1; // Overlays _v0
    [FieldOffset(0)] public short _v2; // Overlays _v0
}

partial struct MyUnion
{
    private __DataUnion_Range _data;
    private object? _ref; // contains all reference types       
}

```



#### 2. Composition *(sum(sizeof(T)) + 4)*

A straightforward approach where every type has its own dedicated field.

**Pros:** Safe, works with everything (including Generics).

**Cons:** Memory footprint equals the sum of all types.

```csharp
partial struct MyUnion 
{
    private readonly long _v0;
    private readonly int _v1;
    private readonly short _v2;
    private readonly string _v3;
}
```


#### 3. Boxing *(sizeof(IntPtr) + 4)*

Stores everything as a single object field.

**Pros:** The smallest possible struct size.

**Cons:** Causes Boxing for value types, which is bad for high-performance scenarios. Use this primarily for classes or when memory size is more critical than GC pressure.

partial struct MyUnion {
    private readonly object? _value;
}


### 📦 Installation

**NuGet**
dotnet add package DotPointers.OneOf

📄 License This project is licensed under the MIT License