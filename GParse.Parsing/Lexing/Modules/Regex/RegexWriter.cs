using System;
using System.Collections.Generic;
using GParse.Common.Math;
using GParse.Common.Utilities;
using GParse.Parsing.Lexing.Modules.Regex.AST;

namespace GParse.Parsing.Lexing.Modules.Regex
{
    internal class RegexWriter : ITreeVisitor<String>
    {
        internal static readonly IReadOnlyDictionary<Node, String> ReverseClassLUT;

        static RegexWriter ( )
        {
            var dict = new Dictionary<Node, String> ( );
            foreach ( (String name, Node node) in RegexParser.ClassTree.GetAll ( ) )
                if ( !dict.ContainsKey ( node ) )
                    dict[node] = name;
            ReverseClassLUT = dict;
        }

        public String Visit ( Alternation alternation )
        {
            if ( ReverseClassLUT.TryGetValue ( alternation, out var className ) )
                return className;

            var isSet = true;
            var conv = Array.ConvertAll ( alternation.Children, child =>
            {
                if ( ReverseClassLUT.TryGetValue ( child, out className ) )
                    return className;
                else
                {
                    if ( !( child is Literal ) && !( child is Range ) )
                        isSet = false;
                    return child.Accept ( this );
                }
            } );

            if ( isSet )
            {
                return ( alternation.IsNegated ? "[^" : "[" )
                    + String.Join ( "", conv )
                    + ( alternation.IsLazy ? "]?" : "]" );
            }
            else
            {
                var res = $"(?:{String.Join ( "|", conv )})";
                if ( alternation.IsLazy )
                    res += "?";
                return res;
            }
        }

        public String Visit ( Capture capture ) => $"({capture.Inner.Accept ( this )}{( capture.IsLazy ? ")?" : ")" )}";

        public String Visit ( CaptureReference reference ) => $"\\{( reference.IsLazy ? reference.CaptureNumber + "?" : reference.CaptureNumber.ToString ( ) )}";

        public String Visit ( Literal literal )
        {
            String @out;
            var val = literal.Value;
            switch ( val )
            {
                case '"':
                case '\'':
                case '[':
                case ']':
                case '*':
                case '+':
                case '?':
                case '\\':
                case '>':
                case '-':
                case '|':
                case '.':
                    @out = $"\\{val}";
                    break;

                default:
                    @out = StringUtilities.GetCharacterRepresentation ( val );
                    break;
            }

            return literal.IsLazy ? @out + '?' : @out;
        }

        public String Visit ( Range range )
        {
            if ( ReverseClassLUT.TryGetValue ( range, out var className ) )
                return className;

            Range<Char> crange = range.CharRange;
            return $"{StringUtilities.GetCharacterRepresentation ( crange.Start )}-{StringUtilities.GetCharacterRepresentation ( crange.End )}";
        }

        public String Visit ( Repetition repetition )
        {
            Range<UInt32> rrange = repetition.Range;
            var res = repetition.Inner.Accept ( this );
            if ( rrange.Start == 0 && rrange.End == UInt32.MaxValue )
                res += "*";
            else if ( rrange.Start == 1 && rrange.End == UInt32.MaxValue )
                res += "+";
            else
                res += $"{{{rrange.Start}, {rrange.End}}}";

            if ( repetition.IsLazy )
                res += "?";

            return res;
        }

        public String Visit ( Sequence sequence )
        {
            if ( ReverseClassLUT.TryGetValue ( sequence, out var className ) )
                return className;

            var res = String.Join ( "", Array.ConvertAll ( sequence.Children, child => child.Accept ( this ) ) );
            return sequence.IsLazy ? $"(?:{res})?" : res;
        }
    }
}
