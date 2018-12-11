using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    /// <summary>
    /// Defines the interface of a token reader
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public interface ITokenReader<TokenTypeT> : IEnumerable<Token<TokenTypeT>>
    {
        /// <summary>
        /// The location of the token reader
        /// </summary>
        SourceLocation Location { get; }

        /// <summary>
        /// Consumes the token at <paramref name="offset" /> in
        /// the stream without moving
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        Token<TokenTypeT> Lookahead ( Int32 offset = 0 );

        /// <summary>
        /// Consumes the next token in the stream
        /// </summary>
        /// <returns></returns>
        Token<TokenTypeT> Consume ( );

        /// <summary>
        /// Skips a certain amount of tokens
        /// </summary>
        /// <param name="count">The amount of tokens to skip</param>
        void Skip ( Int32 count );

        #region IsAhead

        /// <summary>
        /// Whether the character at a given
        /// <paramref name="offset" /> from the first unread
        /// character has the
        /// <see cref="Token{TokenTypeT}.Type" /> equal to <paramref name="tokenType" />
        /// </summary>
        /// <param name="tokenType">The wanted type</param>
        /// <param name="offset">The offset</param>
        /// <returns></returns>
        Boolean IsAhead ( TokenTypeT tokenType, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given
        /// <paramref name="offset" /> from the first unread
        /// character has the
        /// <see cref="Token{TokenTypeT}.Type" /> in the given <paramref name="tokenTypes" />
        /// </summary>
        /// <param name="tokenTypes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given
        /// <paramref name="offset" /> from the first unread
        /// character has the <see cref="Token{TokenTypeT}.ID" />
        /// equal to <paramref name="ID" />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( String ID, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given
        /// <paramref name="offset" /> from the first unread
        /// character has the <see cref="Token{TokenTypeT}.ID" />
        /// in the given <paramref name="ids" />
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( IEnumerable<String> ids, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given
        /// <paramref name="offset" /> from the first unread
        /// character has the
        /// <see cref="Token{TokenTypeT}.Type" /> equal to
        /// <paramref name="tokenType" /> and the
        /// <see cref="Token{TokenTypeT}.ID" /> equal to <paramref name="id" />
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( TokenTypeT tokenType, String id, Int32 offset = 0 );

        /// <summary>
        /// Whether the character at a given
        /// <paramref name="offset" /> from the first unread
        /// character has the
        /// <see cref="Token{TokenTypeT}.Type" /> in the given
        /// <paramref name="tokenTypes" /> and has the
        /// <see cref="Token{TokenTypeT}.ID" /> in the given <paramref name="ids" />
        /// </summary>
        /// <param name="tokenTypes"></param>
        /// <param name="ids"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Boolean IsAhead ( IEnumerable<TokenTypeT> tokenTypes, IEnumerable<String> ids, Int32 offset = 0 );

        #endregion IsAhead

        #region Accept

        /// <summary>
        /// Only advances in the stream if the token has the
        /// required <paramref name="ID" />
        /// </summary>
        /// <param name="ID">The ID to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept ( String ID, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has one of
        /// the required <paramref name="IDs" />
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Accept ( IEnumerable<String> IDs, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has the
        /// required <paramref name="ID" />
        /// </summary>
        /// <param name="ID">The ID to check for</param>
        /// <returns></returns>
        Boolean Accept ( String ID );

        /// <summary>
        /// Only advances in the stream if the token has one of
        /// the required <paramref name="IDs" />
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        Boolean Accept ( IEnumerable<String> IDs );

        /// <summary>
        /// Only advances in the stream if the token has the
        /// required <paramref name="type" />
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept ( TokenTypeT type, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has one of
        /// the required <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Accept ( IEnumerable<TokenTypeT> types, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has the
        /// required <paramref name="type" />
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <returns></returns>
        Boolean Accept ( TokenTypeT type );

        /// <summary>
        /// Only advances in the stream if the token has one of
        /// the required <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        Boolean Accept ( IEnumerable<TokenTypeT> types );

        /// <summary>
        /// Only advances in the stream if the token has the
        /// required <paramref name="ID" /> and <paramref name="type" />
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <param name="ID">The ID to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept ( TokenTypeT type, String ID, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has the one
        /// of the required <paramref name="IDs" /> and <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <param name="IDs"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Boolean Accept ( IEnumerable<TokenTypeT> types, IEnumerable<String> IDs, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has the
        /// required <paramref name="ID" /> and <paramref name="type" />
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <param name="ID">The ID to check for</param>
        /// <returns></returns>
        Boolean Accept ( TokenTypeT type, String ID );

        /// <summary>
        /// Only advances in the stream if the token has the one
        /// of the required <paramref name="IDs" /> and <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        Boolean Accept ( IEnumerable<TokenTypeT> types, IEnumerable<String> IDs );

        #endregion Accept

        #region Expect

        /// <summary>
        /// Throws an exception if the next token in the stream
        /// does not have the <paramref name="ID" /> required
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        Token<TokenTypeT> Expect ( String ID );

        /// <summary>
        /// Throws an exception if the next token in the stream
        /// does not have one of the required <paramref name="IDs" />
        /// </summary>
        /// <param name="IDs"></param>
        /// <returns></returns>
        Token<TokenTypeT> Expect ( IEnumerable<String> IDs );

        /// <summary>
        /// Throws an exception if the next token in the stream
        /// does not have the <paramref name="type" /> required
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Token<TokenTypeT> Expect ( TokenTypeT type );

        /// <summary>
        /// Throws an excepton if the next token in the stream
        /// does not have one of the required <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        Token<TokenTypeT> Expect ( IEnumerable<TokenTypeT> types );

        /// <summary>
        /// Throws an exception if the next token in the stream
        /// does not have the <paramref name="ID" /> and
        /// <paramref name="type" /> required
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        Token<TokenTypeT> Expect ( TokenTypeT type, String ID );

        /// <summary>
        /// Throws an exception if the next token in the stream
        /// does not have one of the required
        /// <paramref name="IDs" /> or <paramref name="types" />
        /// </summary>
        /// <param name="types"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        Token<TokenTypeT> Expect ( IEnumerable<TokenTypeT> types, IEnumerable<String> IDs );

        #endregion Expect
    }
}
