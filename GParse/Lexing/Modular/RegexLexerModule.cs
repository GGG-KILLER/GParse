using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using GParse.IO;
using GParse.Math;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// A module that defines a token through a regex pattern
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class RegexLexerModule<TokenTypeT> : ILexerModule<TokenTypeT>
        where TokenTypeT : notnull
    {
        private readonly String _id;
        private readonly TokenTypeT _type;
        private readonly String? _expression;
        private readonly Regex? _regex;
        private readonly Func<Match, Object>? _converter;
        private readonly Boolean _isTrivia;

        /// <inheritdoc />
        public String Name => $"Regex Lexer Module: {this._expression}";

        /// <inheritdoc />
        public String? Prefix { get; }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        /// <param name="isTrivia"></param>
        public RegexLexerModule ( String id, TokenTypeT type, String regex, String? prefix, Func<Match, Object>? converter, Boolean isTrivia )
        {
            this._converter = converter;
            this._expression = regex;
            this._id = id;
            this._isTrivia = isTrivia;
            this.Prefix = prefix;
            this._type = type;
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        /// <param name="isTrivia"></param>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex, String? prefix, Func<Match, Object>? converter, Boolean isTrivia )
        {
            this._converter = converter;
            this._regex = regex;
            this._id = id;
            this._isTrivia = isTrivia;
            this.Prefix = prefix;
            this._type = type;
        }

        /// <inheritdoc/>
        public Boolean TryConsume ( ICodeReader reader, DiagnosticList diagnostics, [NotNullWhen ( true )] out Token<TokenTypeT>? token )
        {
            var start = reader.Position;
            Match? result = this._expression != null ? reader.PeekRegex ( this._expression ) : reader.PeekRegex ( this._regex! );
            if ( result.Success )
            {
                reader.Advance ( result.Length );
                token = new Token<TokenTypeT> (
                    this._id,
                    this._type,
                    new Range<Int32> ( start, reader.Position ),
                    this._isTrivia,
                    this._converter != null ? this._converter ( result ) : result.Value,
                    result.Value );
                return true;
            }

            token = null;
            return false;
        }
    }
}
