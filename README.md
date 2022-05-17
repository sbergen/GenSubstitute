# GenSubstitute

GenSubstitute is a C# source generator based mocking library.

It's still fairly early in its development,
so while feedback and bug reports are extremely welcome,
use at your own risk!

## Installation

### .NET projects

Get from NuGet in your IDE, or use
```
dotnet add package GenSubstitute --version 0.1.0-alpha
```

### Unity 2021.3 or newer

1. Download `UnityDlls.zip` from the latest [release](https://github.com/sbergen/GenSubstitute/releases).
2. Add the `.dll` files into your Unity assets.
3. Optionally add the `.pdb` files next to the dlls for debug symbols.
4. Reference `GenSubstitute.Runtime.dll` from your test project.
5. Set up `GenSubstitute.SourceGenerator.Unity.dll` as a source generator, as outlined in the
   [Unity instructions](https://docs.unity3d.com/Manual/roslyn-analyzers.html).

## Examples

You can compare these with the [NSubstitute examples](https://nsubstitute.github.io/).

### Basic usage

```cs
// Create:
var calculator = Gen.Substitute<ICalculator>().Create();

// Set a return value:
calculator.SetUp.Add(1, 2).Returns(3);
Assert.Equal(3, calculator.Object.Add(1, 2));

// Configure a method with a lambda
calculator.SetUp.Add(Arg.Any, Arg.Any).As((a, b) => a + b);
Assert.Equal(42, calculator.Object.Add(40, 2));

// Check received calls and arguments:
calculator.Received.Add(1, Arg.Any).Once();
calculator.Received.Add(2, 2).Never();
Assert.Equal(40, calculator.Received.Add(Arg.Any, Arg.Any)[1].Arg1);

// Events are not yet supported
```

### Helpful exceptions

```
ReceivedCallsAssertionException: Expected to receive one call matching:
  System.Int32 ICalculator.Add(1, any System.Int32)
Actually received:
  Int32 ICalculator.Add(40, 2)
```

Some prettifying of the error messages is on the TODO list.

## Pros (compared to dynamically generated mocks)
- Can be made to run on AOT platforms:
  - Supporting Unity was one of the main goals, but requires a non-incremental source generator version (running on older Roslyn libraries).
  - I have not tried any AOT platforms yet, but plan on supporting them later.
- Easier mocking of method behavior.
- No surprising runtime errors:
  - Setting up and checking received calls are done on separate objects, preventing surprising failures at runtime.
  - Argument matchers, method call matchers, and received calls are plain objects:
    They can be constructed and reused freely in any context.
- Easier to extend:
  - This is also related to everything being just regular objects,
  you can e.g. write your own extension methods on `IReceivedCallsInfo<T>` to build custom assertions.

## Cons
- GenSubstitute is still work in progress.
- Slightly longer syntax, as configuring and checking is done separately from the implementation (but see the pros).
- IDE refactorings on method and property names do not update the generated code.
  This could be achieved with some additional tooling, which should be investigated.
- Code generation will inherently have some effect on IDE performance.

## Features

### Implemented
- Mocking interfaces (classes are not supported at the moment)
  - Interfaces which hide members with `new` (e.g. `IEnumerable<T>`) are not yet supported.
  - Mocking events is not yet supported.
- Mocking properties
  - Manually configured
  - Value-retaining mode (anything set will be returned by get)
- Mocking methods
  - Configuring return values
  - Configuring functionality with `Action` or `Func`
  - `ref` and `out` parameters are supported
- Argument matching with a value or predicate
  - You may configure multiple matchers,
    but if multiple matches are found at runtime, this will be an error.
    Reconfiguring the same arguments must thus be done on the returned configurer object.
- Checking received calls
  - This can be done on three levels:
    - Per substitution context (multiple mocked objects)
    - Per mock object
    - Per method
  - Checking number of received calls
  - Checking that calls are received in a specific order

### Definitely on the TODO list
- Better documentation
- Improving messages in received call assertions (and other errors/assertions).
- Supporting interfaces which hide members with `new`.
- Configuring default return values in a substitution context.
  Currently everything just returns `default`, unless configured otherwise.
- Mocking events (I personally prefer `IObservable` over events, but this will probably be needed at some point).
- Preventing functionality meant only for generated code from being used directly.
  Due to the generated code living alongside "user code", this is not completely trivial.
- Thread-safety: Currently GenSubstitute is only safe to use from a single thread.

### Will probably be added
- Mocking delegates. Fairly easy to live without, but could leverage the infrastructure.
- Configuring a sequence of return values for methods/properties

### Needs investigation
- Supporting IDE renaming refactoring better

### Hesitant to add
- Mocking classes. I consider this bad practice, but could consider adding it if there is demand.

## Kudos

- [NSubstitute](https://nsubstitute.github.io/) is the C# mocking framework I've used the most,
  and was a big inspiration, as seen from the name :)
- Other source-generator-based mocking libraries that were useful benchmarks:
  - [SourceMock](https://github.com/ashmind/SourceMock)
  - [MockGen](https://github.com/thomas-girotto/MockGen)
  - [MockSourceGenerator](https://github.com/hermanussen/MockSourceGenerator)

## Why a new library?

My main motivation for writing GenSubstitute was to eventually support it [Unity](https://unity3d.com/),
in order to run tests that used mocks in [IL2CPP](https://docs.unity3d.com/Manual/IL2CPP.html) builds.
While there are some source generator based mocking libraries around,
there were things about each I did not like,
so I thought it would be fun to write my own.
I also hope others find it useful!

I also believe there are things that can be done better with source generation compared to dynamic mocking.
And when working on GenSubstitute, I've been trying to challenge my current ways of thinking about mocking,
and think outside the box where possible - I'm willing to bet there are discoveries yet to be made!