using System;
using System.Linq;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Utility methods for <see cref="GrammarNode{T}"/>.
    /// </summary>
    public static class NodeUtils
    {
        /// <summary>
        /// Checks whether the node is an alternation set element.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Boolean IsAlternationSetElement ( GrammarNode<Char> node ) =>
            node is CharacterRange
                 or CharacterSet
                 or CharacterTerminal
                 or UnicodeCategoryTerminal;

        /// <summary>
        /// Checks whether the provided alternation is equivalent to
        /// a regex alternation set.
        /// </summary>
        /// <param name="alternation"></param>
        /// <returns></returns>
        public static Boolean IsAlternationSet ( Alternation<Char> alternation ) =>
            alternation.GrammarNodes.All ( node => IsAlternationSetElement ( node ) );
    }
}
