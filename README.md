# GenSubstitute

GenSubstitute is a C# source generator based mocking library.

## Pros
- Runs on AOT
- Argument matcher types can't mismatch
- No "magic" required for argument matchers, can e.g. be reused
- No "Returns can only be called on substitutes"


## Cons
- Refactoring interfaces does not refactor builder methods
- Slows down editor (should profile and optimize)
- Incomplete, for now

## TODO
- Write a proper REAMDE :)
- Delegate mocking (this will be different, but simpler)
- Default value policies for return values (instead of fully automatic recursive mocking)
- `new` in interfaces (e.g. `IEnumerable<T>`) - configuring probably gets complicated
- Received.InOrder equivalent? Probably should use an optional mocking context.
- Sequential return values?
- Support non-interfaces?
