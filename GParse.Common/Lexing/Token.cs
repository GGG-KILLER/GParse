using System;
using System.Collections.Generic;

namespace GParse.Common.Lexing
{
    public class Token<TokenTypeT> : IEquatable<Token<TokenTypeT>> where TokenTypeT : Enum
    {
        /// <summary>
        /// The ID of the token
        /// </summary>
        public readonly String ID;

        /// <summary>
        /// The raw value of the token
        /// </summary>
        public readonly String Raw;

        /// <summary>
        /// The value of the token
        /// </summary>
        public readonly Object Value;

        /// <summary>
        /// The <see cref="TokenType" /> of the token
        /// </summary>
        public readonly TokenTypeT Type;

        /// <summary>
        /// The <see cref="SourceRange" /> of the token
        /// </summary>
        public readonly SourceRange Range;

        /// <summary>
        /// Whether this token is a piece of trivia, such as
        /// comments and/or whitespaces
        /// </summary>
        public readonly Boolean IsTrivia;

        /// <summary>
        /// The trivia this token contains
        /// </summary>
        public readonly Token<TokenTypeT>[] Trivia;

        public Token ( String ID, String raw, Object value, TokenTypeT type, SourceRange range )
        {
            this.ID       = ID ?? throw new ArgumentNullException ( nameof ( ID ) );
            this.Raw      = raw ?? throw new ArgumentNullException ( nameof ( raw ) );
            this.Value    = value ?? throw new ArgumentNullException ( nameof ( value ) );
            this.Type     = type;
            this.Range    = range;
            this.IsTrivia = false;
            this.Trivia   = Array.Empty<Token<TokenTypeT>> ( );
        }

        public Token ( String ID, String raw, Object value, TokenTypeT type, SourceRange range, Boolean isTrivia ) : this ( ID, raw, value, type, range )
        {
            this.IsTrivia = isTrivia;
        }

        public Token ( String ID, String raw, Object value, TokenTypeT type, SourceRange range, Token<TokenTypeT>[] trivia ) : this ( ID, raw, value, type, range )
        {
            this.Trivia = trivia ?? throw new ArgumentNullException ( nameof ( trivia ) );
        }

        public Token ( String ID, String raw, Object value, TokenTypeT type, SourceRange range, Boolean isTrivia, Token<TokenTypeT>[] trivia ) : this ( ID, raw, value, type, range )
        {
            this.IsTrivia = isTrivia;
            this.Trivia = trivia;
        }

        public override String ToString ( ) => $"Token_{this.ID}<{this.Type}> ( {this.Raw}, {this.Value} )";

        #region Generated Code

        #region IEquatable<Token<TokenT>>

        public Boolean Equals ( Token<TokenTypeT> other ) => other != null
            && this.ID == other.ID
            && this.Raw == other.Raw
            && this.Value.Equals ( other.Value )
            && this.Type.Equals ( other.Type )
            && this.Range.Equals ( other.Range );

        #endregion IEquatable<Token<TokenT>>

        #region Object

        public override Boolean Equals ( Object obj ) => obj is Token<TokenTypeT> tok ? this.Equals ( tok ) : false;

        public override Int32 GetHashCode ( )
        {
            var hashCode = -690953047;
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<String>.Default.GetHashCode ( this.ID );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<String>.Default.GetHashCode ( this.Raw );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<Object>.Default.GetHashCode ( this.Value );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<TokenTypeT>.Default.GetHashCode ( this.Type );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<SourceRange>.Default.GetHashCode ( this.Range );
            return hashCode;
        }

        #endregion Object

        #region operator ==/!=

        public static Boolean operator == ( Token<TokenTypeT> token1, Token<TokenTypeT> token2 ) => EqualityComparer<Token<TokenTypeT>>.Default.Equals ( token1, token2 );

        public static Boolean operator != ( Token<TokenTypeT> token1, Token<TokenTypeT> token2 ) => !( token1 == token2 );

        #endregion operator ==/!=

        #endregion Generated Code
    }
}
