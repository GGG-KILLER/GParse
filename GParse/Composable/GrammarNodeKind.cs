using GParse.Lexing.Composable;

namespace GParse.Composable
{
    /// <summary>
    /// The kind of a <see cref="GrammarNode{T}"/>.
    /// </summary>
    public enum GrammarNodeKind
    {
        #region Generic Nodes

        /// <summary>
        /// An <see cref="Alternation{T}"/>.
        /// </summary>
        Alternation,

        /// <summary>
        /// A <see cref="NonTerminal{T}"/>.
        /// </summary>
        NonTerminal,

        /// <summary>
        /// A <see cref="Repetition{T}"/>.
        /// </summary>
        Repetition,

        /// <summary>
        /// A <see cref="Sequence{T}"/>.
        /// </summary>
        Sequence,

        #endregion Generic Nodes

        #region Character Nodes

        /// <summary>
        /// An <see cref="Any"/> node.
        /// </summary>
        CharacterAny,

        /// <summary>
        /// A <see cref="Lexing.Composable.CharacterRange"/>.
        /// </summary>
        CharacterRange,

        /// <summary>
        /// A <see cref="Lexing.Composable.CharacterTerminal"/>.
        /// </summary>
        CharacterTerminal,

        /// <summary>
        /// A <see cref="NamedBackreference"/>.
        /// </summary>
        CharacterNamedBackreference,

        /// <summary>
        /// A <see cref="NamedCapture"/>.
        /// </summary>
        CharacterNamedCapture,

        /// <summary>
        /// A <see cref="NegatedCharacterRange"/>.
        /// </summary>
        CharacterNegatedRange,

        /// <summary>
        /// A <see cref="NegatedCharacterTerminal"/>.
        /// </summary>
        CharacterNegatedTerminal,

        /// <summary>
        /// A <see cref="NegatedSet"/>.
        /// </summary>
        CharacterNegatedSet,

        /// <summary>
        /// A <see cref="NegatedUnicodeCategoryTerminal"/>.
        /// </summary>
        CharacterNegatedUnicodeCategoryTerminal,

        /// <summary>
        /// A <see cref="NegativeLookahead"/>.
        /// </summary>
        CharacterNegativeLookahead,

        /// <summary>
        /// A <see cref="NumberedBackreference"/>.
        /// </summary>
        CharacterNumberedBackreference,

        /// <summary>
        /// A <see cref="NumberedCapture"/>.
        /// </summary>
        CharacterNumberedCapture,

        /// <summary>
        /// An <see cref="OptimizedNegatedSet"/>.
        /// </summary>
        CharacterOptimizedNegatedSet,

        /// <summary>
        /// An <see cref="OptimizedSet"/>.
        /// </summary>
        CharacterOptimizedSet,

        /// <summary>
        /// A <see cref="PositiveLookahead"/>.
        /// </summary>
        CharacterPositiveLookahead,

        /// <summary>
        /// A <see cref="Set"/>.
        /// </summary>
        CharacterSet,

        /// <summary>
        /// A <see cref="StringTerminal"/>.
        /// </summary>
        CharacterStringTerminal,

        /// <summary>
        /// An <see cref="UnicodeCategoryTerminal"/>.
        /// </summary>
        CharacterUnicodeCategoryTerminal

        #endregion Character Nodes
    }
}
