# DotPointers.OneOf

[![NuGet](https://img.shields.io/nuget/v/DotPointers.OneOf.svg)](https://www.nuget.org/packages/DotPointers.OneOf/)
![License](https://img.shields.io/github/license/SashaGonch228/DotPointers.OneOf)
![GitHub Stars](https://img.shields.io/github/stars/SashaGonch228/DotPointers.OneOf?style=social)
![NuGet Downloads](https://img.shields.io/nuget/dt/DotPointers.OneOf)

<img src="logo.png" align="right" width="160px" />

DotPointers.OneOf is a **high-performance** Source Generator for C# designed to create **memory-efficient** Discriminated Unions.

By leveraging modern C# features and low-level memory management, it provides a powerful alternative to traditional polymorphism and manual type checking, ensuring maximum performance with zero runtime overhead.

## 📑 Table of Contents
1. [Key Features](#-key-features)
2. [Quick Start](#-quick-start)
3. [Serialization](#-serialization-zero-boilerplate)
4. [Layout Strategies](#-layout-strategies)
5. [Build-In types](#-build-in-types)
6. [Benchmark](#-benchmark)
7. [Installation](#-installation)

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
        _ => true,
        _ => false
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
| **Unity** | `[GenerateUnity]` | Wrapper for unity inspector |


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

#### 4. Hybrid *(sizeof(IntPtr) + sum(sizeof(T struct)) + 4)*

All value types are stored sequentially, and objects are stored in a single object field 

**Pros:** The smallest possible struct size.

**Cons:** Causes Boxing for value types, which is bad for high-performance scenarios. Use this primarily for classes or when memory size is more critical than GC pressure.

```csharp
partial struct MyUnion {
    private readonly object? _ref;
	private readonly long _v0;
	private readonly int _v1;
	private readonly short _v2;
}
```

### ⚙ Build-In types

Universal

| Type             | Purpose |
| ---------------- | ------- |
| OneOf`<T0, ..., T7>`| Universal union type supporting up to 8 different types. Perfect for general use cases. Uses composition |
| Void types | Types that reserve a slot but do not occupy memory |

Core Containers

| Type             | States (Options)           | Purpose |
| ---------------- | -------------------------- | ----- |
| Option`<T>`      | Value, Empty               | A safe replacement for null. Forces explicit handling of missing values. |
| Result`<T>`      | Value, Error               | Handles operations that can fail with an Exception. |
| Validation<T, E> | Value, Failures            | Accumulates validation errors (array of E[]) instead of short-circuiting on the first failure. |
| Response`<T>`    | Content, NotFound, Failure | Perfect for API/Service layers: distinguishes between "No Result Found" and "System Error". |

Collections

| Type           | States (Options)       | Purpose |
| -------------- | ---------------------- | ----- |
| OneOrMany`<T>` | Single, Multiple       | Guarantees at least one element. Optimized to avoid allocations for single-item scenarios. Implements IEnumerable<T>. |
| Many`<T>`      | Single, Multiple, None | A versatile wrapper for 0, 1, or N elements. Implements IEnumerable<T>. |
| Attempt`<T>`   | Value, Task            | Lazy data retrieval: either the value is already present, or it must be awaited via Task<T>. |

Domain & Logic Types

| Type       | States (Options)           | Purpose |
| ---------- | -------------------------- | ----- |
| Numeric    | Number, Text               | Handles numbers that might arrive as strings (e.g., from web forms or CSV files). |
| Range`<T>` | Single, Between, Any, None | Describes ranges: a specific value, an interval, "Any" (wildcard), or "None". |
| Tristate   | True, False, Unknown       | Extended logic (SQL-style). Allows distinguishing "No" from "Indeterminate". |

### 💥 Benchmark

#### Match Performance

| Method                        | Mean         | Ratio | Code Size | Object size |
|------------------------------ |-------------:|------:|----------:|------------:|
| mcintyre321 OneOf Match       | 1.112 ms     |  1.00 |     720 B |        32 B |
| Explicit Match                | **1.049 ms** |  0.94 |     706 B |    **20 B** |
| Composition Match             | 1.083 ms     |  0.97 |     706 B |        32 B |
| Explicit DirectSwitch         | **0,958 ms** |  0.86 | **224 B** |    **20 B** |

#### Creation Performance

| Method                   | Mean         | Ratio | Code Size | Object size |
|------------------------- |-------------:|------:|----------:|------------:|
| mcintyre321 OneOf Create | 490 ns       |  1.00 |     82 B  |        32 B |
| Explicit Create          | **417 ns**   |  0.94 |     90 B  |    **20 B** |

*Object size measured for a Union of 3 value types (GUID, long, int).*

### 📦 Installation

**NuGet**
dotnet add package DotPointers.OneOf

📄 License This project is licensed under the MIT License