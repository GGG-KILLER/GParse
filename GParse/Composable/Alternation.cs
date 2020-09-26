using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// Represents an alternation of different possible grammar trees.
    /// </summary>
    public class Alternation<T> : GrammarNodeListContainer<Alternation<T>, T>
    {
        /// <summary>
        /// Initializes an alternation.
        /// </summary>
        /// <param name="grammarNodes"></param>
        public Alternation ( params GrammarNode<T>[] grammarNodes ) : base ( grammarNodes, true )
        {
        }

        /// <summary>
        /// Initializes an alternation.
        /// </summary>
        /// <param name="grammarNodes"></param>
        public Alternation ( IEnumerable<GrammarNode<T>> grammarNodes ) : base ( grammarNodes, true )
        {

        }
    }
}
