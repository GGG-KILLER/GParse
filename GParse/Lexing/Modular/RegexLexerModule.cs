using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using GParse.IO;
using GParse.Math;
using Tsu;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// A module that defines a token through a regex pattern
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public sealed class RegexLexerModule<TTokenType> : ILexerModule<TTokenType>
        where TTokenType : notnull
    {
        private readonly string _id;
        private readonly TTokenType _type;
        private readonly string? _expression;
        private readonly Regex? _regex;
        private readonly Func<Match, DiagnosticList, object>? _converter;
        private readonly bool _isTrivia;

        /// <inheritdoc />
        public string Name => $"Regex Lexer Module: {_expression}";

        /// <inheritdoc />
        public string? Prefix { get; }

        private RegexLexerModule(string id, TTokenType type, string? prefix, Func<Match, DiagnosticList, object>? converter, bool isTrivia)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _type = type ?? throw new ArgumentNullException(nameof(type));
            Prefix = prefix;
            _converter = converter;
            _isTrivia = isTrivia;
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TTokenType}" />
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
        public RegexLexerModule(string id, TTokenType type, string regex, string? prefix, Func<Match, DiagnosticList, object>? converter, bool isTrivia)
            : this(id, type, prefix, converter, isTrivia)
        {
            if (string.IsNullOrEmpty(regex))
                throw new ArgumentException($"'{nameof(regex)}' cannot be null or empty", nameof(regex));

            _expression = regex;
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TTokenType}" />
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
        public RegexLexerModule(string id, TTokenType type, Regex regex, string? prefix, Func<Match, DiagnosticList, object>? converter, bool isTrivia)
            : this(id, type, prefix, converter, isTrivia)
        {
            _regex = regex ?? throw new ArgumentNullException(nameof(regex));
        }

        /// <inheritdoc/>
        public Option<Token<TTokenType>> TryConsume(ICodeReader reader, DiagnosticList diagnostics)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var start = reader.Position;
            var result = _expression != null ? reader.PeekRegex(_expression) : reader.PeekRegex(_regex!);
            if (result.Success)
            {
                reader.Advance(result.Length);
                return new Token<TTokenType>(
                    _id,
                    _type,
                    new Range<int>(start, reader.Position),
                    _isTrivia,
                    _converter != null ? _converter(result, diagnostics) : result.Value,
                    result.Value);
            }

            return default;
        }
    }
}