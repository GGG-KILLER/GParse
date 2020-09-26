using System.Diagnostics.CodeAnalysis;

namespace GParse.Composable
{
    /// <summary>
    /// A class containing extension methods for grammar nodes.
    /// </summary>
    public partial class GrammarNodeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull ( "node" )]
        public static Repetition<T>? Optional<T> ( GrammarNode<T>? node ) =>
            node is null ? null : new Repetition<T> ( node, new RepetitionRange ( 0, 1 ), false );
    }
}