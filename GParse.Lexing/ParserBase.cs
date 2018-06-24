using System;
using System.Collections.Generic;
using GParse.Common;
using GParse.Common.Errors;

namespace GParse.Parsing
{
    public class ParserBase
    {
        protected readonly List<Token> TokenList;
        protected Int32 Position;

        public ParserBase ( IEnumerable<Token> tokens )
        {
            this.TokenList = new List<Token> ( tokens );
            this.Position = 0;
        }

        public ParserBase ( LexerBase lexer ) : this ( lexer.Lex ( ) )
        {
        }

        /// <summary>
        /// Peeks at the next token in the list
        /// </summary>
        /// <param name="offset">Offset from the current position</param>
        /// <returns></returns>
        public Token Peek ( Int32 offset = 0 )
        {
            var idx = Math.Max ( Math.Min ( this.TokenList.Count - 1, this.Position + offset ), 0 );
            return this.TokenList[idx];
        }

        /// <summary>
        /// Retrieves the next token in the list
        /// </summary>
        /// <param name="tokenList">
        /// A <see cref="List{T}" /> may also be passed so that
        /// the token is automagically added to the list (for
        /// parsers that need to store tokens)
        /// </param>
        /// <returns></returns>
        public T Get<T> ( IList<T> tokenList = null ) where T : Token
        {
            Token token = this.Peek ( );
            this.Position = Math.Min ( this.Position + 1, this.TokenList.Count - 1 );
            tokenList?.Add ( ( T ) token );
            return ( T ) token;
        }

        /// <summary>
        /// Returns the starting location of the last token consumed.
        /// </summary>
        /// <returns></returns>
        public SourceLocation GetLocation ( )
        {
            Token tok = this.Peek ( -1 );
            return tok.Range.Start;
        }

        /// <summary>
        /// Checks whether the next unconsumed token is of the ID provided
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Boolean NextIs ( String ID )
        {
            return this.Peek ( ).ID == ID;
        }

        /// <summary>
        /// Checks whether the next unconsumed token is of the
        /// Type provided
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public Boolean NextIs ( TokenType Type )
        {
            return this.Peek ( ).Type == Type;
        }

        /// <summary>
        /// Checks whether the next unconsumed token has both the
        /// Type and ID provided
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Boolean NextIs ( TokenType Type, String ID )
        {
            return this.NextIs ( ID ) && this.NextIs ( Type );
        }

        /// <summary>
        /// Consumes the next token if it matches the ID. Returns
        /// whether the operation was succesful
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="tok"></param>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public Boolean Consume<T> ( String ID, out T tok, IList<T> tokenList = null ) where T : Token
        {
            if ( this.NextIs ( ID ) )
            {
                tok = this.Get<T> ( );
                tokenList?.Add ( tok );
                return true;
            }
            tok = null;
            return false;
        }

        /// <summary>
        /// Consumes the next token if it matches the Type.
        /// Returns whether the operation was succesful
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="tok"></param>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public Boolean Consume<T> ( TokenType Type, out T tok, IList<T> tokenList = null ) where T : Token
        {
            if ( this.NextIs ( Type ) )
            {
                tok = this.Get<T> ( );
                tokenList?.Add ( tok );
                return true;
            }
            tok = null;
            return false;
        }

        /// <summary>
        /// Consumes the next token if it matches the ID and Type.
        /// Returns whether the operation was succesful
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="ID"></param>
        /// <param name="tok"></param>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public Boolean Consume<T> ( TokenType Type, String ID, out T tok, IList<T> tokenList = null ) where T : Token
        {
            if ( this.NextIs ( Type, ID ) )
            {
                tok = this.Get<T> ( );
                tokenList?.Add ( tok );
                return true;
            }
            tok = null;
            return false;
        }

        /// <summary>
        /// Throws an exception if the next unconsumed token
        /// doesn't matches the provided ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public T Expect<T> ( String ID, IList<T> tokenList = null ) where T : Token
        {
            if ( !this.NextIs ( ID ) )
                throw new ParseException ( this.GetLocation ( ), $"Expected a token with ID `{ID}` but got one with `{this.Peek ( ).ID}` instead." );
            return this.Get ( tokenList );
        }

        /// <summary>
        /// Throws an exception if the next unconsumed token ID is
        /// not in the list of IDs passed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IDs"></param>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public T Expect<T> ( String[] IDs, IList<T> tokenList = null ) where T : Token
        {
            foreach ( var ID in IDs )
                if ( this.NextIs ( ID ) )
                    return this.Get ( tokenList );

            throw new ParseException ( this.GetLocation ( ), $"Expected a token with the ID in the list {{`{String.Join ( "`, `", IDs )}`}} but got one with `{this.Peek ( ).ID}` instead." );
        }

        /// <summary>
        /// Throws an exception if the next unconsumed token
        /// doesn't matches the provided Type
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public T Expect<T> ( TokenType Type, IList<T> tokenList = null ) where T : Token
        {
            if ( !this.NextIs ( Type ) )
                throw new ParseException ( this.GetLocation ( ), $"Expected a token with ID `{Type}` but got one with `{this.Peek ( ).Type}` instead." );
            return this.Get ( tokenList );
        }

        /// <summary>
        /// Throws an exception if the next unconsumed token ID is
        /// not in the list of IDs passed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Types"></param>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public T Expect<T> ( TokenType[] Types, IList<T> tokenList = null ) where T : Token
        {
            foreach ( TokenType Type in Types )
                if ( this.NextIs ( Type ) )
                    return this.Get ( tokenList );

            throw new ParseException ( this.GetLocation ( ), $"Expected a token with the Type in the list {{`{String.Join ( "`, `", Types )}`}} but got one with `{this.Peek ( ).Type}` instead." );
        }

        /// <summary>
        /// Throws an exception if the next unconsumed token
        /// doesn't matches the provided Type and ID
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="ID"></param>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        public T Expect<T> ( TokenType Type, String ID, IList<T> tokenList = null ) where T : Token
        {
            if ( !this.NextIs ( Type, ID ) )
                throw new ParseException ( this.GetLocation ( ), $"Expected a token with ID `{ID}` and Type `{Type}` but got one with ID `{this.Peek ( ).ID}` and Type `{this.Peek ( ).Type}`." );
            return this.Get ( tokenList );
        }
    }
}
