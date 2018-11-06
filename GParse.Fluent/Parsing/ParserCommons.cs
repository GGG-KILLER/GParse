using System;
using System.Collections.Generic;
using GParse.Common.IO;
using GParse.Fluent.Exceptions;
using GParse.Fluent.Matchers;
using GParse.StateMachines;

namespace GParse.Fluent.Parsing
{
    internal class ParserCommons
    {
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
        public static readonly Transducer<Char, BaseMatcher> CharTransducer;
        public static readonly Transducer<Char, BaseMatcher> RegexClassTransducer;

        static ParserCommons ( )
        {
            CharTransducer = new Transducer<Char, BaseMatcher> ( );
            State<Char, BaseMatcher> initial = CharTransducer.InitialState;
            for ( var i = Char.MinValue; i <= Char.MaxValue; i++ )
            {
                switch ( i )
                {
                    case '.':
                        initial.OnInput ( i, new RangeMatcher ( Char.MinValue, Char.MaxValue ) );
                        break;

                    case '\\':
                        initial.OnInput ( i, escapeState =>
                        {
                            foreach ( KeyValuePair<String, BaseMatcher> kv in RegexClassesLUT )
                                if ( kv.Key[0] == '\\' )
                                    escapeState.OnInput ( kv.Key.ToCharArray ( ), kv.Value, 1 );

                            escapeState.OnInput ( 'a', new CharMatcher ( '\a' ) );
                            escapeState.OnInput ( 'b', new CharMatcher ( '\b' ) );
                            escapeState.OnInput ( 'f', new CharMatcher ( '\f' ) );
                            escapeState.OnInput ( 'n', new CharMatcher ( '\n' ) );
                            escapeState.OnInput ( 'r', new CharMatcher ( '\r' ) );
                            escapeState.OnInput ( 't', new CharMatcher ( '\t' ) );
                            escapeState.OnInput ( 'v', new CharMatcher ( '\v' ) );
                        } );
                        break;

                    default:
                        initial.OnInput ( i, new CharMatcher ( i ) );
                        break;
                }
            }

            RegexClassTransducer = ( Transducer<Char, BaseMatcher> ) CharTransducer.Clone ( );
            RegexClassTransducer.InitialState.OnInput ( '[', state =>
            {
                foreach ( KeyValuePair<String, BaseMatcher> kv in RegexClassesLUT )
                    if ( kv.Key[0] == '[' )
                        state.OnInput ( kv.Key.ToCharArray ( ), kv.Value, 1 );
            } );
        }

        #endregion Hardcoded things

        protected SourceCodeReader Reader;

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
