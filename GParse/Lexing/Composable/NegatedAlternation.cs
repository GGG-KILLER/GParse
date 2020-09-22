using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GParse.Composable;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a negated alternation.
    /// </summary>
    public class NegatedAlternation : GrammarNodeListContainer<NegatedAlternation, Char>
    {
        private static IEnumerable<GrammarNode<Char>> GetNodes ( IEnumerable<GrammarNode<Char>> nodes )
        {
            foreach ( GrammarNode<Char> node in nodes )
            {
                // We only accept nodes that can match at most 1 char.
                yield return node switch
                {
                    CharacterRange range => !range,
                    CharacterSet set => !set,
                    CharacterTerminal terminal => !terminal,
                    UnicodeCategoryTerminal category => !category,
                    _ => throw new ArgumentException ( "", "grammarNodeds" )
                };
            }
        }

        /// <summary>
        /// Initializes a new negated alternation.
        /// </summary>
        /// <param name="grammarNodes">The grammar nodes to initialize with.</param>
        public NegatedAlternation ( params GrammarNode<Char>[] grammarNodes ) : base ( GetNodes ( grammarNodes ), true )
        {
        }

        /// <summary>
        /// Initializes a new negated alternation.
        /// </summary>
        /// <param name="grammarNodes">The grammar nodes to initialize with.</param>
        public NegatedAlternation ( IEnumerable<GrammarNode<Char>> grammarNodes ) : base ( GetNodes ( grammarNodes ), true )
        {
        }

        /// <summary>
        /// Converts a child node into a string.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static String NodeToString ( GrammarNode<Char> node )
        {
            return node switch
            {
                NegatedCharacterRange nrange => $"{CharUtils.ToReadableString ( nrange.Start )}-{CharUtils.ToReadableString ( nrange.End )}",
                NegatedCharacterSet nset => String.Join ( "", nset.CharSet.Select ( ch => CharUtils.ToReadableString ( ch ) ) ),
                NegatedCharacterTerminal nterminal => CharUtils.ToReadableString ( nterminal.Value ),
                NegatedUnicodeCategoryTerminal ncategory => $"\\p{{{ncategory.Category}}}",
                _ => throw new InvalidOperationException ( "Invalid node provided." )
            };
        }

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"[^{String.Join ( "", this.GrammarNodes.Select ( node => NodeToString ( node ) ) )}]";
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
