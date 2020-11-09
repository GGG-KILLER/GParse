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
    public sealed class RegexLexerModule<TokenTypeT> : ILexerModule<TokenTypeT>
        where TokenTypeT : notnull
    {
        private readonly String _id;
        private readonly TokenTypeT _type;
        private readonly String? _expression;
        private readonly Regex? _regex;
        private readonly Func<Match, DiagnosticList, Object>? _converter;
        private readonly Boolean _isTrivia;

        /// <inheritdoc />
        public String Name => $"Regex Lexer Module: {this._expression}";

        /// <inheritdoc />
        public String? Prefix { get; }

        private RegexLexerModule ( String id, TokenTypeT type, String? prefix, Func<Match, DiagnosticList, Object>? converter, Boolean isTrivia )
        {
            this._id = id ?? throw new ArgumentNullException ( nameof ( id ) );
            this._type = type ?? throw new ArgumentNullException ( nameof ( type ) );
            this.Prefix = prefix;
            this._converter = converter;
            this._isTrivia = isTrivia;
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
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown when <paramref name="id"/> is null;</para>
        /// -or-
        /// <para>or <paramref name="type"/> is null.</para>
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="regex"/> is null or empty.</exception>
        public RegexLexerModule ( String id, TokenTypeT type, String regex, String? prefix, Func<Match, DiagnosticList, Object>? converter, Boolean isTrivia )
            : this ( id, type, prefix, converter, isTrivia )
        {
            if ( String.IsNullOrEmpty ( regex ) )
                throw new ArgumentException ( $"'{nameof ( regex )}' cannot be null or empty", nameof ( regex ) );

            this._expression = regex;
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
        /// <exception cref="ArgumentNullException">
        /// <para>Thrown when <paramref name="id"/> is null;</para>
        /// -or-
        /// <para>or <paramref name="type"/> is null;</para>
        /// -or-
        /// <para>or <paramref name="regex"/> is null.</para>
        /// </exception>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex, String? prefix, Func<Match, DiagnosticList, Object>? converter, Boolean isTrivia )
            : this ( id, type, prefix, converter, isTrivia )
        {
            this._regex = regex ?? throw new ArgumentNullException ( nameof ( regex ) );
        }

        /// <inheritdoc/>
        public Boolean TryConsume ( ICodeReader reader, DiagnosticList diagnostics, [NotNullWhen ( true )] out Token<TokenTypeT>? token )
        {
            if ( reader is null )
                throw new ArgumentNullException ( nameof ( reader ) );

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
                    this._converter != null ? this._converter ( result, diagnostics ) : result.Value,
                    result.Value );
                return true;
            }

            token = null;
            return false;
        }
    }
}
