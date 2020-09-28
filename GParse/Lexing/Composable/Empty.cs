using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a node that will always succeed with a length of 0.
    /// </summary>
    public sealed class Empty : GrammarNode<Char>
    {
    }
}
