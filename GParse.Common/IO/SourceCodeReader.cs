using System;
using System.Collections.Generic;

namespace GParse.Common.IO
{
    public class SourceCodeReader
    {
        /// <summary>
        /// The string
        /// </summary>
        private readonly String Code;

        /// <summary>
        /// Stores the locations that the user saves with <see cref="Save" />
        /// </summary>
        private readonly Stack<SourceLocation> LocationHistory = new Stack<SourceLocation> ( );

        #region Location Management

        /// <summary>
        /// Current source code line
        /// </summary>
        public Int32 Line { get; private set; }

        /// <summary>
        /// Current source code column
        /// </summary>
        public Int32 Column { get; private set; }

        /// <summary>
        /// Current position of the reader
        /// </summary>
        public Int32 Position { get; private set; }

        /// <summary>
        /// Current <see cref="SourceLocation" /> of the reader
        /// </summary>
        public SourceLocation Location => new SourceLocation ( this.Line, this.Column, this.Position );

        #endregion Location Management

        /// <summary>
        /// The amount of chars left to be read
        /// </summary>
        public Int32 ContentLeftSize { get; private set; }

        /// <summary>
        /// The length of the string that was passed to this reader
        /// </summary>
        public Int32 Length => this.Code.Length;

        /// <summary>
        /// Whether we've hit the end of the string yet
        /// </summary>
        /// <returns></returns>
        public Boolean IsAtEOF => this.Position == this.Code.Length;

        /// <summary>
        /// Whether we still have content left
        /// </summary>
        public Boolean HasContent => this.Position != this.Code.Length;

        /// <summary>
        /// Initializes a string reader
        /// </summary>
        /// <param name="str"></param>
        public SourceCodeReader ( String str )
        {
            this.Code = str ?? throw new ArgumentNullException ( nameof ( str ) );
            this.Position = 0;
            this.Line = 1;
            this.Column = 1;
            this.ContentLeftSize = this.Code.Length;
        }

        /// <summary>
        /// Advances by <paramref name="offset" /> chars
        /// </summary>
        /// <param name="offset"></param>
        public void Advance ( Int32 offset )
        {
            if ( offset < 0 )
                throw new ArgumentOutOfRangeException ( nameof ( offset ), "Offset must be positive" );
            if ( offset > this.ContentLeftSize )
                throw new ArgumentOutOfRangeException ( nameof ( offset ), "Offset is too big." );

            // Scans all characters until the offset for line
            // breaks and updates the location variables accordingly
            var end = this.Position + offset + 1;
            for ( var prev = this.Position - 1; this.Position < end; prev++, this.Position++, this.ContentLeftSize-- )
            {
                // We actually only want to increment AFTER the
                // line break, otherwise the line break itself
                // would be considered to be on the same line as
                // the actual next line
                if ( prev > -1 && this.Code[prev] == '\n' )
                {
                    this.Line++;
                    this.Column = 1;
                }
                else
                    this.Column++;
            }

            // Fix the extra iteration made before the for
            // condition check is made
            this.Position--;
            this.ContentLeftSize++;
        }

        #region IsNext Variations

        /// <summary>
        /// Returns whether the next character in the "stream" is <paramref name="ch" />.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public Boolean IsNext ( Char ch )
            => this.HasContent && this.Code[this.Position] == ch;

        /// <summary>
        /// Returns whether the character at
        /// <paramref name="offset" /> in the "stream" is <paramref name="ch" />.
        /// </summary>
        /// <param name="ch">    </param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsNext ( Char ch, Int32 offset )
        {
            if ( !this.HasContent || offset > this.ContentLeftSize )
                return false;
            return this.Code[this.Position + offset] == ch;
        }

        /// <summary>
        /// Confirms wether or not the next
        /// <paramref name="str" />.Length chars are <paramref name="str" />
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Boolean IsNext ( String str )
        {
            var len = str.Length;
            if ( len > this.ContentLeftSize )
                return false;

            for ( Int32 i1 = this.Position, i2 = 0; i2 < len; i1++, i2++ )
                if ( this.Code[i1] != str[i2] )
                    return false;
            return true;
        }

        /// <summary>
        /// Confirms wether or not the next
        /// <paramref name="str" />.Length chars on the offset
        /// <paramref name="offset" /> are <paramref name="str" />
        /// </summary>
        /// <param name="str">   </param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsNext ( String str, Int32 offset )
        {
            var len = str.Length;
            if ( len + offset > this.ContentLeftSize )
                return false;

            for ( Int32 i1 = this.Position + offset, i2 = 0; i2 < len; i1++, i2++ )
                if ( this.Code[i1] != str[i2] )
                    return false;
            return true;
        }

        #endregion IsNext Variations

        #region FindOffset

        /// <summary>
        /// Returns the offset of <paramref name="ch" />
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public Int32 FindOffset ( Char ch )
        {
            if ( this.IsAtEOF )
                return -1;
            var idx = this.Code.IndexOf ( ch, this.Position );
            return idx == -1 ? -1 : idx - this.Position;
        }

        /// <summary>
        /// Returns the offset of <paramref name="str" />
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Int32 FindOffset ( String str )
        {
            if ( this.IsAtEOF )
                return -1;
            var idx = this.Code.IndexOf ( str, this.Position );
            return idx == -1 ? -1 : idx - this.Position;
        }

        /// <summary>
        /// Finds the offset of a character that satisfies a <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Int32 FindOffset ( Predicate<Char> predicate )
        {
            if ( predicate == null )
                throw new ArgumentNullException ( nameof ( predicate ) );
            if ( this.IsAtEOF )
                return -1;

            for ( var i = this.Position; i < this.Length; i++ )
                if ( predicate ( this.Code[i] ) )
                    return i - this.Position;
            return -1;
        }

        #endregion FindOffset

        #region Peeking

        /// <summary>
        /// Returns a character without advancing on the string
        /// </summary>
        /// <returns></returns>
        public Char? Peek ( )
        {
            if ( this.ContentLeftSize == 0 )
                return null;
            return this.Code[this.Position];
        }

        /// <summary>
        /// Returns a character without advancing on the string
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Char? Peek ( Int32 offset )
        {
            if ( offset < 0 )
                throw new ArgumentException ( "Negative offsets not supported.", nameof ( offset ) );
            if ( offset >= this.ContentLeftSize )
                return null;
            return this.Code[this.Position + offset];
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
            if ( length > this.ContentLeftSize )
                return null;
            return this.Code.Substring ( this.Position, length );
        }

        #endregion Peeking

        #region Reading

        #region Basic Reads

        /// <summary>
        /// Returns a character while advancing on the string
        /// </summary>
        /// <returns></returns>
        public Char? Read ( )
        {
            if ( this.ContentLeftSize == 0 )
                return null;
            // Maybe use try-finally here?
            var @return = this.Code[this.Position];
            this.Advance ( 1 );
            return @return;
        }

        /// <summary>
        /// Returns a character while advancing on the string
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Char? Read ( Int32 offset )
        {
            if ( offset < 0 )
                throw new ArgumentException ( "Negative offsets not supported.", nameof ( offset ) );
            if ( offset == 0 )
                return this.Read ( );
            if ( offset >= this.ContentLeftSize )
                return null;
            // Maybe use try-finally here?
            var @return = this.Code[this.Position + offset];
            this.Advance ( offset + 1 );
            return @return;
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
                return String.Empty;
            if ( length > this.ContentLeftSize )
                return null;
            // Maybe use try-finally here?
            var @return = this.Code.Substring ( this.Position, length );
            this.Advance ( length );
            return @return;
        }

        #endregion Basic Reads

        #region Delimiter-based Reading

        /// <summary>
        /// Reads until <paramref name="delim" /> is found.
        /// </summary>
        /// <param name="delim"></param>
        /// <returns></returns>
        public String ReadStringUntil ( Char delim )
        {
            var length = this.FindOffset ( delim );
            return this.ReadString ( length == -1 ? this.ContentLeftSize : length );
        }

        /// <summary>
        /// Reads until <paramref name="delim" /> is found
        /// </summary>
        /// <param name="delim"></param>
        /// <returns></returns>
        public String ReadStringUntil ( String delim )
        {
            var length = this.FindOffset ( delim );
            return this.ReadString ( length == -1 ? this.ContentLeftSize : length );
        }

        /// <summary>
        /// Read until <paramref name="ch" /> is not found
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public String ReadStringUntilNot ( Char ch )
        {
            // Find first different char and go from there
            var length = this.FindOffset ( v => v != ch );
            return this.ReadString ( length == -1 ? this.ContentLeftSize : length );
        }

        #endregion Delimiter-based Reading

        #region Filter-based Reading

        /// <summary>
        /// Reads while <paramref name="filter" /> returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringWhile ( Predicate<Char> filter )
        {
            if ( filter == null )
                throw new ArgumentNullException ( nameof ( filter ) );

            var length = this.FindOffset ( v => !filter ( v ) );
            return this.ReadString ( length == -1 ? this.ContentLeftSize : length );
        }

        /// <summary>
        /// Reads until <paramref name="filter" /> returns false
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringWhileNot ( Predicate<Char> filter )
        {
            if ( filter == null )
                throw new ArgumentNullException ( nameof ( filter ) );

            var length = this.FindOffset ( filter );
            return this.ReadString ( length == -1 ? this.ContentLeftSize : length );
        }

        #endregion Filter-based Reading

        /// <summary>
        /// Reads a line
        /// </summary>
        /// <returns></returns>
        public String ReadLine ( )
        {
            // Expect CR + LF
            var lineend = this.FindOffset ( "\r\n" );

            // Fallback to LF if no CR + LF
            if ( lineend == -1 )
                lineend = this.FindOffset ( '\n' );

            // Fallback to EOF if no LF
            if ( lineend == -1 )
                lineend = this.ContentLeftSize;

            return this.ReadString ( lineend );
        }

        #endregion Reading

        /// <summary>
        /// Resets the Source Code reader
        /// </summary>
        public void Reset ( )
        {
            this.Line = 1;
            this.Column = 1;
            this.Position = 0;
            this.ContentLeftSize = this.Code.Length;
            this.LocationHistory.Clear ( );
        }

        /// <summary>
        /// Returns to <paramref name="location" />
        /// </summary>
        /// <param name="location">
        /// todo: describe save parameter on Rewind
        /// </param>
        public void Rewind ( SourceLocation location )
        {
            (Int32 line, Int32 col, Int32 pos) = location;
            if ( line < 0 || col < 0 || pos < 0 )
                throw new Exception ( "Invalid rewind position." );
            this.Line = line;
            this.Column = col;
            this.Position = pos;
            this.ContentLeftSize = this.Code.Length - pos;
        }

        #region Saving and Loading of Location

        /// <summary>
        /// Saves the current position of the reader in a stack
        /// </summary>
        /// <returns></returns>
        public void Save ( )
        {
            this.LocationHistory.Push ( this.Location );
        }

        /// <summary>
        /// Discards the last saved position
        /// </summary>
        public void DiscardSave ( )
        {
            this.LocationHistory.Pop ( );
        }

        /// <summary>
        /// Returns to the last position in the saved positions
        /// stack and returns the position
        /// </summary>
        public void Load ( )
        {
            (Int32 line, Int32 col, Int32 pos) = this.LocationHistory.Pop ( );
            this.Line = line;
            this.Column = col;
            this.Position = pos;
            this.ContentLeftSize = this.Code.Length - pos;
        }

        #endregion Saving and Loading of Location

        public override String ToString ( )
        {
            return this.Code.Substring ( this.Position, this.ContentLeftSize );
        }
    }
}
