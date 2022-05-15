using System.Runtime.CompilerServices;

// This stuff is complex enough, that I want to test some internals also.
// Normal usage tests are separate from unit tests, so that if usage breaks,
// unit tests still run.
// Might change my mind later :)
[assembly: InternalsVisibleTo("GenSubstitute.Tests")]
