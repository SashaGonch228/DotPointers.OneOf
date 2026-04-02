# DotPointers.OneOf

DotPointers.OneOf is a **high-performance** Source Generator for C# designed to create **memory-efficient** Discriminated Unions.

By leveraging modern C# features and low-level memory management, it provides a powerful alternative to traditional polymorphism and manual type checking, ensuring maximum performance with zero runtime overhead.

## 📑 Table of Contents
1. [Key Features](#-key-features)
2. [Quick Start](#-quick-start)
3. [Serialization](#-serialization-zero-boilerplate)
4. [Layout Strategies](#-layout-strategies)
5. [Benchmark](#-benchmark)
6. [Installation](#-installation)

### 💎 Key Features

- **Memory-Efficient Layouts:** Gain full control over data representation. Use Explicit layout to overlay fields and minimize struct size, or Composition for maximum compatibility.

- **High-Performance CodeGen:** Generates highly inlinable code using Unsafe and AggressiveInlining. Achieve the speed of hand-optimized logic while maintaining a clean, high-level API.

- **Direct Access & Switching:** The fastest way to handle Unions in .NET. Work directly with the Kind discriminator to bypass delegate overhead and achieve superior performance.

- **Compile-Time Safety:** No runtime magic. All methods (Match, Switch, TryPick) are generated during the build, providing instant feedback and full IDE support.

- **Zero GC Pressure:** Operates exclusively on the stack with value types. Avoids boxing in hot paths and eliminates Garbage Collector overhead.

- **Native Serialization:** Out-of-the-box support for MemoryPack, System.Text.Json, and Newtonsoft.Json via simple attributes—no custom converters required.

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

Note on Void: Since the generator uses a fixed-arity interface, use the Void type as a placeholder for unused type slots. These slots will be ignored by the generator and won't affect the memory layout.

*More samples in DotPointers.OneOf.Samples*

### 🔌 Serialization (Zero-Boilerplate)
DotPointers.OneOf supports the most popular serializers out of the box. No custom converters needed.

| Serializer | Attribute | Description |
| :--- | :--- | :--- |
| **MemoryPack** | `[GenerateMemoryPack]` | Ultra-fast binary serialization (1-byte Kind tag + sizeof(current)). |
| **System.Text.Json** | `[GenerateSystemTextJson]` | Native .NET JSON support. |
| **Newtonsoft.Json** | `[GenerateNewtonsoftJson]` | Legacy/Advanced JSON support. |


### 🛠 Layout Strategies

You can control how the data is stored in memory via OneOfLayoutKind. Choosing the right strategy is key for performance and memory usage.

#### 1. Explicit *(max(sizeof(TValue)) + sizeof(IntPtr) + 4)*

The most efficient way for value types. All value types are overlaid in a single Explicit struct (sharing the same memory space), while reference types are stored in a separate field.

**Pros:** Minimum memory footprint, allocation-free.

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

```csharp
partial struct MyUnion {
    private readonly object? _value;
}
```

### 💥 Benchmark

| Method                        | Mean         | Ratio | Code Size | Allocated | Object size |
|------------------------------ |-------------:|------:|----------:|----------:|------------:|
| Match_OneOf_Massive           | 1.371 ms     |  1.00 |     758 B |         - |        32 B |
| Match_Explicit_Massive        | **1.302 ms** |  0.95 |     788 B |         - |    **20 B** |
| Match_Composition_Massive     | 1.442 ms     |  1.05 |     791 B |         - |        32 B |
| DirectSwitch_Explicit_Massive | **1.148 ms** |  0.84 | **237 B** |         - |    **20 B** |

*Object size measured for a Union of 3 value types (GUID, long, int).*

### 📦 Installation

**NuGet**
dotnet add package DotPointers.OneOf

📄 License This project is licensed under the MIT License