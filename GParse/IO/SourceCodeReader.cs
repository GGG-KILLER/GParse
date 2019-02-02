using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GParse.IO
{
    /// <summary>
    /// A delegate that makes use of the <see langword="in" /> modifier.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    public delegate Boolean InPredicate<T> ( in T item );

    /// <summary>
    /// A source code reader
    /// </summary>
    public class SourceCodeReader : IEquatable<SourceCodeReader>
    {
        /// <summary>
        /// Cache of compiled regular expressions
        /// </summary>
        private static readonly Dictionary<String, Regex> RegexCache = new Dictionary<String, Regex> ( );

        /// <summary>
        /// The string
        /// </summary>
        private readonly String Code;

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

        /// <summary>
        /// Resets the Source Code reader
        /// </summary>
        public void Reset ( )
        {
            this.Line = 1;
            this.Column = 1;
            this.Position = 0;
        }

        /// <summary>
        /// Returns to <paramref name="location" />
        /// </summary>
        /// <param name="location">todo: describe save parameter on Rewind</param>
        public void Rewind ( SourceLocation location )
        {
            if ( location.Line < 0 || location.Column < 0 || location.Byte < 0 )
                throw new Exception ( "Invalid rewind position." );

            this.Line = location.Line;
            this.Column = location.Column;
            this.Position = location.Byte;
        }

        #endregion Location Management

        /// <summary>
        /// The length of the string that was passed to this reader
        /// </summary>
        public Int32 Length => this.Code.Length;

        /// <summary>
        /// The amount of chars left to be read
        /// </summary>
        public Int32 ContentLeft => this.Length - this.Position;

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
        }

        /// <summary>
        /// Initializes this reader
        /// </summary>
        /// <param name="reader"></param>
        public SourceCodeReader ( SourceCodeReader reader )
        {
            this.Code = reader.Code ?? throw new InvalidOperationException ( "Cannot clone a reader whose Code is null" );
            this.Position = reader.Position;
            this.Line = reader.Line;
            this.Column = reader.Column;
        }

        /// <summary>
        /// Advances by <paramref name="offset" /> chars
        /// </summary>
        /// <param name="offset"></param>
        public void Advance ( Int32 offset )
        {
            if ( offset < 0 )
                throw new ArgumentOutOfRangeException ( nameof ( offset ), "Offset must be positive" );
            if ( offset > this.ContentLeft )
                throw new ArgumentOutOfRangeException ( nameof ( offset ), "Offset is too big." );

            while ( offset-- > 0 )
            {
                if ( this.Code[this.Position++] == '\n' )
                {
                    this.Line++;
                    this.Column = 1;
                }
                else
                    this.Column++;
            }
        }

        #region IsNext Variations

        /// <summary>
        /// Returns whether the next character in the "stream" is <paramref name="ch" />.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public Boolean IsNext ( Char ch ) =>
            this.HasContent && this.Code[this.Position] == ch;

        /// <summary>
        /// Returns whether the character at <paramref name="offset" /> in the "stream" is
        /// <paramref name="ch" />.
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsNext ( Char ch, Int32 offset ) =>
            !this.HasContent || offset > this.ContentLeft ? false : this.Code[this.Position + offset] == ch;

        /// <summary>
        /// Confirms wether or not the next <paramref name="str" />.Length chars are
        /// <paramref name="str" />
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Boolean IsNext ( String str )
        {
            var len = str.Length;
            if ( len > this.ContentLeft )
                return false;

            // TODO: KMP substring search
            for ( Int32 i1 = this.Position, i2 = 0; i2 < len; i1++, i2++ )
                if ( this.Code[i1] != str[i2] )
                    return false;
            return true;
        }

        /// <summary>
        /// Confirms wether or not the next <paramref name="str" />.Length chars on the offset
        /// <paramref name="offset" /> are <paramref name="str" />
        /// </summary>
        /// <param name="str"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Boolean IsNext ( String str, Int32 offset )
        {
            var len = str.Length;
            if ( len + offset > this.ContentLeft )
                return false;

            // TODO: KMP substring search
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
        /// Finds the offset of a character that satisfies a <paramref name="predicate" />
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

        /// <summary>
        /// Finds the offset of a character that satisfies a <paramref name="predicate" />
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Int32 FindOffset ( InPredicate<Char> predicate )
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
        public Char? Peek ( ) => this.ContentLeft == 0 ? null : ( Char? ) this.Code[this.Position];

        /// <summary>
        /// Returns a character without advancing on the string
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Char? Peek ( Int32 offset )
        {
            if ( offset < 0 )
                throw new ArgumentException ( "Negative offsets not supported.", nameof ( offset ) );

            return offset >= this.ContentLeft ? null : ( Char? ) this.Code[this.Position + offset];
        }

        /// <summary>
        /// Returns a string of <paramref name="length" /> size without advancing on the string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public String PeekString ( Int32 length )
        {
            if ( length < 0 )
                throw new ArgumentException ( "Length must be positive.", nameof ( length ) );

            return length > this.ContentLeft ? null : this.Code.Substring ( this.Position, length );
        }

        /// <summary>
        /// Attempts to match a regex but does not advance.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Match PeekRegex ( String expression )
        {
            if ( !RegexCache.ContainsKey ( expression ) )
                RegexCache[expression] = new Regex ( "\\G" + expression, RegexOptions.Compiled );

            return RegexCache[expression].Match ( this.Code, this.Position );
        }

        /// <summary>
        /// Attempts to match a regex but does not advance.
        /// </summary>
        /// <param name="regex">
        /// <para>
        /// A <see cref="Regex" /> instance that contains an expression starting with the \G modifier.
        /// </para>
        /// <para>
        /// An exception will be thrown if the match does not start at the same position the reader is
        /// located at.
        /// </para>
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method is offered purely for the performance benefits of regular expressions generated
        /// with Regex.CompileToAssembly
        /// (https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.compiletoassembly).
        /// It is not meant to be used with anything else, since all regexes passed in the form of strings
        /// are stored in an internal cache and the instances are initialized with
        /// <see cref="RegexOptions.Compiled" />.
        /// </remarks>
        public Match PeekRegex ( Regex regex )
        {
            Match match = regex.Match ( this.Code, this.Position );
            if ( match.Success && match.Index != this.Position )
                throw new ArgumentException ( "The regular expression being used does not contain the '\\G' modifier at the start. The matched result does not start at the reader's current location.", nameof ( regex ) );
            return match;
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
            if ( this.ContentLeft == 0 )
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
            if ( offset >= this.ContentLeft )
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
            if ( length > this.ContentLeft )
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
            return this.ReadString ( length == -1 ? this.ContentLeft : length );
        }

        /// <summary>
        /// Reads until <paramref name="delim" /> is found
        /// </summary>
        /// <param name="delim"></param>
        /// <returns></returns>
        public String ReadStringUntil ( String delim )
        {
            var length = this.FindOffset ( delim );
            return this.ReadString ( length == -1 ? this.ContentLeft : length );
        }

        /// <summary>
        /// Read until <paramref name="ch" /> is not found
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public String ReadStringUntilNot ( Char ch )
        {
            // Find first different char and go from there
            var length = this.FindOffset ( ( in Char v ) => v != ch );
            return this.ReadString ( length == -1 ? this.ContentLeft : length );
        }

        #endregion Delimiter-based Reading

        #region Filter-based Reading

        /// <summary>
        /// Reads from the reader while the <paramref name="filter" /> returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringWhile ( Predicate<Char> filter )
        {
            if ( filter == null )
                throw new ArgumentNullException ( nameof ( filter ) );

            var length = this.FindOffset ( v => !filter ( v ) );
            return this.ReadString ( length == -1 ? this.ContentLeft : length );
        }

        /// <summary>
        /// Reads from the reader while the <paramref name="filter" /> returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringWhile ( InPredicate<Char> filter )
        {
            if ( filter == null )
                throw new ArgumentNullException ( nameof ( filter ) );

            var length = this.FindOffset ( ( in Char v ) => !filter ( v ) );
            return this.ReadString ( length == -1 ? this.ContentLeft : length );
        }

        /// <summary>
        /// Reads from the reader until the <paramref name="filter" /> returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringWhileNot ( Predicate<Char> filter )
        {
            if ( filter == null )
                throw new ArgumentNullException ( nameof ( filter ) );

            var length = this.FindOffset ( filter );
            return this.ReadString ( length == -1 ? this.ContentLeft : length );
        }

        /// <summary>
        /// Reads from the reader until the <paramref name="filter" /> returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringWhileNot ( InPredicate<Char> filter )
        {
            if ( filter == null )
                throw new ArgumentNullException ( nameof ( filter ) );

            var length = this.FindOffset ( filter );
            return this.ReadString ( length == -1 ? this.ContentLeft : length );
        }

        /// <summary>
        /// Reads from the reader until the <paramref name="filter" /> returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringUntill ( Predicate<Char> filter ) => this.ReadStringWhileNot ( filter );

        /// <summary>
        /// Reads from the reader until the <paramref name="filter" /> returns true
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public String ReadStringUntill ( InPredicate<Char> filter ) => this.ReadStringWhileNot ( filter );

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
                lineend = this.ContentLeft;

            return this.ReadString ( lineend );
        }

        /// <summary>
        /// Attempts to match a regex but does not advance if it fails.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Match MatchRegex ( String expression )
        {
            if ( !RegexCache.ContainsKey ( expression ) )
                RegexCache[expression] = new Regex ( "\\G" + expression, RegexOptions.Compiled );

            Match match = RegexCache[expression].Match ( this.Code, this.Position );
            if ( match.Success )
                this.Advance ( match.Length );
            return match;
        }

        /// <summary>
        /// Attempts to match a regex but does not advance if it fails.
        /// </summary>
        /// <param name="regex">
        /// <para>
        /// A <see cref="Regex" /> instance that contains an expression starting with the \G modifier.
        /// </para>
        /// <para>
        /// An exception will be thrown if the match does not start at the same position the reader is
        /// located at.
        /// </para>
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method is offered purely for the performance benefits of regular expressions generated
        /// with Regex.CompileToAssembly
        /// (https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.compiletoassembly).
        /// It is not meant to be used with anything else, since all regexes passed in the form of strings
        /// are stored in an internal cache and the instances are initialized with
        /// <see cref="RegexOptions.Compiled" />.
        /// </remarks>
        public Match MatchRegex ( Regex regex )
        {
            Match match = regex.Match ( this.Code, this.Position );
            if ( match.Success )
            {
                if ( match.Index != this.Position )
                    throw new ArgumentException ( "The regular expression being used does not contain the '\\G' modifier at the start. The matched result does not start at the reader's current location.", nameof ( regex ) );
                this.Advance ( match.Length );
            }
            return match;
        }

        #endregion Reading

        #region Object

        /// <summary>
        /// Returns the rest of the "stream" as a string
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) => this.Code.Substring ( this.Position, this.ContentLeft );

        #endregion Object

        /// <summary>
        /// There is only one kind of cloning this can have (shallow ≡ deep for this)
        /// </summary>
        /// <returns></returns>
        public SourceCodeReader Copy ( ) => new SourceCodeReader ( this );

        #region IEquatable<SourceCodeReader>

        /// <summary>
        /// Whether this reader is qeual to another
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( SourceCodeReader other ) =>
            other.Code == this.Code && other.Position == this.Position && other.Line == this.Line && other.Column == this.Column && other.ContentLeft == this.ContentLeft;

        #endregion IEquatable<SourceCodeReader>
    }
}
