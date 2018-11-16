using System;
using GParse.Common.IO;
using GParse.Common.Utilities;
using GParse.Fluent.Exceptions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Parsing
{
    internal class RegexExpressionParser : ParserCommons
    {
        public RegexExpressionParser ( SourceCodeReader reader )
        {
            this.Reader = reader;
        }

        private BaseMatcher ParseChar ( )
        {
            Int32 consumed;
            if ( ( consumed = CharAndEscapesTransducer.Execute ( new CrudeEnumerableSourceCodeReader ( this.Reader ), out BaseMatcher matcher ) ) > 0 )
            {
                this.Reader.Advance ( consumed );
                return matcher;
            }
            return null;
        }

        /// <summary>
        /// set-char-range ::= char, '-', char ; set ::= '[', {
        /// set-char-range | char }, ']' ;
        /// </summary>
        /// <returns></returns>
        private AlternatedMatcher ParseSet ( )
        {
            // Use a hashset so we don't get repeated matchers
            var opts = new NoDuplicatesList<BaseMatcher> ( );
            this.Expect ( '[' );

            // First character in a regex set can be a ] which
            // should be interpreted as normal char
            if ( this.Consume ( ']' ) )
                opts.Add ( new CharMatcher ( ']' ) );

            // Read the contents of the set
            while ( this.Reader.Peek ( ) != ']' && this.Reader.HasContent )
            {
                Int32 consumed;
                if ( ( consumed = RegexClassesTransducer.Execute ( new CrudeEnumerableSourceCodeReader ( this.Reader ), out BaseMatcher matcher ) ) > 0 )
                {
                    this.Reader.Advance ( consumed );
                    // Don't nest alternated matchers unnecessarily
                    if ( matcher is AlternatedMatcher alternated )
                        opts.AddRange ( alternated.PatternMatchers );
                    else
                        opts.Add ( matcher );
                }
                else
                {
                    Common.SourceLocation startLoc = this.Reader.Location;
                    BaseMatcher m1 = this.ParseChar ( );
                    // Actual ranges
                    if ( m1 is CharMatcher charMatcher1 && this.Reader.Peek ( 1 ) != ']' && this.Consume ( '-' ) )
                    {
                        BaseMatcher m2 = this.ParseChar ( );
                        if ( !( m2 is CharMatcher charMatcher2 ) )
                            throw new InvalidExpressionException ( startLoc, "Invalid range" );
                        if ( charMatcher2.Filter < charMatcher1.Filter )
                            throw new InvalidExpressionException ( startLoc, "Range cannot have start greater than end." );
                        opts.Add ( new RangeMatcher ( charMatcher1.Filter, charMatcher2.Filter ) );
                    }
                    else
                        opts.Add ( m1 );
                }
            }
            this.Expect ( ']' );

            return new AlternatedMatcher ( opts.ToArray ( ) );
        }

        private BaseMatcher ParseSuffixed ( ) => throw new NotImplementedException ( this.ToString ( ) );
    }
}
