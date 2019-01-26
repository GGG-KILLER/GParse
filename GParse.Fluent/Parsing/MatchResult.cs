using System;
using System.Collections.Generic;
using GParse.Common.Errors;

namespace GParse.Fluent.Parsing
{
    /// <summary>
    /// Result of a match
    /// </summary>
    public struct MatchResult<NodeT> : IEquatable<MatchResult<NodeT>>
    {
        /// <summary>
        /// Whether the match was sucessful
        /// </summary>
        public readonly Boolean Success;

        /// <summary>
        /// The nodes that resulted of this match
        /// </summary>
        public readonly NodeT[] Nodes;

        /// <summary>
        /// The strings that resulted of this match
        /// </summary>
        public readonly String[] Strings;

        /// <summary>
        /// Error when match is unsuccessful
        /// </summary>
        public readonly FatalParsingException Error;

        /// <summary>
        /// Initializes a successful
        /// <see cref="MatchResult{NodeT}" /> without any nodes
        /// </summary>
        /// <param name="strings"></param>
        public MatchResult ( String[] strings ) : this ( Array.Empty<NodeT> ( ), strings )
        {
        }

        /// <summary>
        /// Initializes a successful
        /// <see cref="MatchResult{NodeT}" /> without any strings
        /// </summary>
        /// <param name="nodes"></param>
        public MatchResult ( NodeT[] nodes ) : this ( nodes, Array.Empty<String> ( ) )
        {
        }

        /// <summary>
        /// Intializes a successful <see cref="MatchResult{NodeT}" />
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="strings"></param>
        public MatchResult ( NodeT[] nodes, String[] strings )
        {
            this.Success = true;
            this.Nodes = nodes;
            this.Strings = strings;
            this.Error = null;
        }

        /// <summary>
        /// Initializes an unsuccessful <see cref="MatchResult{NodeT}" />
        /// </summary>
        /// <param name="error"></param>
        public MatchResult ( FatalParsingException error )
        {
            this.Success = false;
            this.Nodes = null;
            this.Strings = null;
            this.Error = error;
        }

        /// <summary>
        /// Returns a section of the <see cref="Nodes" /> array
        /// </summary>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public NodeT[] GetNodesSection ( Int32 index, Int32 length )
        {
            // Negative length
            if ( length < 0 )
                length = this.Nodes.Length + length;

            var narr = new NodeT[length];
            Array.Copy ( this.Nodes, index, narr, 0, length );
            return narr;
        }

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => obj is MatchResult<NodeT> && this.Equals ( ( MatchResult<NodeT> ) obj );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( MatchResult<NodeT> other ) => this.Success == other.Success && EqualityComparer<NodeT[]>.Default.Equals ( this.Nodes, other.Nodes ) && EqualityComparer<String[]>.Default.Equals ( this.Strings, other.Strings ) && EqualityComparer<FatalParsingException>.Default.Equals ( this.Error, other.Error );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 1864596756;
            hashCode = hashCode * -1521134295 + this.Success.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<NodeT[]>.Default.GetHashCode ( this.Nodes );
            hashCode = hashCode * -1521134295 + EqualityComparer<String[]>.Default.GetHashCode ( this.Strings );
            hashCode = hashCode * -1521134295 + EqualityComparer<FatalParsingException>.Default.GetHashCode ( this.Error );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="result1"></param>
        /// <param name="result2"></param>
        /// <returns></returns>
        public static Boolean operator == ( MatchResult<NodeT> result1, MatchResult<NodeT> result2 ) => result1.Equals ( result2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="result1"></param>
        /// <param name="result2"></param>
        /// <returns></returns>
        public static Boolean operator != ( MatchResult<NodeT> result1, MatchResult<NodeT> result2 ) => !( result1 == result2 );

        #endregion Generated Code
    }
}
