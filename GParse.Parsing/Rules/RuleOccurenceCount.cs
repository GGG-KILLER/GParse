namespace GParse.Parsing.Rules
{
    /// <summary>
    /// Indicates the amount of times a certain search step (the
    /// names are pretty much self-explaining)
    /// </summary>
    internal enum OccurrenceCount
    {
        OnlyOne,
        ZeroOrOne,
        OneOrMore,
        ZeroOrMore
    }
}
