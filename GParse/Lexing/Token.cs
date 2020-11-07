using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GParse.Math;

namespace GParse.Lexing
{
    /// <summary>
    /// A token outputted by the lexer.
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class Token<TokenTypeT> : IEquatable<Token<TokenTypeT>>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The ID of the token.
        /// </summary>
        public String Id { get; }

        /// <summary>
        /// The raw value of the token.
        /// </summary>
        public String Raw { get; }

        /// <summary>
        /// The value of the token.
        /// </summary>
        public Object? Value { get; }

        /// <summary>
        /// The type of the token.
        /// </summary>
        public TokenTypeT Type { get; }

        /// <summary>
        /// The range of positions the token was in.
        /// </summary>
        public Range<Int32> Range { get; }

        /// <summary>
        /// Whether this token is a piece of trivia, such as comments and/or whitespaces
        /// </summary>
        public Boolean IsTrivia { get; }

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
        public Token ( String id, String raw, Object? value, TokenTypeT type, Range<Int32> range )
        {
            this.Id = id ?? throw new ArgumentNullException ( nameof ( id ) );
            this.Raw = raw ?? throw new ArgumentNullException ( nameof ( raw ) );
            this.Value = value;
            this.Type = type;
            this.Range = range;
            this.IsTrivia = false;
            this._trivia = Array.Empty<Token<TokenTypeT>> ( );
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
        public Token ( String id, String raw, Object? value, TokenTypeT type, Range<Int32> range, Boolean isTrivia )
            : this ( id, raw, value, type, range )
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
        public Token ( String id, String raw, Object? value, TokenTypeT type, Range<Int32> range, Token<TokenTypeT>[] trivia )
            : this ( id, raw, value, type, range )
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
        public Token (
            String id,
            String raw,
            Object? value,
            TokenTypeT type,
            Range<Int32> range,
            Boolean isTrivia,
            Token<TokenTypeT>[] trivia )
            : this ( id, raw, value, type, range )
        {
            this.IsTrivia = isTrivia;
            this._trivia = trivia ?? throw new ArgumentNullException ( nameof ( trivia ) );
        }

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals ( Object? obj ) => obj is Token<TokenTypeT> token && this.Equals ( token );

        /// <inheritdoc />
        public Boolean Equals ( Token<TokenTypeT>? other ) =>
            other is not null
            && this.Id == other.Id
            && this.Raw == other.Raw
            && EqualityComparer<Object?>.Default.Equals ( this.Value, other.Value )
            && EqualityComparer<TokenTypeT>.Default.Equals ( this.Type, other.Type )
            && this.Range.Equals ( other.Range )
            && this.IsTrivia == other.IsTrivia
            && EqualityComparer<Token<TokenTypeT>[]>.Default.Equals ( this._trivia, other._trivia );

        /// <inheritdoc />
        [SuppressMessage ( "CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Suppression is valid for some target frameworks." )]
        [SuppressMessage ( "Style", "IDE0070:Use 'System.HashCode'", Justification = "We have to maintain consistent behavior between all target frameworks." )]
        public override Int32 GetHashCode ( )
        {
            var hashCode = -839334579;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Id );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Raw );
#pragma warning disable CS8604 // Possible null reference argument.
            hashCode = hashCode * -1521134295 + EqualityComparer<Object?>.Default.GetHashCode ( this.Value );
#pragma warning restore CS8604 // Possible null reference argument.
            hashCode = hashCode * -1521134295 + EqualityComparer<TokenTypeT>.Default.GetHashCode ( this.Type );
            hashCode = hashCode * -1521134295 + this.Range.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.IsTrivia.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<Token<TokenTypeT>[]>.Default.GetHashCode ( this._trivia );
            return hashCode;
        }

        /// <inheritdoc />
        public static Boolean operator == ( Token<TokenTypeT> left, Token<TokenTypeT> right ) => left.Equals ( right );

        /// <inheritdoc />
        public static Boolean operator != ( Token<TokenTypeT> left, Token<TokenTypeT> right ) => !( left == right );

        #endregion Generated Code
    }
}