using System;
using System.Text.RegularExpressions;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing.Composable;
using GParse.Math;
using Tsu;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// The <see cref="TokenLexerDelegate{T}"/> lexer builder.
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public class DelegateLexerBuilder<TTokenType>
        where TTokenType : notnull
    {
        private readonly TTokenType _eofTokenType;

        /// <inheritdoc cref="LexerModuleTree{TTokenType}.FallbackModule"/>
        public TokenLexerDelegate<TTokenType>? Fallback
        {
            get => Delegates.FallbackDelegate;
            set => Delegates.FallbackDelegate = value;
        }

        /// <summary>
        /// The lexer module tree.
        /// </summary>
        protected LexerDelegateTree<TTokenType> Delegates { get; } = new();

        /// <summary>
        /// Initializes a new modular lexer builder.
        /// </summary>
        /// <param name="eofTokenType"></param>
        public DelegateLexerBuilder(TTokenType eofTokenType)
        {
            _eofTokenType = eofTokenType;
        }

        /// <summary>
        /// Adds a delegate to the lexer (affects existing instances).
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="lexerDelegate"></param>
        public virtual void AddDelegate(string? prefix, TokenLexerDelegate<TTokenType> lexerDelegate)
        {
            if (lexerDelegate is null) throw new ArgumentNullException(nameof(lexerDelegate));
            Delegates.AddChild(prefix, lexerDelegate);
        }

        /// <summary>
        /// Removes all modules with the specified prefix.
        /// </summary>
        /// <param name="prefix"></param>
        public virtual void ClearPrefix(string? prefix) => Delegates.ClearPrefix(prefix);

        #region AddLiteral

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="text">The raw value of the token</param>
        public virtual void AddLiteral(string id, TTokenType type, string text)
        {
            AddDelegate(text, (reader, diagnostics) =>
            {
                var start = reader.Position;
                reader.Advance(text.Length);
                return new Token<TTokenType>(id, type, new Range<int>(start, reader.Position), null, text);
            });
        }

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="text">The raw value of the token</param>
        /// <param name="isTrivia">
        /// Whether this token is considered trivia (will not show up in the enumerated token sequence but
        /// inside <see cref="Token{TTokenType}.Trivia" /> instead)
        /// </param>
        public virtual void AddLiteral(string id, TTokenType type, string text, bool isTrivia)
        {
            AddDelegate(text, (reader, diagnostics) =>
            {
                var start = reader.Position;
                reader.Advance(text.Length);
                return new Token<TTokenType>(
                    id,
                    type,
                    new Range<int>(start, reader.Position),
                    isTrivia,
                    (object?) null,
                    text);
            });
        }

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="text">The raw value of the token</param>
        /// <param name="value">The value of this token</param>
        public virtual void AddLiteral(string id, TTokenType type, string text, object? value)
        {
            AddDelegate(text, (reader, diagnostics) =>
            {
                var start = reader.Position;
                reader.Advance(text.Length);
                return new Token<TTokenType>(
                    id,
                    type,
                    new Range<int>(start, reader.Position),
                    value,
                    text);
            });
        }

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="text">The raw value of the token</param>
        /// <param name="value">The value of this token</param>
        /// <param name="isTrivia">
        /// Whether this token is considered trivia (will not show up in the enumerated token sequence but
        /// inside <see cref="Token{TTokenType}.Trivia" /> instead)
        /// </param>
        public virtual void AddLiteral(string id, TTokenType type, string text, object? value, bool isTrivia)
        {
            AddDelegate(text, (reader, diagnostics) =>
            {
                var start = reader.Position;
                reader.Advance(text.Length);
                return new Token<TTokenType>(
                    id,
                    type,
                    new Range<int>(start, reader.Position),
                    isTrivia,
                    value,
                    text);
            });
        }

        #endregion AddLiteral

        #region AddRegex

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        public virtual void AddRegex(string id, TTokenType type, string regex)
        {
            AddDelegate(null, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var result = reader.PeekRegex(regex);
                if (result.Success)
                {
                    reader.Advance(result.Length);
                    return new Token<TTokenType>(
                        id,
                        type,
                        new Range<int>(start, reader.Position),
                        null,
                        result.Value);
                }

                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        public virtual void AddRegex(string id, TTokenType type, Regex regex)
        {
            AddDelegate(null, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var result = reader.PeekRegex(regex);
                if (result.Success)
                {
                    reader.Advance(result.Length);
                    return new Token<TTokenType>(
                        id,
                        type,
                        new Range<int>(start, reader.Position),
                        null,
                        result.Value);
                }

                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        public virtual void AddRegex(string id, TTokenType type, string regex, string? prefix)
        {
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var result = reader.PeekRegex(regex);
                if (result.Success)
                {
                    reader.Advance(result.Length);
                    return new Token<TTokenType>(
                        id,
                        type,
                        new Range<int>(start, reader.Position),
                        null,
                        result.Value);
                }

                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        public virtual void AddRegex(string id, TTokenType type, Regex regex, string? prefix)
        {
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var result = reader.PeekRegex(regex);
                if (result.Success)
                {
                    reader.Advance(result.Length);
                    return new Token<TTokenType>(
                        id,
                        type,
                        new Range<int>(start, reader.Position),
                        null,
                        result.Value);
                }

                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        /// <param name="converter">The function to convert the raw value into a desired type</param>
        public virtual void AddRegex(
            string id,
            TTokenType type,
            string regex,
            string? prefix,
            Func<Match, DiagnosticList, Option<object>> converter)
        {
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var result = reader.PeekRegex(regex);
                if (result.Success)
                {
                    var value = converter(result, diagnostics);
                    if (value.IsSome)
                    {
                        reader.Advance(result.Length);
                        return new Token<TTokenType>(
                            id,
                            type,
                            new Range<int>(start, reader.Position),
                            value.Value,
                            result.Value);
                    }
                }

                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        /// <param name="converter">The function to convert the raw value into a desired type</param>
        public virtual void AddRegex(
            string id,
            TTokenType type,
            Regex regex,
            string? prefix,
            Func<Match, DiagnosticList, Option<object>> converter)
        {
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var result = reader.PeekRegex(regex);
                if (result.Success)
                {
                    var value = converter(result, diagnostics);
                    if (value.IsSome)
                    {
                        reader.Advance(result.Length);
                        return new Token<TTokenType>(
                            id,
                            type,
                            new Range<int>(start, reader.Position),
                            value.Value,
                            result.Value);
                    }
                }

                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        /// <param name="converter">The function to convert the raw value into a desired type</param>
        /// <param name="isTrivia">
        /// Whether this token is considered trivia (will not show up in the enumerated token sequence but
        /// inside <see cref="Token{TTokenType}.Trivia" /> instead)
        /// </param>
        public virtual void AddRegex(
            string id,
            TTokenType type,
            string regex,
            string? prefix,
            Func<Match, DiagnosticList, Option<object>> converter,
            bool isTrivia)
        {
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var result = reader.PeekRegex(regex);
                if (result.Success)
                {
                    var value = converter(result, diagnostics);
                    if (value.IsSome)
                    {
                        reader.Advance(result.Length);
                        return new Token<TTokenType>(
                            id,
                            type,
                            new Range<int>(start, reader.Position),
                            isTrivia,
                            value.Value,
                            result.Value);
                    }
                }

                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        /// <param name="converter">The function to convert the raw value into a desired type</param>
        /// <param name="isTrivia">
        /// Whether this token is considered trivia (will not show up in the enumerated token sequence but
        /// inside <see cref="Token{TTokenType}.Trivia" /> instead)
        /// </param>
        public virtual void AddRegex(
            string id,
            TTokenType type,
            Regex regex,
            string? prefix,
            Func<Match, DiagnosticList, Option<object>> converter,
            bool isTrivia)
        {
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var result = reader.PeekRegex(regex);
                if (result.Success)
                {
                    var value = converter(result, diagnostics);
                    if (value.IsSome)
                    {
                        reader.Advance(result.Length);
                        return new Token<TTokenType>(
                            id,
                            type,
                            new Range<int>(start, reader.Position),
                            value.Value,
                            result.Value);
                    }
                }

                return Option.None<Token<TTokenType>>();
            });
        }

        #endregion AddRegex

        #region AddGrammar

        /// <summary>
        /// Defines a token as a grammar tree.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="spanTokenFactory"></param>
        public virtual void AddGrammar(GrammarNode<char> node, GrammarTreeLexerModule<TTokenType>.SpanTokenFactory spanTokenFactory)
        {
            var tree = GrammarTreeOptimizer.Optimize(node) ?? node;
            var prefix = GrammarTreePrefixObtainer.Calculate(tree);
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var match = GrammarTreeInterpreter.MatchSpan(reader, tree);
                if (match.IsMatch)
                    return spanTokenFactory(start, match, diagnostics);
                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a grammar tree.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="stringTokenFactory"></param>
        public virtual void AddGrammar(GrammarNode<char> node, GrammarTreeLexerModule<TTokenType>.StringTokenFactory stringTokenFactory)
        {
            var tree = GrammarTreeOptimizer.Optimize(node) ?? node;
            var prefix = GrammarTreePrefixObtainer.Calculate(tree);
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var match = GrammarTreeInterpreter.MatchString(reader, tree);
                if (match.IsMatch)
                    return stringTokenFactory(start, match, diagnostics);
                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a regex grammar tree.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="spanTokenFactory">The function responsible for converting a match into a token.</param>
        public virtual void AddGrammar(
            string regex,
            GrammarTreeLexerModule<TTokenType>.SpanTokenFactory spanTokenFactory)
        {
            var node = RegexParser.Parse(regex);
            var tree = GrammarTreeOptimizer.Optimize(node) ?? node;
            var prefix = GrammarTreePrefixObtainer.Calculate(tree);
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var match = GrammarTreeInterpreter.MatchSpan(reader, tree);
                if (match.IsMatch)
                    return spanTokenFactory(start, match, diagnostics);
                return Option.None<Token<TTokenType>>();
            });
        }

        /// <summary>
        /// Defines a token as a regex grammar tree.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="stringTokenFactory">The function responsible for converting a match into a token.</param>
        public virtual void AddGrammar(
            string regex,
            GrammarTreeLexerModule<TTokenType>.StringTokenFactory stringTokenFactory)
        {
            var node = RegexParser.Parse(regex);
            var tree = GrammarTreeOptimizer.Optimize(node) ?? node;
            var prefix = GrammarTreePrefixObtainer.Calculate(tree);
            AddDelegate(prefix, (reader, diagnostics) =>
            {
                var start = reader.Position;
                var match = GrammarTreeInterpreter.MatchString(reader, tree);
                if (match.IsMatch)
                    return stringTokenFactory(start, match, diagnostics);
                return Option.None<Token<TTokenType>>();
            });
        }

        #endregion AddGrammar

        /// <summary>
        /// Obtains a lexer for the provided <paramref name="input"/> and <paramref name="diagnostics"/>.
        /// </summary>
        /// <param name="input">The input to be used by the lexer.</param>
        /// <param name="diagnostics">The diagnostic list to be used by the lexer.</param>
        /// <returns>The built lexer.</returns>
        public virtual ILexer<TTokenType> GetLexer(string input, DiagnosticList diagnostics) =>
            GetLexer(new StringCodeReader(input), diagnostics);

        /// <summary>
        /// Obtains a lexer for the provided <paramref name="reader"/> and <paramref name="diagnostics"/>.
        /// </summary>
        /// <param name="reader">The reader to be used by the lexer.</param>
        /// <param name="diagnostics">The diagnostic list to be used by the lexer.</param>
        /// <returns></returns>
        public virtual ILexer<TTokenType> GetLexer(ICodeReader reader, DiagnosticList diagnostics) =>
            new DelegateLexer<TTokenType>(Delegates, _eofTokenType, reader, diagnostics);
    }
}
