using System;
using System.Linq;
using System.Text;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Lexing.Modules.Regex;
using GParse.Parsing.Lexing.Modules.Regex.AST;
using GParse.Parsing.Lexing.Modules.Regex.Runner;

namespace GParse.Parsing.Lexing.Modules
{
    public class RegexLexerModule<TokenTypeT> : ILexerModule<TokenTypeT> where TokenTypeT : IEquatable<TokenTypeT>
    {
        private readonly String ID;
        private readonly TokenTypeT Type;
        private readonly Node RegexNode;
        private readonly Boolean IsLiteral;
        private readonly Func<String, Object> Converter;

        public String Name { get; }
        public String Prefix { get; }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex, Func<String, Object> converter = null )
        {
            this.ID = ID;
            this.Type = type;
            this.RegexNode = new RegexParser ( regex ).Parse ( );
            this.Converter = converter;

            if ( this.RegexNode is Sequence sequence )
            {
                if ( Array.TrueForAll ( sequence.Children, child => child is Literal ) )
                {
                    this.IsLiteral = true;
                    this.Prefix = String.Join ( "", Array.ConvertAll ( sequence.Children, child => ( child as Literal ).Value ) );
                    this.RegexNode = null;
                }
                else
                {
                    var acc = new StringBuilder ( );
                    for ( var i = 0; i < sequence.Children.Length; i++ )
                    {
                        if ( sequence.Children[i] is Literal literal )
                            acc.Append ( literal.Value );
                        else
                            break;
                    }

                    if ( acc.Length > 0 )
                    {
                        this.Prefix = acc.ToString ( );
                        this.RegexNode = new Sequence ( sequence.Children.Skip ( acc.Length ) );
                    }
                }
            }
        }

        // Ideally modules should be stateless, but read
        // CanConsumeNext for the explanation.
        private Common.SourceLocation Start;
        private Common.SourceLocation End;
        private String StoredResult;

        public Boolean CanConsumeNext ( SourceCodeReader reader )
        {
            if ( this.IsLiteral )
                return true;

            // Ideally CanConsumeNext should not leave the reader
            // modified, but in this case since we'll be called in
            // sequence, it's good to not re-execute the matching again.
            this.Start = reader.Location;
            this.StoredResult = null;
            Result<String, MatchError> res = new RegexRunner ( reader ).SafeVisit ( this.RegexNode );
            if (  res.Success )
            {
                this.End = reader.Location;
                this.StoredResult = res.Value;

                reader.Rewind ( this.Start );
                return true;
            }
            return false;
        }

        public Token<TokenTypeT> ConsumeNext ( SourceCodeReader reader )
        {
            if ( this.IsLiteral )
            {
                Common.SourceLocation start = reader.Location;
                reader.Advance ( this.Prefix.Length );
                return new Token<TokenTypeT> ( this.ID, this.Prefix, this.Converter != null ? this.Converter ( this.Prefix ) : this.Prefix, this.Type, start.To ( reader.Location ) );
            }
            else if ( this.StoredResult != null )
            {
                reader.Rewind ( this.End );
                return new Token<TokenTypeT> ( this.ID, this.StoredResult, this.Converter != null ? this.Converter ( this.StoredResult ) : this.StoredResult, this.Type, this.Start.To ( this.End ) );
            }
            else
                throw new InvalidOperationException ( "Cannot consume a token when check wasn't successful." );
        }
    }

    public static class ILexerBuilderRegexExtensions
    {
        public static void AddRegex<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String expression, Func<String, Object> converter = null )
            where TokenTypeT : IEquatable<TokenTypeT>
        {
            builder.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, expression, converter ) );
        }
    }
}
