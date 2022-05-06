namespace GenSubstitute
{
    public static class Gen
    {
        public static GenerateMarker<T> Substitute<T>() => new ();
    }
}
