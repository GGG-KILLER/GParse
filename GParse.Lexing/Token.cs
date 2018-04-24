using System;
using System.Collections.Generic;
using GParse.Common;

namespace GParse.Lexing
{
    public class Token : IEquatable<Token>
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
        public readonly TokenType Type;

        /// <summary>
        /// The <see cref="SourceRange" /> of the token
        /// </summary>
        public readonly SourceRange Range;

        public Token ( String ID, String raw, Object value, TokenType type, SourceRange range )
        {
            this.ID = ID ?? throw new ArgumentNullException ( nameof ( ID ) );
            this.Raw = raw ?? throw new ArgumentNullException ( nameof ( raw ) );
            this.Value = value ?? throw new ArgumentNullException ( nameof ( value ) );
            this.Type = type;
            this.Range = range;
        }

        public override String ToString ( )
        {
            return $"Token_{this.ID}<{this.Type}> ( {this.Raw}, {this.Value} )";
        }

        #region Generated Code

        public override Boolean Equals ( Object obj )
        {
            return this.Equals ( obj as Token );
        }

        public Boolean Equals ( Token other )
        {
            return other != null
                     && this.ID == other.ID
                     && this.Raw == other.Raw
                    && EqualityComparer<Object>.Default.Equals ( this.Value, other.Value )
                     && this.Type == other.Type
                    && this.Range.Equals ( other.Range );
        }

        public override Int32 GetHashCode ( )
        {
            var hashCode = -690953047;
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<String>.Default.GetHashCode ( this.ID );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<String>.Default.GetHashCode ( this.Raw );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<Object>.Default.GetHashCode ( this.Value );
            hashCode = ( hashCode * -1521134295 ) + this.Type.GetHashCode ( );
            hashCode = ( hashCode * -1521134295 ) + EqualityComparer<SourceRange>.Default.GetHashCode ( this.Range );
            return hashCode;
        }

        public static Boolean operator == ( Token token1, Token token2 )
        {
            return EqualityComparer<Token>.Default.Equals ( token1, token2 );
        }

        public static Boolean operator != ( Token token1, Token token2 )
        {
            return !( token1 == token2 );
        }

        #endregion Generated Code
    }
}
