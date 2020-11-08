using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        /// The raw textual value of the token.
        /// </summary>
        public String? Text { get; }

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
        /// The trivia this token contains.
        /// </summary>
        public ImmutableArray<Token<TokenTypeT>> Trivia { get; }

        /// <summary>
        /// Initializes a new token.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        /// <param name="isTrivia"><inheritdoc cref="IsTrivia" path="/summary"/></param>
        /// <param name="trivia"><inheritdoc cref="Trivia" path="/summary"/></param>
        /// <param name="value"><inheritdoc cref="Value" path="/summary"/></param>
        /// <param name="text"><inheritdoc cref="Text" path="/summary"/></param>
        public Token (
            String id,
            TokenTypeT type,
            Range<Int32> range,
            Boolean isTrivia,
            IEnumerable<Token<TokenTypeT>> trivia,
            Object? value,
            String? text )
        {
            this.Id = id ?? throw new ArgumentNullException ( nameof ( id ) );
            this.Text = text;
            this.Value = value;
            this.Type = type;
            this.Range = range;
            this.IsTrivia = isTrivia;
            this.Trivia = ( trivia ?? throw new ArgumentNullException ( nameof ( trivia ) ) ).ToImmutableArray ( );
        }

        /// <summary>
        /// Initializes a new token.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        /// <param name="isTrivia"><inheritdoc cref="IsTrivia" path="/summary"/></param>
        /// <param name="trivia"><inheritdoc cref="Trivia" path="/summary"/></param>
        /// <param name="value"><inheritdoc cref="Value" path="/summary"/></param>
        public Token (
            String id,
            TokenTypeT type,
            Range<Int32> range,
            Boolean isTrivia,
            IEnumerable<Token<TokenTypeT>> trivia,
            Object? value )
            : this ( id, type, range, isTrivia, trivia, value, null )
        {
        }

        /// <summary>
        /// Initializes a new token.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        /// <param name="isTrivia"><inheritdoc cref="IsTrivia" path="/summary"/></param>
        /// <param name="trivia"><inheritdoc cref="Trivia" path="/summary"/></param>
        public Token (
            String id,
            TokenTypeT type,
            Range<Int32> range,
            Boolean isTrivia,
            IEnumerable<Token<TokenTypeT>> trivia )
            : this ( id, type, range, isTrivia, trivia, null, null )
        {
        }

        /// <summary>
        /// Initializes a new token.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        /// <param name="isTrivia"><inheritdoc cref="IsTrivia" path="/summary"/></param>
        /// <param name="value"><inheritdoc cref="Value" path="/summary"/></param>
        /// <param name="text"><inheritdoc cref="Text" path="/summary"/></param>
        public Token (
            String id,
            TokenTypeT type,
            Range<Int32> range,
            Boolean isTrivia,
            Object? value,
            String? text )
            : this ( id, type, range, isTrivia, ImmutableArray<Token<TokenTypeT>>.Empty, value, text )
        {
        }

        /// <summary>
        /// Initializes a new token.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        /// <param name="isTrivia"><inheritdoc cref="IsTrivia" path="/summary"/></param>
        /// <param name="value"><inheritdoc cref="Value" path="/summary"/></param>
        public Token (
            String id,
            TokenTypeT type,
            Range<Int32> range,
            Boolean isTrivia,
            Object? value )
            : this ( id, type, range, isTrivia, ImmutableArray<Token<TokenTypeT>>.Empty, value, null )
        {
        }

        /// <summary>
        /// Initializes a new token.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        /// <param name="isTrivia"><inheritdoc cref="IsTrivia" path="/summary"/></param>
        public Token (
            String id,
            TokenTypeT type,
            Range<Int32> range,
            Boolean isTrivia )
            : this ( id, type, range, isTrivia, ImmutableArray<Token<TokenTypeT>>.Empty, null, null )
        {
        }

        /// <summary>
        /// Initializes a new non-trivia token.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        /// <param name="value"><inheritdoc cref="Value" path="/summary"/></param>
        /// <param name="text"><inheritdoc cref="Text" path="/summary"/></param>
        public Token (
            String id,
            TokenTypeT type,
            Range<Int32> range,
            Object? value,
            String? text )
            : this ( id, type, range, false, ImmutableArray<Token<TokenTypeT>>.Empty, value, text )
        {
        }

        /// <summary>
        /// Initializes a new non-trivia token.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        /// <param name="value"><inheritdoc cref="Value" path="/summary"/></param>
        public Token (
            String id,
            TokenTypeT type,
            Range<Int32> range,
            Object? value )
            : this ( id, type, range, false, ImmutableArray<Token<TokenTypeT>>.Empty, value, null )
        {
        }

        /// <summary>
        /// Initializes a new non-trivia token.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        public Token (
            String id,
            TokenTypeT type,
            Range<Int32> range )
            : this ( id, type, range, false, ImmutableArray<Token<TokenTypeT>>.Empty, null, null )
        {
        }

        #region Generated Code

        /// <inheritdoc />
        public override Boolean Equals ( Object? obj ) =>
            obj is Token<TokenTypeT> token && this.Equals ( token );

        /// <inheritdoc />
        public Boolean Equals ( Token<TokenTypeT>? other ) =>
            other is not null
            && this.Id == other.Id
            && this.Text == other.Text
            && EqualityComparer<Object?>.Default.Equals ( this.Value, other.Value )
            && EqualityComparer<TokenTypeT>.Default.Equals ( this.Type, other.Type )
            && this.Range.Equals ( other.Range )
            && this.IsTrivia == other.IsTrivia
            && this.Trivia.SequenceEqual ( other.Trivia );

        /// <inheritdoc />
        [SuppressMessage ( "CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Suppression is valid for some target frameworks." )]
        [SuppressMessage ( "Style", "IDE0070:Use 'System.HashCode'", Justification = "We have to maintain consistent behavior between all target frameworks." )]
        public override Int32 GetHashCode ( )
        {
            var hashCode = -839334579;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Id );
#pragma warning disable CS8604 // Possible null reference argument.
            hashCode = hashCode * -1521134295 + EqualityComparer<String?>.Default.GetHashCode ( this.Text );
            hashCode = hashCode * -1521134295 + EqualityComparer<Object?>.Default.GetHashCode ( this.Value );
#pragma warning restore CS8604 // Possible null reference argument.
            hashCode = hashCode * -1521134295 + EqualityComparer<TokenTypeT>.Default.GetHashCode ( this.Type );
            hashCode = hashCode * -1521134295 + this.Range.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.IsTrivia.GetHashCode ( );
            foreach ( Token<TokenTypeT> trivia in this.Trivia )
                hashCode = hashCode * -1521134295 + trivia.GetHashCode ( );
            return hashCode;
        }

        /// <inheritdoc />
        public static Boolean operator == ( Token<TokenTypeT> left, Token<TokenTypeT> right ) =>
            ReferenceEquals ( left, right )
            || ( left is not null && right is not null && left.Equals ( right ) );

        /// <inheritdoc />
        public static Boolean operator != ( Token<TokenTypeT> left, Token<TokenTypeT> right ) => !( left == right );

        #endregion Generated Code
    }
}