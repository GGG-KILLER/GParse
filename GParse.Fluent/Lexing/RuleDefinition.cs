using System;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Lexing
{
    /// <summary>
    /// Defines a rule
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public readonly struct RuleDefinition<TokenTypeT> where TokenTypeT : Enum
    {
        /// <summary>
        /// The name of the rule
        /// </summary>
        public readonly String Name;

        /// <summary>
        /// The type of the token that this rule matches
        /// </summary>
        public readonly TokenTypeT Type;

        /// <summary>
        /// The body of the rule
        /// </summary>
        public readonly BaseMatcher Body;

        /// <summary>
        /// The function that converts the contents into a desired format
        /// </summary>
        public readonly Func<String, Object> Converter;

        /// <summary>
        /// Creates a rule definition
        /// </summary>
        /// <param name="name"></param>
        /// <param name="body"></param>
        /// <param name="type"></param>
        /// <param name="converter"></param>
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
