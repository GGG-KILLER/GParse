using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Common.IO;
using GParse.Common.Lexing;

namespace GParse.Parsing
{
    public class TokenManager
    {
        /// <summary>
        /// The definition of a token
        /// </summary>
        private struct TokenDef
        {
            /// <summary>
            /// The ID of a token
            /// </summary>
            public String ID;

            /// <summary>
            /// The raw token
            /// </summary>
            public String Raw;

            /// <summary>
            /// The type of the token
            /// </summary>
            public TokenType Type;

            /// <summary>
            /// Whether a separator is required after this token
            /// </summary>
            public Boolean SeparatorReq;

            /// <summary>
            /// The function that filters whether a char is
            /// consitedered a separator
            /// </summary>
            public Func<Char, Boolean> SeparatorFilter;
        }

        /// <summary>
        /// The token dictionary
        /// </summary>
        private readonly Dictionary<String, TokenDef> _tokens = new Dictionary<String, TokenDef> ( );

        /// <summary>
        /// Adds a token to the list
        /// </summary>
        /// <param name="ID">The ID of the token</param>
        /// <param name="Raw">The Raw form of the token</param>
        /// <param name="Type">
        /// The <see cref="TokenType" /> of the token
        /// </param>
        /// <param name="Filter"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public TokenManager AddToken ( String ID, String Raw, TokenType Type, Func<Char, Boolean> Filter = null )
        {
            if ( this._tokens.ContainsKey ( Raw ) )
                throw new InvalidOperationException ( $"Duplicated token {Raw}" );

            this._tokens.Add ( Raw, new TokenDef
            {
                ID = ID,
                Raw = Raw,
                Type = Type,
                SeparatorReq = Filter != null,
                SeparatorFilter = Filter
            } );
            return this;
        }

        /// <summary>
        /// Removes a token from the list
        /// </summary>
        /// <param name="Raw">The raw contents of the token</param>
        public TokenManager RemoveToken ( String Raw )
        {
            this._tokens.Remove ( Raw );
            return this;
        }

        /// <summary>
        /// Clears the list of tokens
        /// </summary>
        public TokenManager Clear ( )
        {
            this._tokens.Clear ( );
            return this;
        }

        /// <summary>
        /// Reads a token from the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public Token ReadToken ( SourceCodeReader reader )
        {
            SourceLocation start = reader.Location;
            foreach ( TokenDef def in this._tokens.Values.OrderByDescending ( tok => tok.Raw.Length ) )
            {
                if ( reader.IsNext ( def.Raw ) )
                {
                    if ( def.SeparatorReq && !reader.EOF ( ) && !def.SeparatorFilter ( ( Char ) reader.Peek ( def.Raw.Length ) ) )
                        throw new LexException ( "Failed to find separator for this token.",
                            start );

                    reader.Advance ( def.Raw.Length );
                    return new Token ( def.ID, def.Raw, def.Raw, def.Type, start.To ( reader.Location ) );
                }
            }

            throw new LexException ( "No registered tokens found.", start );
        }

        /// <summary>
        /// Attempts to read a token from the reader
        /// </summary>
        /// <param name="reader">The string reader</param>
        /// <param name="Token">The token to output</param>
        /// <returns>Whether a token was read or not</returns>
        public Boolean TryReadToken ( SourceCodeReader reader, out Token Token )
        {
            SourceLocation start = reader.Location;
            foreach ( TokenDef def in this._tokens.Values.OrderByDescending ( tok => tok.Raw.Length ) )
            {
                if ( reader.IsNext ( def.Raw ) )
                {
                    if ( def.SeparatorReq && !reader.EOF ( ) && !def.SeparatorFilter ( ( Char ) reader.Peek ( def.Raw.Length ) ) )
                        break;

                    reader.Advance ( def.Raw.Length );
                    Token = new Token ( def.ID, def.Raw, def.Raw, def.Type, start.To ( reader.Location ) );
                    return true;
                }
            }

            Token = null;
            return false;
        }
    }
}
