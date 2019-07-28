using System;
using System.Collections.Generic;

namespace GParse.Lexing
{
    /// <summary>
    /// A token outputted by the lexer
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public struct Token<TokenTypeT> : IEquatable<Token<TokenTypeT>>
    {
        /// <summary>
        /// The ID of the token
        /// </summary>
        public readonly String Id;

        /// <summary>
        /// The raw value of the token
        /// </summary>
        public readonly String Raw;

        /// <summary>
        /// The value of the token
        /// </summary>
        public readonly Object Value;

        /// <summary>
        /// The type of the token
        /// </summary>
        public readonly TokenTypeT Type;

        /// <summary>
        /// The <see cref="SourceRange" /> of the token
        /// </summary>
        public readonly SourceRange Range;

        /// <summary>
        /// Whether this token is a piece of trivia, such as comments and/or whitespaces
        /// </summary>
        public readonly Boolean IsTrivia;

        /// <summary>
        /// The trivia this token contains
        /// </summary>
        private readonly Token<TokenTypeT>[] _trivia;

        /// <summary>
        /// The trivia this token contains
        /// </summary>
        public IReadOnlyList<Token<TokenTypeT>> Trivia => this._trivia;

        /// <summary>
        /// Initializes this token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="range"></param>
        public Token ( String id, String raw, Object value, TokenTypeT type, SourceRange range )
        {
            this.Id       = id ?? throw new ArgumentNullException ( nameof ( id ) );
            this.Raw      = raw ?? throw new ArgumentNullException ( nameof ( raw ) );
            this.Value    = value;
            this.Type     = type;
            this.Range    = range;
            this.IsTrivia = false;
            this._trivia  = Array.Empty<Token<TokenTypeT>> ( );
        }

        /// <summary>
        /// Initializes this token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="range"></param>
        /// <param name="isTrivia"></param>
        public Token ( String id, String raw, Object value, TokenTypeT type, SourceRange range, Boolean isTrivia ) : this ( id, raw, value, type, range )
        {
            this.IsTrivia = isTrivia;
        }

        /// <summary>
        /// Initializes this token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="range"></param>
        /// <param name="trivia"></param>
        public Token ( String id, String raw, Object value, TokenTypeT type, SourceRange range, Token<TokenTypeT>[] trivia ) : this ( id, raw, value, type, range )
        {
            this._trivia = trivia ?? throw new ArgumentNullException ( nameof ( trivia ) );
        }

        /// <summary>
        /// Initializes this token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="range"></param>
        /// <param name="isTrivia"></param>
        /// <param name="trivia"></param>
        public Token ( String id, String raw, Object value, TokenTypeT type, SourceRange range, Boolean isTrivia, Token<TokenTypeT>[] trivia ) : this ( id, raw, value, type, range )
        {
            this.IsTrivia = isTrivia;
            this._trivia  = trivia ?? throw new ArgumentNullException ( nameof ( trivia ) );
        }

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) =>
            obj is Token<TokenTypeT> && this.Equals ( ( Token<TokenTypeT> ) obj );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( Token<TokenTypeT> other ) =>
            this.Id == other.Id
            && this.Raw == other.Raw
            && EqualityComparer<Object>.Default.Equals ( this.Value, other.Value )
            && EqualityComparer<TokenTypeT>.Default.Equals ( this.Type, other.Type )
            && this.Range.Equals ( other.Range )
            && this.IsTrivia == other.IsTrivia
            && this._trivia.Equals ( other._trivia );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -1868424698;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Id );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Raw );
            hashCode = hashCode * -1521134295 + EqualityComparer<Object>.Default.GetHashCode ( this.Value );
            hashCode = hashCode * -1521134295 + EqualityComparer<TokenTypeT>.Default.GetHashCode ( this.Type );
            hashCode = hashCode * -1521134295 + EqualityComparer<SourceRange>.Default.GetHashCode ( this.Range );
            hashCode = hashCode * -1521134295 + this.IsTrivia.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<Token<TokenTypeT>[]>.Default.GetHashCode ( this._trivia );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="token1"></param>
        /// <param name="token2"></param>
        /// <returns></returns>
        public static Boolean operator == ( Token<TokenTypeT> token1, Token<TokenTypeT> token2 ) => token1.Equals ( token2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="token1"></param>
        /// <param name="token2"></param>
        /// <returns></returns>
        public static Boolean operator != ( Token<TokenTypeT> token1, Token<TokenTypeT> token2 ) => !( token1 == token2 );

        #endregion Generated Code
    }
}
