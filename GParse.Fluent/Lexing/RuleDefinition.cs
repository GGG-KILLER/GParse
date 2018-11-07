using System;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Lexing
{
    public struct RuleDefinition<TokenTypeT> where TokenTypeT : Enum
    {
        public readonly String Name;
        public readonly TokenTypeT Type;
        public readonly BaseMatcher Body;
        public readonly Func<String, Object> Converter;

        public RuleDefinition ( String name, BaseMatcher body, TokenTypeT type, Func<String, Object> converter = null )
        {
            if ( String.IsNullOrEmpty ( name ) )
                throw new ArgumentException ( "Rule name cannot be null or empty.", nameof ( name ) );
            this.Name      = name;
            this.Body      = body ?? throw new ArgumentNullException ( nameof ( body ) );
            this.Type      = type;
            this.Converter = converter;
        }
    }
}
