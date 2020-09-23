using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a negated alternation.
    /// </summary>
    public sealed class NegatedAlternation : GrammarNodeListContainer<NegatedAlternation, Char>
    {
        /// <summary>
        /// Initializes a new negated alternation.
        /// </summary>
        /// <param name="grammarNodes">The grammar nodes to initialize with.</param>
        public NegatedAlternation ( params GrammarNode<Char>[] grammarNodes ) : base ( grammarNodes, true )
        {
        }

        /// <summary>
        /// Initializes a new negated alternation.
        /// </summary>
        /// <param name="grammarNodes">The grammar nodes to initialize with.</param>
        public NegatedAlternation ( IEnumerable<GrammarNode<Char>> grammarNodes ) : base ( grammarNodes, true )
        {
        }

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            GrammarNodeToStringConverter.Convert ( this );
    }

    /// <summary>
    /// A class containing extension methods for grammar nodes.
    /// </summary>
    public static partial class GrammarNodeExtensions
    {
        /// <summary>
        /// Negates an alternation.
        /// </summary>
        /// <param name="alternation"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull ( "alternation" )]
        public static NegatedAlternation AsNegated ( this Alternation<Char> alternation ) =>
            new NegatedAlternation ( alternation.GrammarNodes );
    }
}
