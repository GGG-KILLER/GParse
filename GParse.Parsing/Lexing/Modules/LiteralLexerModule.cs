using System;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing.Modules
{
    public class LiteralLexerModule<TokenTypeT> : ILexerModule<TokenTypeT> where TokenTypeT : Enum
    {
        private readonly String ID;
        private readonly TokenTypeT Type;

        public String Name => $"Literal Module: '{this.Prefix}'";
        public String Prefix { get; }

        public LiteralLexerModule ( String ID, TokenTypeT type, String raw )
        {
            this.ID = ID;
            this.Type = type;
            this.Prefix = raw;
        }

        public Boolean CanConsumeNext ( SourceCodeReader reader ) => true;

        public Token<TokenTypeT> ConsumeNext ( SourceCodeReader reader )
        {
            Common.SourceLocation start = reader.Location;
            reader.Advance ( this.Prefix.Length );
            return new Token<TokenTypeT> ( this.ID, this.Prefix, this.Prefix, this.Type, start.To ( reader.Location ) );
        }
    }

    public static class ILexerBuilderLiteralExtensions
    {
        public static void AddLiteral<TokenTypeT> ( this ILexerBuilder<TokenTypeT> builder, String ID, TokenTypeT type, String raw )
            where TokenTypeT : Enum => builder.AddModule ( new LiteralLexerModule<TokenTypeT> ( ID, type, raw ) );
    }
}
