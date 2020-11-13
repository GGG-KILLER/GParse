using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    public static partial class GrammarNodeExtensions
    {
        /// <summary>
        /// Negates a node if possible. Throws if not possible.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <returns></returns>
        public static GrammarNode<Char>? Negate ( this GrammarNode<Char> grammarNode )
        {
            return grammarNode switch
            {
                CharacterTerminal characterTerminal => !characterTerminal,
                CharacterRange characterRange => !characterRange,
                Lookahead lookahead => !lookahead,
                NegatedCharacterRange negatedCharacterRange => !negatedCharacterRange,
                NegatedCharacterTerminal negatedCharacterTerminal => !negatedCharacterTerminal,
                NegatedSet negatedSet => !negatedSet,
                NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal => !negatedUnicodeCategoryTerminal,
                NegativeLookahead negativeLookahead => !negativeLookahead,
                Set set => !set,
                UnicodeCategoryTerminal unicodeCategoryTerminal => !unicodeCategoryTerminal,
                OptimizedSet optimizedSet => !optimizedSet,
                OptimizedNegatedSet optimizedNegatedSet => !optimizedNegatedSet,
                _ => throw new InvalidOperationException ( $"Cannot negate a node of the type {grammarNode?.GetType ( ).Name ?? "null"}." ),
            };
        }
    }
}
