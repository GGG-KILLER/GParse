using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GParse.Common.IO;
using GParse.Parsing.Lexing.Modules.Regex.AST;
using GParse.Parsing.Lexing.Modules.Regex.Runner;

namespace GParse.Parsing.Lexing.Modules.Regex
{
    internal class RegexRunner : ITreeVisitor<Result<String, MatchError>>
    {
        private readonly Dictionary<Int32, String> Captures = new Dictionary<Int32, String> ( );
        private readonly SourceCodeReader Reader;

        private readonly Stack<Node> Nexts = new Stack<Node> ( );

        public RegexRunner ( SourceCodeReader reader )
        {
            this.Reader = reader;
        }

        /// <summary>
        /// Runs the regex of the specified node on the reader and
        /// resets the position to the one the reader was in
        /// before the node was executed
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Result<String, MatchError> PeekVisit ( Node node )
        {
            if ( node == null )
                throw new ArgumentNullException ( nameof ( node ) );

            Common.SourceLocation loc = this.Reader.Location;
            try { return node.Accept ( this ); }
            finally { this.Reader.Rewind ( loc ); }
        }

        /// <summary>
        /// Attempts to run the regex of the specified node on the
        /// reader and if it fails, the reader is reset to the
        /// location it was before the node was executed
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Result<String, MatchError> SafeVisit ( Node node )
        {
            if ( node == null )
                throw new ArgumentNullException ( nameof ( node ) );

            Common.SourceLocation loc = this.Reader.Location;
            try
            {
                Result<String, MatchError> res = node.Accept ( this );
                if ( !res.Success )
                    this.Reader.Rewind ( loc );
                return res;
            }
            catch
            {
                this.Reader.Rewind ( loc );
                throw;
            }
        }

        public Result<String, MatchError> Visit ( Alternation alternation )
        {
            Common.SourceLocation start = this.Reader.Location;
            if ( this.Reader.IsAtEOF )
                return new Result<String, MatchError> ( new MatchError ( this.Reader.Location, "EOF." ) );

            if ( alternation.IsNegated )
            {
                foreach ( Node child in alternation.Children )
                    if ( !this.Reader.IsAtEOF && this.SafeVisit ( child ).Success
                        && !alternation.IsLazy )
                        return new Result<String, MatchError> ( new MatchError ( start, "Expected not to match but matched." ) );
                return new Result<String, MatchError> ( this.Reader.ReadString ( 1 ) );
            }

            foreach ( Node child in alternation.Children )
            {
                Result<String, MatchError> res = this.SafeVisit ( child );
                if ( res.Success )
                    return res;
            }

            return alternation.IsLazy
                ? new Result<String, MatchError> ( "" )
                : new Result<String, MatchError> ( new MatchError ( start, "Unable to match." ) );
        }

        public Result<String, MatchError> Visit ( Capture capture )
        {
            Result<String, MatchError> res = this.SafeVisit ( capture.Inner );
            if ( res.Success )
            {
                this.Captures[capture.CaptureNumber] = res.Value;
                return res;
            }

            return capture.IsLazy
                ? new Result<String, MatchError> ( "" )
                : res;
        }

        public Result<String, MatchError> Visit ( CaptureReference reference )
        {
            if ( this.Captures.TryGetValue ( reference.CaptureNumber, out var value ) )
            {
                if ( value != "" && !this.Reader.IsNext ( value ) )
                    return reference.IsLazy
                        ? new Result<String, MatchError> ( "" )
                        : new Result<String, MatchError> ( new MatchError ( this.Reader.Location, $"Expected to match capture {reference.CaptureNumber}'s value ('{value}') but got something else." ) );

                this.Reader.Advance ( value.Length );
                return new Result<String, MatchError> ( value );
            }

            throw new InvalidRegexException ( this.Reader.Location, $"Attempt to reference invalid capture ({reference.CaptureNumber}). Existent captures: {{{String.Join ( ", ", this.Captures.Select ( kv => $"[{kv.Key}] = '{kv.Value}'" ) )}}}" );
        }

        public Result<String, MatchError> Visit ( Literal literal )
        {
            if ( this.Reader.IsNext ( literal.Value ) )
            {
                this.Reader.Advance ( 1 );
                return new Result<String, MatchError> ( literal.Value.ToString ( ) );
            }
            else if ( literal.IsLazy )
                return new Result<String, MatchError> ( "" );
            return new Result<String, MatchError> ( new MatchError ( this.Reader.Location, $"Expected {literal.Value} but got something else." ) );
        }

        // Check if the next character in the reader (if any) is
        // in the range. If it is, return whatever we peeked at,
        // otherwise return null.
        public Result<String, MatchError> Visit ( Range range )
        {
            if ( this.Reader.HasContent && range.CharRange.ValueIn ( ( Char ) this.Reader.Peek ( ) ) )
                return new Result<String, MatchError> ( this.Reader.ReadString ( 1 ) );

            return range.IsLazy
                ? new Result<String, MatchError> ( "" )
                : new Result<String, MatchError> ( new MatchError ( this.Reader.Location, $"Expected character inside {range.CharRange} but got something outside of it." ) );
        }

        public Result<String, MatchError> Visit ( Repetition repetition )
        {
            Int32 i;
            Common.SourceLocation start = this.Reader.Location;
            var res = new Result<String, MatchError> ( new MatchError ( this.Reader.Location, "Not executed." ) );
            var acc = new StringBuilder ( );
            Node next = this.Nexts.Count == 0 ? null : this.Nexts.Peek ( );

            for ( i = 0; i <= repetition.Range.End; i++ )
            {
                // If we've already hit the minimum quota and
                // either there isn't a next or next can already
                // be matched, stop doing anything
                if ( repetition.IsLazy && i >= repetition.Range.Start
                    && ( next == null || this.PeekVisit ( next ).Success ) )
                    break;

                // Otherwise attempt to execute normally.
                res = this.SafeVisit ( repetition.Inner );
                if ( res.Success )
                    acc.Append ( res.Value );
                else
                    break;
            }

            // Return null in case we've failed to execute the
            // minimum amount of times
            return i < repetition.Range.Start
                ? ( repetition.IsLazy
                    ? new Result<String, MatchError> ( "" )
                    : new Result<String, MatchError> ( new MatchError ( start, "Failed to match expression the minimum amount of times." ) ) )
                : new Result<String, MatchError> ( acc.ToString ( ) );
        }

        public Result<String, MatchError> Visit ( Sequence sequence )
        {
            var acc = new StringBuilder ( );
            for ( var i = 0; i < sequence.Children.Length; i++ )
            {
                this.Nexts.Push ( i < sequence.Children.Length - 1 ? sequence.Children[i + 1] : null );
                Result<String, MatchError> res = this.SafeVisit ( sequence.Children[i] );
                this.Nexts.Pop ( );

                if ( res.Success )
                    acc.Append ( res.Value );
                else
                    return sequence.IsLazy
                        ? new Result<String, MatchError> ( "" )
                        : res;
            }

            return new Result<String, MatchError> ( acc.ToString ( ) );
        }
    }
}
