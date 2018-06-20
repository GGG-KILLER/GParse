using System;
using System.Collections.Generic;

namespace GParse.Common.IO
{
    public class SourceCodeReader
    {
        /// <summary>
        /// The string
        /// </summary>
        private readonly String _str;

        private readonly Stack<SourceLocation> _locationStack = new Stack<SourceLocation> ( );

        /// <summary>
        /// Initializes a string reader
        /// </summary>
        /// <param name="str"></param>
        public SourceCodeReader ( String str )
        {
            this._str = str ?? throw new ArgumentNullException ( nameof ( str ) );
            this.Length = str.Length;
            this.Position = 0;
            this.Line = 0;
            this.Column = 0;
        }

        /// <summary>
        /// Current source code column
        /// </summary>
        public Int32 Column { get; private set; }

        /// <summary>
        /// The length of the string
        /// </summary>
        public Int32 Length { get; }

        /// <summary>
        /// Current source code line
        /// </summary>
        public Int32 Line { get; private set; }

        /// <summary>
        /// Current <see cref="SourceLocation" /> of the reader
        /// </summary>
        public SourceLocation Location => new SourceLocation ( this.Line, this.Column, this.Position );

        /// <summary>
        /// Current position of the reader
        /// </summary>
        public Int32 Position { get; private set; }

        /// <summary>
        /// Advances by <paramref name="offset" /> chars
        /// </summary>
        /// <param name="offset"></param>
        public void Advance ( Int32 offset )
        {
            if ( offset < 0 )
                throw new ArgumentException ( "Offset must be positive", nameof ( offset ) );

            if ( this.Position + offset > this.Length )
                return;

            var end = this.Position + offset;
            while ( this.Position < end )
            {
                this.Position++;
                if ( this.Position < this.Length )
                {
                    this.Column++;

                    if ( this._str[this.Position] == '\n' )
                    {
                        this.Line++;
                        this.Column = 0;
                    }
                }
            }
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
        /// Returns whether the next character in the "stream" is <paramref name="ch" />.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public Boolean IsNext ( Char ch )
            => !this.EOF ( ) && this.Peek ( ) == ch;

        /// <summary>
        /// Returns whether the character at
        /// <paramref name="offset" /> in the "stream" is <paramref name="ch" />.
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsNext ( Char ch, Int32 offset )
            => !this.EOF ( ) && this.Peek ( offset ) == ch;

        /// <summary>
        /// Confirms wether or not the next
        /// <paramref name="str" />.Length chars are <paramref name="str" />
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Boolean IsNext ( String str )
        {
            var len = str.Length;
            if ( this.Position + len - 1 >= this.Length )
                return false;

            for ( var idx = 0; idx < len; idx++ )
            {
                if ( this.Peek ( idx ) != str[idx] )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Confirms wether or not the next
        /// <paramref name="str" />.Length chars on the offset
        /// <paramref name="offset" /> are <paramref name="str" />
        /// </summary>
        /// <param name="str"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsNext ( String str, Int32 offset )
        {
            var len = str.Length + offset;
            if ( this.Position + len - 1 + offset >= this.Length )
                return false;

            for ( var idx = offset; idx < len; idx++ )
            {
                if ( this.Peek ( idx ) != str[idx] )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the offset of <paramref name="ch" />
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public Int32 OffsetOf ( Char ch )
        {
            var idx = this._str.IndexOf ( ch, this.Position );
            return idx != -1 ? idx - this.Position : -1;
        }

        /// <summary>
        /// Returns the offset of <paramref name="str" />
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Int32 OffsetOf ( String str )
        {
            var idx = this._str.IndexOf ( str, this.Position );
            return idx != -1 ? idx - this.Position : -1;
        }

        /// <summary>
        /// Returns a character without advancing on the string
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Int32 Peek ( Int32 offset = 0 )
        {
            if ( offset < 0 )
                throw new ArgumentException ( "Negative offsets not supported.", nameof ( offset ) );

            return this.Position + offset < this.Length ? this._str[this.Position + offset] : -1;
        }

        /// <summary>
        /// Returns a string of <paramref name="length" /> size
        /// without advancing on the string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public String PeekString ( Int32 length )
        {
            if ( length < 0 )
                throw new ArgumentException ( "Length must be positive.", nameof ( length ) );
            return this.Position + length <= this.Length ? this._str.Substring ( this.Position, length ) : null;
        }

        /// <summary>
        /// Returns a character while advancing on the string
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Int32 ReadChar ( Int32 offset = 0 )
        {
            if ( offset < 0 )
                throw new ArgumentException ( "Negative offsets not supported.", nameof ( offset ) );

            if ( this.Position + offset < this.Length )
            {
                try
                {
                    this.Advance ( offset );
                    return this._str[this.Position];
                }
                finally
                {
                    this.Advance ( 1 );
                }
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Reads a string of <paramref name="length" /> length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public String ReadString ( Int32 length )
        {
            if ( length < 0 )
                throw new ArgumentException ( "Length must be positive.", nameof ( length ) );

            if ( length == 0 )
                return "";

            if ( this.Position + length <= this.Length )
            {
                var start = this.Position;
                this.Advance ( length );
                return this._str.Substring ( start, length );
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Reads until <paramref name="ch" /> is found
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public String ReadStringUntil ( Char ch )
        {
            var length = this.OffsetOf ( ch );
            return length != -1 ? this.ReadString ( length ) : null;
        }

        /// <summary>
        /// Reads until <paramref name="str" /> is found
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public String ReadStringUntil ( String str )
        {
            var length = this.OffsetOf ( str );
            return length != -1 ? this.ReadString ( length ) : null;
        }

        /// <summary>
        /// Read until <paramref name="ch" /> is not found
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public String ReadStringUntilNot ( Char ch )
        {
            var length = 0;
            while ( this.Peek ( length ) == ch )
                length++;
            return this.ReadString ( length );
        }

        /// <summary>
        /// Reads while <paramref name="filter" /> returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringWhile ( Func<Char, Boolean> filter )
        {
            if ( filter == null )
                throw new ArgumentNullException ( nameof ( filter ) );

            var length = 0;
            while ( this.Peek ( length ) != -1 && filter ( ( Char ) this.Peek ( length ) ) )
                length++;
            return this.ReadString ( length );
        }

        /// <summary>
        /// Reads until <paramref name="filter" /> returns false
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringWhileNot ( Func<Char, Boolean> filter )
        {
            if ( filter == null )
                throw new ArgumentNullException ( nameof ( filter ) );

            var length = 0;
            while ( this.Peek ( length ) != -1 && filter ( ( Char ) this.Peek ( length ) ) )
                length++;
            return this.ReadString ( length );
        }

        /// <summary>
        /// Reads a line
        /// </summary>
        /// <returns></returns>
        public String ReadLine ( )
        {
            // Expect CR + LF
            var lineend = this.OffsetOf ( "\r\n" );

            // Fallback to LF if no CR + LF
            if ( lineend == -1 )
                lineend = this.OffsetOf ( '\n' );

            // Fallback to EOF if no LF
            if ( lineend == -1 )
                lineend = this.Length - this.Position;

            return this.ReadString ( lineend );
        }

        /// <summary>
        /// Resets the Source Code reader
        /// </summary>
        public void Reset ( )
        {
            this.Line = 0;
            this.Column = 0;
            this.Position = 0;
        }

        /// <summary>
        /// Returns to <paramref name="location" />
        /// </summary>
        /// <param name="location">
        /// todo: describe save parameter on Rewind
        /// </param>
        public void Rewind ( SourceLocation location )
        {
            if ( location.Byte < 0 || location.Line < 0 || location.Column < 0 )
                throw new Exception ( "Invalid rewind position." );
            this.Position = location.Byte;
            this.Line = location.Line;
            this.Column = location.Column;
        }

        /// <summary>
        /// Saves the current position of the reader in a stack
        /// </summary>
        /// <returns></returns>
        public void Save ( )
        {
            this._locationStack.Push ( this.Location );
        }

        /// <summary>
        /// Discards the last saved position
        /// </summary>
        public void DiscardSave ( )
        {
            this._locationStack.Pop ( );
        }

        /// <summary>
        /// Returns to the last position in the saved positions
        /// stack and returns the position
        /// </summary>
        public SourceLocation LoadSave ( )
        {
            SourceLocation last = this._locationStack.Pop ( );
            this.Line = last.Line;
            this.Column = last.Column;
            this.Position = last.Byte;
            return last;
        }

        /// <summary>
        /// Saved position of the reader
        /// </summary>
        public struct SavePoint
        {
            public Int32 Line, Column, Position;

            public override String ToString ( )
            {
                return $"{{ Line: {this.Line}, Column: {this.Column}, Position: {this.Position}}}";
            }
        }

        public override String ToString ( )
        {
            return this._str.Substring ( this.Position );
        }
    }
}
