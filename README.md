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
- Support properties
- Support default value policies for return values (instead of fully automatic recursive mocking)
- Received.InOrder equivalent?
- Sequential return values?
- Support non-interfaces?
