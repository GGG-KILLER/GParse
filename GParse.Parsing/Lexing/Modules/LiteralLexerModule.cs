using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace GParse.Parsing.Lexing.Modules
{
    public class LiteralLexerModule : ILexerModule
    {
        private readonly String ID;
        private readonly TokenType Type;

        public String Name => $"Literal Module: '{this.Prefix}'";
        public String Prefix { get; }

        public LiteralLexerModule ( String ID, TokenType type, String raw )
        {
            this.ID = ID;
            this.Type = type;
            this.Prefix = raw;
        }

        public Boolean CanConsumeNext ( SourceCodeReader reader ) => true;

        public Token ConsumeNext ( SourceCodeReader reader )
        {
            Common.SourceLocation start = reader.Location;
            reader.Advance ( this.Prefix.Length );
            return new Token ( this.ID, this.Prefix, this.Prefix, this.Type, start.To ( reader.Location ) );
        }
    }

    public static class ILexerBuilderLiteralExtensions
    {
        public static void AddLiteral ( this ILexerBuilder builder, String ID, TokenType type, String raw )
        {
            builder.AddModule ( new LiteralLexerModule ( ID, type, raw ) );
        }
    }
}
