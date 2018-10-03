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
        private readonly Boolean IsLiteral;
        private readonly Func<Match, Object> Converter;
        private readonly Boolean IsTrivia;

        public String Name { get; }
        public String Prefix { get; }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex ) : this ( ID, type, regex, "", null, false )
        {
        }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex, Boolean isTrivia ) : this ( ID, type, regex, "", null, isTrivia )
        {
        }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex, Func<Match, Object> converter ) : this ( ID, type, regex, "", converter, false )
        {
        }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex, Func<Match, Object> converter, Boolean isTrivia ) : this ( ID, type, regex, "", converter, isTrivia )
        {
        }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex, String prefix ) : this ( ID, type, regex, prefix, null, false )
        {
        }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex, String prefix, Boolean isTrivia ) : this ( ID, type, regex, prefix, null, isTrivia )
        {
        }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex, String prefix, Func<Match, Object> converter ) : this ( ID, type, regex, prefix, converter, false )
        {
        }

        public RegexLexerModule ( String ID, TokenTypeT type, String regex, String prefix, Func<Match, Object> converter, Boolean isTrivia )
        {
            this.ID = ID;
            this.Type = type;
            this.Expression = regex;
            this.Converter = converter;
            this.IsTrivia = isTrivia;
            this.Prefix = prefix;
        }

        // Ideally modules should be stateless, but read
        // CanConsumeNext for the explanation.
        private Common.SourceLocation Start;
        private Common.SourceLocation End;
        private Match StoredResult;

        public Boolean CanConsumeNext ( SourceCodeReader reader )
        {
            if ( this.IsLiteral )
                return true;

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

    public static class ILexerBuilderRegexExtensions
    {
        public static void AddRegex<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String regex ) where TokenTypeT : Enum =>
            builder.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, regex ) );

        public static void AddRegex<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String regex, Boolean isTrivia ) where TokenTypeT : Enum =>
            builder.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, regex, isTrivia ) );

        public static void AddRegex<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String regex, Func<Match, Object> converter ) where TokenTypeT : Enum =>
            builder.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, regex, converter ) );

        public static void AddRegex<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String regex, Func<Match, Object> converter, Boolean isTrivia ) where TokenTypeT : Enum =>
            builder.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, regex, converter, isTrivia ) );

        public static void AddRegex<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String regex, String prefix ) where TokenTypeT : Enum =>
            builder.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, regex, prefix ) );

        public static void AddRegex<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String regex, String prefix, Boolean isTrivia ) where TokenTypeT : Enum =>
            builder.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, regex, prefix, isTrivia ) );

        public static void AddRegex<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String regex, String prefix, Func<Match, Object> converter ) where TokenTypeT : Enum =>
            builder.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, regex, prefix, converter ) );

        public static void AddRegex<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String regex, String prefix, Func<Match, Object> converter, Boolean isTrivia ) where TokenTypeT : Enum =>
            builder.AddModule ( new RegexLexerModule<TokenTypeT> ( ID, type, regex, prefix, converter, isTrivia ) );
    }
}
