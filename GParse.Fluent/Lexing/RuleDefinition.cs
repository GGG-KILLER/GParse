using System;
using GParse.Common.Lexing;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Lexing
{
    public struct RuleDefinition
    {
        public readonly String Name;
        public readonly TokenType Type;
        public readonly BaseMatcher Body;
        public readonly Func<String, Object> Converter;

        public RuleDefinition ( String name, BaseMatcher body, TokenType type = TokenType.Other, Func<String, Object> converter = null )
        {
            if ( String.IsNullOrEmpty ( name ) )
                throw new ArgumentException ( "Rule name cannot be null or empty.", nameof ( name ) );
            this.Name = name;
            this.Body = body ?? throw new ArgumentNullException ( nameof ( body ) );
            this.Type = type;
            this.Converter = converter;
        }
    }
}
