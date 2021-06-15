using System;
using System.Diagnostics.CodeAnalysis;
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
        [return: NotNullIfNotNull("grammarNode")]
        public static GrammarNode<Char>? Negate(this GrammarNode<Char> grammarNode)
        {
            return grammarNode?.Kind switch
            {
                null => null,
                GrammarNodeKind.CharacterTerminal => !(CharacterTerminal) grammarNode,
                GrammarNodeKind.CharacterRange => !(CharacterRange) grammarNode,
                GrammarNodeKind.CharacterPositiveLookahead => !(PositiveLookahead) grammarNode,
                GrammarNodeKind.CharacterNegatedRange => !(NegatedCharacterRange) grammarNode,
                GrammarNodeKind.CharacterNegatedTerminal => !(NegatedCharacterTerminal) grammarNode,
                GrammarNodeKind.CharacterNegatedSet => !(NegatedSet) grammarNode,
                GrammarNodeKind.CharacterNegatedUnicodeCategoryTerminal => !(NegatedUnicodeCategoryTerminal) grammarNode,
                GrammarNodeKind.CharacterNegativeLookahead => !(NegativeLookahead) grammarNode,
                GrammarNodeKind.CharacterSet => !(Set) grammarNode,
                GrammarNodeKind.CharacterUnicodeCategoryTerminal => !(UnicodeCategoryTerminal) grammarNode,
                GrammarNodeKind.CharacterOptimizedSet => !(OptimizedSet) grammarNode,
                GrammarNodeKind.CharacterOptimizedNegatedSet => !(OptimizedNegatedSet) grammarNode,
                _ => throw new InvalidOperationException($"Cannot negate a node of the type {grammarNode.GetType().Name}."),
            };
        }
    }
}