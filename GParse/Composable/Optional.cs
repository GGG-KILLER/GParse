using System.Diagnostics.CodeAnalysis;

namespace GParse.Composable
{
    /// <summary>
    /// A class containing extension methods for grammar nodes.
    /// </summary>
    public static partial class GrammarNodeExtensions
    {
        /// <summary>
        /// Makes a node optional.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull ( "node" )]
        public static Repetition<T>? Optional<T> ( this GrammarNode<T>? node ) =>
            node is null ? null : new Repetition<T> ( node, new RepetitionRange ( 0, 1 ), false );
    }
}