using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GParse.Lexing
{
    /// <summary>
    /// Defines the interface of a token reader
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public interface ITokenReader<TTokenType>
        where TTokenType : notnull
    {
        /// <summary>
        /// The current token index.
        /// </summary>
        public Int32 Position { get; }

        /// <summary>
        /// The total token count.
        /// </summary>
        public Int32 Length { get; }

        /// <summary>
        /// Consumes the token at <paramref name="offset" /> in the stream without moving
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        Token<TTokenType> Lookahead(Int32 offset = 0);

        /// <summary>
        /// Consumes the next token in the stream
        /// </summary>
        /// <returns></returns>
        Token<TTokenType> Consume();

        /// <summary>
        /// Skips a certain amount of tokens
        /// </summary>
        /// <param name="count">The amount of tokens to skip</param>
        void Skip(Int32 count);

        #region IsAhead

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TTokenType}.Type" /> equal to <paramref name="tokenType" />
        /// </summary>
        /// <param name="tokenType">The wanted type</param>
        /// <param name="offset">The offset</param>
        /// <returns></returns>
        Boolean IsAhead(TTokenType tokenType, Int32 offset = 0);

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TTokenType}.Type" /> in the given <paramref name="tokenTypes" />
        /// </summary>
        /// <param name="tokenTypes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead(IEnumerable<TTokenType> tokenTypes, Int32 offset = 0);

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TTokenType}.Id" /> equal to <paramref name="id" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead(String id, Int32 offset = 0);

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TTokenType}.Id" /> in the given <paramref name="ids" />
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead(IEnumerable<String> ids, Int32 offset = 0);

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TTokenType}.Type" /> equal to <paramref name="tokenType" /> and the
        /// <see cref="Token{TTokenType}.Id" /> equal to <paramref name="id" />
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead(TTokenType tokenType, String id, Int32 offset = 0);

        /// <summary>
        /// Whether the character at a given <paramref name="offset" /> from the first unread character
        /// has the <see cref="Token{TTokenType}.Type" /> in the given <paramref name="tokenTypes" /> and
        /// has the <see cref="Token{TTokenType}.Id" /> in the given <paramref name="ids" />
        /// </summary>
        /// <param name="tokenTypes"></param>
        /// <param name="ids"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead(IEnumerable<TTokenType> tokenTypes, IEnumerable<String> ids, Int32 offset = 0);

        #endregion IsAhead

        #region Accept

        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="id" />
        /// </summary>
        /// <param name="id">The ID to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept(String id, [NotNullWhen(true)] out Token<TTokenType>? token);

        /// <summary>
        /// Only advances in the stream if the token has one of the required <paramref name="ids" />
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Accept(IEnumerable<String> ids, [NotNullWhen(true)] out Token<TTokenType>? token);

        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="type" />
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept(TTokenType type, [NotNullWhen(true)] out Token<TTokenType>? token);

        /// <summary>
        /// Only advances in the stream if the token has one of the required <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Accept(IEnumerable<TTokenType> types, [NotNullWhen(true)] out Token<TTokenType>? token);

        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="id" /> and
        /// <paramref name="type" />
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <param name="id">The ID to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept(TTokenType type, String id, [NotNullWhen(true)] out Token<TTokenType>? token);

        /// <summary>
        /// Only advances in the stream if the token has the one of the required <paramref name="ids" />
        /// and <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <param name="ids"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Accept(IEnumerable<TTokenType> types, IEnumerable<String> ids, [NotNullWhen(true)] out Token<TTokenType>? token);

        #endregion Accept

        /// <summary>
        /// Restores the position to the provided <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The position to restore to.</param>
        void Restore(Int32 position);
    }
}