using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a node that matches any single char.
    /// </summary>
    public sealed class Any : GrammarNode<Char>
    {
    }
}
