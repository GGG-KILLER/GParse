using System;
using System.Collections.Generic;
using GParse.Lexing;
using GParse.Verbose.IO.Errors;

namespace GParse.Verbose.IO
{
    public class TokenReader
    {
        /// <summary>
        /// The token list
        /// </summary>
        private readonly Token[] _tokens;

        private readonly Stack<Int32> _indexStack = new Stack<Int32> ( );

        /// <summary>
        /// Initializes a string reader
        /// </summary>
        /// <param name="str"></param>
        public TokenReader ( Token[] tokens )
        {
            this._tokens = tokens ?? throw new ArgumentNullException ( nameof ( tokens ) );
            this.Length = tokens.Length;
            this.Position = 0;
        }

        /// <summary>
        /// The length of the token list
        /// </summary>
        public Int32 Length { get; }

        /// <summary>
        /// Current position of the reader
        /// </summary>
        public Int32 Position { get; private set; }

        /// <summary>
        /// Advances by <paramref name="offset" /> tokens
        /// </summary>
        /// <param name="offset"></param>
        public void Advance ( Int32 offset )
        {
            if ( this.Position + offset > this.Length )
                return;

            this.Position += offset;
        }

        /// <summary>
        /// Whether we've hit the end of the string
        /// </summary>
        /// <returns></returns>
        public Boolean EOF ( )
        {
            return this.Position == this.Length;
        }

        /// <summary>
        /// Confirms whether or not the next token has the id provided
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean IsNext ( String id )
        {
            return this.Peek ( ).ID == id;
        }

        /// <summary>
        /// Returns whether or not the next token has the type provided
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Boolean IsNext ( TokenType type )
        {
            return this.Peek ( ).Type == type;
        }

        /// <summary>
        /// Returns whether or not the next token has the id and
        /// type provided
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Boolean IsNext ( String id, TokenType type )
        {
            return this.IsNext ( id ) && this.IsNext ( type );
        }

        /// <summary>
        /// Throws an exception if the next token does not have
        /// the id provided
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Token Expect ( String ID )
        {
            if ( !this.IsNext ( ID ) )
                throw new UnexpectedTokenException ( $"Expected a token with ID {ID} but got one with {this.Peek ( ).ID}" );
            return this.Read ( );
        }

        /// <summary>
        /// Throws an exception if the next token is not of the
        /// type provided
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public Token Expect ( TokenType Type )
        {
            if ( !this.IsNext ( Type ) )
                throw new UnexpectedTokenException ( $"Expected a token with type {Type} but got one with {this.Peek ( ).Type}" );
            return this.Read ( );
        }

        /// <summary>
        /// Throws an exception if the next token is not of the id
        /// and type provided
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public Token Expect ( String ID, TokenType Type )
        {
            if ( !this.IsNext ( ID, Type ) )
                throw new UnexpectedTokenException ( $"Expected a token with ID {ID} and Type {Type} but got one with ID {this.Peek ( ).ID} and type {this.Peek ( ).Type}" );
            return this.Read ( );
        }

        /// <summary>
        /// Returns whether or not a token was consumed and also
        /// sets <paramref name="tok" /> to the consumed token, if any
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="tok"></param>
        /// <returns></returns>
        public Boolean Consume ( String ID, out Token tok )
        {
            if ( this.IsNext ( ID ) )
            {
                tok = this.Read ( );
                return true;
            }
            else
            {
                tok = null;
                return false;
            }
        }

        /// <summary>
        /// Returns whether or not a token was consumed and also
        /// sets <paramref name="tok" /> to the consumed token, if any
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="tok"></param>
        /// <returns></returns>
        public Boolean Consume ( TokenType Type, out Token tok )
        {
            if ( this.IsNext ( Type ) )
            {
                tok = this.Read ( );
                return true;
            }
            else
            {
                tok = null;
                return false;
            }
        }

        /// <summary>
        /// Returns whether or not a token was consumed and also
        /// sets <paramref name="tok" /> to the consumed token, if any
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Type"></param>
        /// <param name="tok"></param>
        /// <returns></returns>
        public Boolean Consume ( String ID, TokenType Type, out Token tok )
        {
            if ( this.IsNext ( ID, Type ) )
            {
                tok = this.Read ( );
                return true;
            }
            else
            {
                tok = null;
                return false;
            }
        }

        /// <summary>
        /// Returns the offset of the token with ID <paramref name="id" />
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Int32 OffsetOf ( String id )
        {
            for ( var i = this.Position; i < this.Length; i++ )
                if ( this._tokens[i].ID == id )
                    return this.Length - this.Position;
            return -1;
        }

        /// <summary>
        /// Returns the offset of the token with Type <paramref name="type" />
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Int32 OffsetOf ( TokenType type )
        {
            for ( var i = this.Position; i < this.Length; i++ )
                if ( this._tokens[i].Type == type )
                    return this.Length - this.Position;
            return -1;
        }

        /// <summary>
        /// Returns a character without advancing on the string
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Token Peek ( Int32 offset = 0 )
        {
            if ( offset < 0 )
                throw new ArgumentException ( "Negative offsets not supported.", nameof ( offset ) );

            return this.Position + offset < this.Length ? this._tokens[this.Position + offset] : null;
        }

        /// <summary>
        /// Returns a character while advancing on the string
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Token Read ( Int32 offset = 0 )
        {
            if ( offset < 0 )
                throw new ArgumentException ( "Negative offsets not supported.", nameof ( offset ) );

            if ( this.Position + offset < this.Length )
            {
                try
                {
                    this.Advance ( offset );
                    return this._tokens[this.Position];
                }
                finally
                {
                    this.Advance ( 1 );
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Resets the Source Code reader
        /// </summary>
        public void Reset ( )
        {
            this.Position = 0;
        }

        /// <summary>
        /// Saves the current position of the reader in a stack
        /// </summary>
        /// <returns></returns>
        public void Save ( )
        {
            this._indexStack.Push ( this.Position );
        }

        /// <summary>
        /// Discards the last saved position
        /// </summary>
        public void DiscardSave ( )
        {
            this._indexStack.Pop ( );
        }

        /// <summary>
        /// Returns to the last position in the saved positions
        /// stack and returns the position
        /// </summary>
        public Int32 Load ( )
        {
            var last = this._indexStack.Pop ( );
            return this.Position = last;
        }
    }
}
