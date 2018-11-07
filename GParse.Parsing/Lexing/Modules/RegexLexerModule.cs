using System;
using System.Text.RegularExpressions;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing.Modules
{
    public class RegexLexerModule<TokenTypeT> : ILexerModule<TokenTypeT> where TokenTypeT : Enum
    {
        private readonly String ID;
        private readonly TokenTypeT Type;
        private readonly String Expression;
        private readonly Func<Match, Object> Converter;
        private readonly Boolean IsTrivia;

        public String Name => $"Regex Lexer Module: {this.Expression}";
        public String Prefix { get; }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex, String prefix = null, Func<Match, Object> converter = null, Boolean isTrivia = false )
        {
            this.Converter  = converter;
            this.Expression = regex;
            this.ID         = ID;
            this.IsTrivia   = isTrivia;
            this.Prefix     = prefix;
            this.Type       = type;
        }

        // Ideally modules should be stateless, but read
        // CanConsumeNext for the explanation.
        private Common.SourceLocation Start;
        private Common.SourceLocation End;
        private Match StoredResult;

        public Boolean CanConsumeNext ( SourceCodeReader reader )
        {
            // Ideally CanConsumeNext should not leave the reader
            // modified, but in this case since we'll be called in
            // sequence, it's good to not re-execute the matching again.
            this.Start = reader.Location;
            this.StoredResult = null;
            Match res = reader.MatchRegex ( this.Expression );
            if ( res.Success )
            {
                this.End = reader.Location;
                this.StoredResult = res;

                reader.Rewind ( this.Start );
                return true;
            }

            reader.Rewind ( this.Start );
            return false;
        }

        public Token<TokenTypeT> ConsumeNext ( SourceCodeReader reader )
        {
            if ( this.StoredResult != null )
            {
                reader.Rewind ( this.End );
                return new Token<TokenTypeT> ( this.ID, this.StoredResult.Value, this.Converter != null ? this.Converter ( this.StoredResult ) : this.StoredResult.Value, this.Type, this.Start.To ( this.End ), this.IsTrivia );
            }
            else
                throw new InvalidOperationException ( "Cannot consume a token when check wasn't successful." );
        }
    }
}
