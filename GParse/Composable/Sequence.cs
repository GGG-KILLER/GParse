using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// Represents a sequence of grammar rules
    /// </summary>
    public sealed class Sequence<T> : GrammarNodeListContainer<Sequence<T>, T>
    {
        /// <summary>
        /// Initializes a sequence.
        /// </summary>
        /// <param name="grammarNodes"></param>
        public Sequence ( params GrammarNode<T>[] grammarNodes ) : base ( grammarNodes, true )
        {
        }

        /// <summary>
        /// Initializes a sequence.
        /// </summary>
        /// <param name="grammarNodes"></param>
        public Sequence ( IEnumerable<GrammarNode<T>> grammarNodes ) : base ( grammarNodes, true )
        {
        }
    }
}
