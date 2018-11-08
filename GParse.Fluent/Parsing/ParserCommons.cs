using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GParse.Common;
using GParse.Common.IO;
using GParse.Fluent.Exceptions;
using GParse.Fluent.Matchers;
using GParse.StateMachines;

namespace GParse.Fluent.Parsing
{
    internal class ParserCommons
    {
        protected class CrudeEnumerableSourceCodeReader : SourceCodeReader, IEnumerable<Char>
        {
            public struct Enumerator : IEnumerator<Char>
            {
                private readonly SourceLocation StartingLocation;
                private SourceCodeReader Reader;
                private Boolean MovedForFirstTime;

                public Char Current
                {
                    get
                    {
                        if ( !this.MovedForFirstTime )
                            throw new InvalidOperationException ( "The iteration has not started. Call MoveNext to start it" );
                        if ( !( this.Reader?.IsAtEOF is true ) )
                            throw new InvalidOperationException ( "The iteration has already finished." );

                        return ( Char ) this.Reader.Peek ( );
                    }
                }

                Object IEnumerator.Current
                {
                    get
                    {
                        if ( !this.MovedForFirstTime )
                            throw new InvalidOperationException ( "The iteration has not started. Call MoveNext to start it" );
                        if ( !( this.Reader?.IsAtEOF is true ) )
                            throw new InvalidOperationException ( "The iteration has already finished." );

                        return ( Char ) this.Reader.Peek ( );
                    }
                }

                public Enumerator ( CrudeEnumerableSourceCodeReader reader )
                {
                    this.Reader = reader;
                    this.StartingLocation = reader.Location;
                    this.MovedForFirstTime = false;
                }

                public void Dispose ( ) => this.Reader = null;

                public Boolean MoveNext ( )
                {
                    if ( !this.MovedForFirstTime )
                    {
                        this.MovedForFirstTime = true;
                        return true;
                    }
                    else if ( this.Reader.HasContent )
                    {
                        this.Reader.Advance ( 1 );
                        return true;
                    }

                    return false;
                }

                public void Reset ( )
                {
                    this.Reader.Rewind ( this.StartingLocation );
                    this.MovedForFirstTime = false;
                }
            }

            public CrudeEnumerableSourceCodeReader ( String str ) : base ( str )
            {
            }

            public CrudeEnumerableSourceCodeReader ( SourceCodeReader reader ) : base ( reader )
            {
            }

            public IEnumerator<Char> GetEnumerator ( ) => new Enumerator ( this );

            IEnumerator IEnumerable.GetEnumerator ( ) => new Enumerator ( this );
        }

        #region Hardcoded things

        public static readonly IReadOnlyDictionary<String, BaseMatcher> RegexClassesLUT = new Dictionary<String, BaseMatcher>
        {
            { @"[:ascii:]", new RangeMatcher ( '\x00', '\xFF' ) },
            { @"\p{ASCII}", new RangeMatcher ( '\x00', '\xFF' ) },
            { @"[:alnum:]", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' )
            ) },
            { @"\p{Alnum}", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' )
            ) },
            { @"[:word:]", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' ),
                new CharMatcher ( '_' )
            ) },
            { @"\w", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' ),
                new CharMatcher ( '_' )
            ) },
            { @"\W", new NegatedMatcher ( new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' ),
                new RangeMatcher ( '0', '9' ),
                new CharMatcher ( '_' )
            ) ) },
            { @"[:alpha:]", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' )
            ) },
            { @"\p{Alpha}", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'Z' ),
                new RangeMatcher ( 'a', 'z' )
            ) },
            { @"[:blank:]", new CharListMatcher ( ' ', '\t' ) },
            { @"\p{Blank}", new CharListMatcher ( ' ', '\t' ) },
            // TODO: Word boundaries
            { @"[:cntrl:]", new AlternatedMatcher (
                new RangeMatcher ( '\x00', '\x1F' ),
                new CharMatcher ( '\x7F' )
            ) },
            { @"\p{Cntrl}", new AlternatedMatcher (
                new RangeMatcher ( '\x00', '\x1F' ),
                new CharMatcher ( '\x7F' )
            ) },
            { @"[:digit:]", new RangeMatcher ( '0', '9' ) },
            { @"\p{Digit}", new RangeMatcher ( '0', '9' ) },
            { @"\d",        new RangeMatcher ( '0', '9' ) },
            { @"\D",        new NegatedMatcher ( new RangeMatcher ( '0', '9' ) ) },
            { @"[:graph:]", new RangeMatcher ( '\x21', '\x7E' ) },
            { @"\p{Graph}", new RangeMatcher ( '\x21', '\x7E' ) },
            { @"[:lower:]", new RangeMatcher ( 'a', 'z' ) },
            { @"\p{Lower}", new RangeMatcher ( 'a', 'z' ) },
            { @"[:print:]", new RangeMatcher ( '\x20', '\x7E' ) },
            { @"\p{Print}", new RangeMatcher ( '\x20', '\x7E' ) },
            { @"[:punct:]", new CharListMatcher ( '[', ']', '[', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',',
                '.', '/', ':', ';', '<', '=', '>', '?', '@', '\\', '^', '_', '`', '{', '|', '}', '~', '-', ']' ) },
            { @"\p{Punct}", new CharListMatcher ( '[', ']', '[', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',',
                '.', '/', ':', ';', '<', '=', '>', '?', '@', '\\', '^', '_', '`', '{', '|', '}', '~', '-', ']' ) },
            { @"[:space:]", new CharListMatcher ( ' ', '\t', '\n', '\r', '\v', '\f' ) },
            { @"\p{Space}", new CharListMatcher ( ' ', '\t', '\n', '\r', '\v', '\f' ) },
            { @"\s",        new CharListMatcher ( ' ', '\t', '\n', '\r', '\v', '\f' ) },
            { @"[:upper:]", new RangeMatcher ( 'A', 'Z' ) },
            { @"\p{Upper}", new RangeMatcher ( 'A', 'Z' ) },
            { @"[:xdigit:]", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'F' ),
                new RangeMatcher ( 'a', 'f' ),
                new RangeMatcher ( '0', '9' )
            ) },
            { @"\p{XDigit}", new AlternatedMatcher (
                new RangeMatcher ( 'A', 'F' ),
                new RangeMatcher ( 'a', 'f' ),
                new RangeMatcher ( '0', '9' )
            ) },
        };

        // TODO: Find out how much memory this takes and see if it's worth it
        #region State Machines

        public static readonly Transducer<Char, BaseMatcher> CharAndEscapesTransducer;

        public static readonly Transducer<Char, BaseMatcher> RegexClassesTransducer;

        static ParserCommons ( )
        {
            CharAndEscapesTransducer = new Transducer<Char, BaseMatcher> ( );
            foreach ( KeyValuePair<String, BaseMatcher> kv in RegexClassesLUT )
                if ( kv.Key[0] == '\\' )
                    CharAndEscapesTransducer.InitialState.OnInput ( kv.Key.ToCharArray ( ), kv.Value );

            foreach ( var ch in new[] { '.', '?', '*', '[', ']', '+', '"', '\'', '\\', '-' } )
                CharAndEscapesTransducer.InitialState.OnInput ( new[] { '\\', ch }, new CharMatcher ( ch ) );

            foreach ( var ch in Enumerable.Range ( Char.MinValue, Char.MaxValue ).Select ( c => ( Char ) c ) )
                if ( ch != '.' && ch != '?' && ch != '*' && ch != '[' && ch != '+' )
                    CharAndEscapesTransducer.InitialState.OnInput ( ch, new CharMatcher ( ch ) );

            for ( var i = 0; i < 256; i++ )
            {
                var m = new CharMatcher ( ( Char ) i );
                if ( i < 8 ) // 0o10
                    CharAndEscapesTransducer.InitialState.OnInput ( $"\\00{Convert.ToString ( i, 8 )}".ToCharArray ( ), m );

                if ( i < 0x10 )
                {
                    CharAndEscapesTransducer.InitialState.OnInput ( $"\\x0{i:x}".ToCharArray ( ), m );
                    CharAndEscapesTransducer.InitialState.OnInput ( $"\\x0{i:X}".ToCharArray ( ), m );
                }

                if ( i < 64 ) // 0o100
                    CharAndEscapesTransducer.InitialState.OnInput ( $"\\0{Convert.ToString ( i, 8 )}".ToCharArray ( ), m );

                CharAndEscapesTransducer.InitialState.OnInput ( $"\\{Convert.ToString ( i, 8 )}".ToCharArray ( ), m );
            }

            CharAndEscapesTransducer.InitialState.OnInput ( '.', new FilterFuncMatcher ( _ => true ) );
            CharAndEscapesTransducer.InitialState.OnInput ( new[] { '\\', 'a' }, new CharMatcher ( '\a' ) );
            CharAndEscapesTransducer.InitialState.OnInput ( new[] { '\\', 'b' }, new CharMatcher ( '\b' ) );
            CharAndEscapesTransducer.InitialState.OnInput ( new[] { '\\', 'f' }, new CharMatcher ( '\f' ) );
            CharAndEscapesTransducer.InitialState.OnInput ( new[] { '\\', 'n' }, new CharMatcher ( '\n' ) );
            CharAndEscapesTransducer.InitialState.OnInput ( new[] { '\\', 'r' }, new CharMatcher ( '\r' ) );
            CharAndEscapesTransducer.InitialState.OnInput ( new[] { '\\', 't' }, new CharMatcher ( '\t' ) );
            CharAndEscapesTransducer.InitialState.OnInput ( new[] { '\\', 'v' }, new CharMatcher ( '\v' ) );

            RegexClassesTransducer = new Transducer<Char, BaseMatcher> ( );
            foreach ( KeyValuePair<String, BaseMatcher> kv in RegexClassesLUT )
                RegexClassesTransducer.InitialState.OnInput ( kv.Key.ToCharArray ( ), kv.Value );
        }

        #endregion State Machines

        #endregion Hardcoded things

        protected SourceCodeReader Reader;

        protected CrudeEnumerableSourceCodeReader EnumerableReader => new CrudeEnumerableSourceCodeReader ( this.Reader );

        #region Utilities

        protected void Expect ( Char ch )
        {
            if ( !this.Reader.IsNext ( ch ) )
                throw new InvalidExpressionException ( this.Reader.Location, $"Expected '{ch}' but got '{( Char ) this.Reader.Peek ( )}'." );
            this.Reader.Advance ( 1 );
        }

        protected Boolean Consume ( Char ch )
        {
            if ( !this.Reader.IsNext ( ch ) )
                return false;
            this.Reader.Advance ( 1 );
            return true;
        }

        protected Boolean Consume ( String str )
        {
            if ( !this.Reader.IsNext ( str ) )
                return false;
            this.Reader.Advance ( str.Length );
            return true;
        }

        #endregion Utilities
    }
}
