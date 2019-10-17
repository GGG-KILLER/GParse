using System.Collections.Generic;

namespace GParse.Composable
{
    /// <summary>
    /// The base class for all grammar nodes
    /// </summary>
    public abstract class GrammarNode
    {
        /// <summary>
        /// Creates an alternation node
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Alternation operator | ( GrammarNode left, GrammarNode right )
        {
            var nodes = new List<GrammarNode> ( );
            if ( left is Alternation leftAlternation )
                nodes.AddRange ( leftAlternation.GrammarNodes );
            else
                nodes.Add ( left );
            if ( right is Alternation rightAlternation )
                nodes.AddRange ( rightAlternation.GrammarNodes );
            else
                nodes.Add ( right );
            return new Alternation ( nodes.ToArray ( ) );
        }

        /// <summary>
        /// Creates a sequence node
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Sequence operator & ( GrammarNode left, GrammarNode right )
        {
            var nodes = new List<GrammarNode> ( );
            if ( left is Sequence leftAlternation )
                nodes.AddRange ( leftAlternation.GrammarNodes );
            else
                nodes.Add ( left );
            if ( right is Sequence rightAlternation )
                nodes.AddRange ( rightAlternation.GrammarNodes );
            else
                nodes.Add ( right );
            return new Sequence ( nodes.ToArray ( ) );
        }

        /// <summary>
        /// Creates a sequence node
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Sequence operator + ( GrammarNode left, GrammarNode right ) =>
            left & right;
    }
}
