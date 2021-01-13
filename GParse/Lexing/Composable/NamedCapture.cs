using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A node that stores the matched value under a name
    /// </summary>
    public sealed class NamedCapture : GrammarNodeContainer<Char>, IEquatable<NamedCapture?>
    {
        /// <summary>
        /// The name of the node to store the match under
        /// </summary>
        public String Name { get; }

        /// <inheritdoc/>
        public override GrammarNodeKind Kind => GrammarNodeKind.CharacterNamedCapture;

        /// <summary>
        /// Initializes this node
        /// </summary>
        /// <param name="name"></param>
        /// <param name="node"></param>
        public NamedCapture ( String name, GrammarNode<Char> node ) : base ( node )
        {
            this.Name = name ?? throw new ArgumentNullException ( nameof ( name ) );
        }

        /// <inheritdoc/>
        public override Boolean Equals ( Object? obj ) => this.Equals ( obj as NamedCapture );

        /// <inheritdoc/>
        public Boolean Equals ( NamedCapture? other ) =>
            other != null
            && EqualityComparer<GrammarNode<Char>>.Default.Equals ( this.InnerNode, other.InnerNode )
            && StringComparer.Ordinal.Equals ( this.Name, other.Name );

        /// <inheritdoc/>
        public override Int32 GetHashCode ( ) =>
            HashCode.Combine ( this.InnerNode, this.Name );

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"(?<{this.Name}>{GrammarNodeToStringConverter.Convert ( this.InnerNode )})";

        /// <summary>
        /// Checks whether two named captures are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( NamedCapture? left, NamedCapture? right )
        {
            if ( right is null ) return left is null;
            return right.Equals ( left );
        }

        /// <summary>
        /// Checks whether two named captures are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( NamedCapture? left, NamedCapture? right ) => !( left == right );
    }

    /// <summary>
    /// A class containing extension methods for grammar nodes.
    /// </summary>
    public static partial class GrammarNodeExtensions
    {
        /// <summary>
        /// Stores the value matched by this node under a name on the matches group
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull ( "node" )]
        public static NamedCapture? AsNamed ( this GrammarNode<Char>? node, String name ) =>
            node is null ? null : new NamedCapture ( name, node );
    }
}