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
    public interface ITokenReader<TokenTypeT> : IEnumerable<Token<TokenTypeT>> where TokenTypeT : Enum
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

        #region Accept

        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="ID"/>
        /// </summary>
        /// <param name="ID">The ID to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept ( String ID, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="ID"/>
        /// </summary>
        /// <param name="ID">The ID to check for</param>
        /// <returns></returns>
        Boolean Accept ( String ID );


        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept ( TokenTypeT type, out Token<TokenTypeT> token );


        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type to check for</param>
        /// <returns></returns>
        Boolean Accept ( TokenTypeT type );


        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="ID"/> and <paramref name="type"/>
        /// </summary>
        /// <param name="ID">The ID to check for</param>
        /// <param name="type">The type to check for</param>
        /// <param name="token">The accepted token</param>
        /// <returns></returns>
        Boolean Accept ( String ID, TokenTypeT type, out Token<TokenTypeT> token );

        /// <summary>
        /// Only advances in the stream if the token has the required <paramref name="ID"/> and <paramref name="type"/>
        /// </summary>
        /// <param name="ID">The ID to check for</param>
        /// <param name="type">The type to check for</param>
        /// <returns></returns>
        Boolean Accept ( String ID, TokenTypeT type );

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
        /// does not have the <paramref name="type" /> required
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Token<TokenTypeT> Expect ( TokenTypeT type );

        /// <summary>
        /// Throws an exception if the next token in the stream
        /// does not have the <paramref name="ID" /> and
        /// <paramref name="type" /> required
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Token<TokenTypeT> Expect ( String ID, TokenTypeT type );

        #endregion Expect
    }
}
