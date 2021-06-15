using System;
using System.Text.RegularExpressions;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing.Composable;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// Defines a <see cref="ILexer{TTokenType}" /> builder
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public class ModularLexerBuilder<TTokenType>
        where TTokenType : notnull
    {
        private readonly TTokenType _eofTokenType;

        /// <inheritdoc cref="LexerModuleTree{TTokenType}.FallbackModule"/>
        public ILexerModule<TTokenType>? Fallback
        {
            get => this.Modules.FallbackModule;
            set => this.Modules.FallbackModule = value;
        }

        /// <summary>
        /// The lexer module tree.
        /// </summary>
        protected LexerModuleTree<TTokenType> Modules { get; } = new LexerModuleTree<TTokenType>();

        /// <summary>
        /// Initializes a new modular lexer builder.
        /// </summary>
        /// <param name="eofTokenType"></param>
        public ModularLexerBuilder(TTokenType eofTokenType)
        {
            this._eofTokenType = eofTokenType;
        }

        /// <summary>
        /// Adds a module to the lexer (affects existing instances)
        /// </summary>
        /// <param name="lexerModule"></param>
        public virtual void AddModule(ILexerModule<TTokenType> lexerModule)
        {
            if (lexerModule is null)
                throw new ArgumentNullException(nameof(lexerModule));
            Modules.AddChild(lexerModule);
        }

        /// <summary>
        /// Removes an module from the lexer (affects existing instances)
        /// </summary>
        /// <param name="lexerModule"></param>
        public virtual void RemoveModule(ILexerModule<TTokenType> lexerModule)
        {
            if (lexerModule is null)
                throw new ArgumentNullException(nameof(lexerModule));
            Modules.RemoveChild(lexerModule);
        }

        #region AddLiteral

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        public virtual void AddLiteral(string id, TTokenType type, string raw) =>
            AddModule(new LiteralLexerModule<TTokenType>(id, type, false, raw, raw));

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        /// <param name="isTrivia">
        /// Whether this token is considered trivia (will not show up in the enumerated token sequence but
        /// inside <see cref="Token{TTokenType}.Trivia" /> instead)
        /// </param>
        public virtual void AddLiteral(string id, TTokenType type, string raw, bool isTrivia) =>
            AddModule(new LiteralLexerModule<TTokenType>(id, type, isTrivia, raw, raw));

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        /// <param name="value">The value of this token</param>
        public virtual void AddLiteral(string id, TTokenType type, string raw, object? value) =>
            this.AddModule(new LiteralLexerModule<TTokenType>(id, type, false, value, raw));

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        /// <param name="value">The value of this token</param>
        /// <param name="isTrivia">
        /// Whether this token is considered trivia (will not show up in the enumerated token sequence but
        /// inside <see cref="Token{TTokenType}.Trivia" /> instead)
        /// </param>
        public virtual void AddLiteral(string id, TTokenType type, string raw, object? value, bool isTrivia) =>
            this.AddModule(new LiteralLexerModule<TTokenType>(id, type, isTrivia, value, raw));

        #endregion AddLiteral

        #region AddRegex

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        public virtual void AddRegex(string id, TTokenType type, string regex) =>
            this.AddModule(new RegexLexerModule<TTokenType>(id, type, regex, null, null, false));

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        public virtual void AddRegex(string id, TTokenType type, Regex regex) =>
            this.AddModule(new RegexLexerModule<TTokenType>(id, type, regex, null, null, false));

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        public virtual void AddRegex(string id, TTokenType type, string regex, string? prefix) =>
            this.AddModule(new RegexLexerModule<TTokenType>(id, type, regex, prefix, null, false));

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        public virtual void AddRegex(string id, TTokenType type, Regex regex, string? prefix) =>
            this.AddModule(new RegexLexerModule<TTokenType>(id, type, regex, prefix, null, false));

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        /// <param name="converter">The function to convert the raw value into a desired type</param>
        public virtual void AddRegex(string id, TTokenType type, string regex, string? prefix, Func<Match, DiagnosticList, object>? converter) =>
            this.AddModule(new RegexLexerModule<TTokenType>(id, type, regex, prefix, converter, false));

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        /// <param name="converter">The function to convert the raw value into a desired type</param>
        public virtual void AddRegex(string id, TTokenType type, Regex regex, string? prefix, Func<Match, DiagnosticList, object>? converter) =>
            this.AddModule(new RegexLexerModule<TTokenType>(id, type, regex, prefix, converter, false));

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
        public virtual void AddRegex(string id, TTokenType type, string regex, string? prefix, Func<Match, DiagnosticList, object>? converter, bool isTrivia) =>
            this.AddModule(new RegexLexerModule<TTokenType>(id, type, regex, prefix, converter, isTrivia));

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
        public virtual void AddRegex(string id, TTokenType type, Regex regex, string? prefix, Func<Match, DiagnosticList, object>? converter, bool isTrivia) =>
            this.AddModule(new RegexLexerModule<TTokenType>(id, type, regex, prefix, converter, isTrivia));

        #endregion AddRegex

        #region AddGrammar

        /// <summary>
        /// Defines a token as a grammar tree.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="spanTokenFactory"></param>
        public virtual void AddGrammar(GrammarNode<char> node, GrammarTreeLexerModule<TTokenType>.SpanTokenFactory spanTokenFactory) =>
            this.AddModule(new GrammarTreeLexerModule<TTokenType>(node, spanTokenFactory));

        /// <summary>
        /// Defines a token as a grammar tree.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="stringTokenFactory"></param>
        public virtual void AddGrammar(GrammarNode<char> node, GrammarTreeLexerModule<TTokenType>.StringTokenFactory stringTokenFactory) =>
            this.AddModule(new GrammarTreeLexerModule<TTokenType>(node, stringTokenFactory));

        /// <summary>
        /// Defines a token as a regex grammar tree.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="spanTokenFactory">The function responsible for converting a match into a token.</param>
        public virtual void AddGrammar(string regex, GrammarTreeLexerModule<TTokenType>.SpanTokenFactory spanTokenFactory) =>
            this.AddGrammar(RegexParser.Parse(regex), spanTokenFactory);

        /// <summary>
        /// Defines a token as a regex grammar tree.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="stringTokenFactory">The function responsible for converting a match into a token.</param>
        public virtual void AddGrammar(string regex, GrammarTreeLexerModule<TTokenType>.StringTokenFactory stringTokenFactory) =>
            this.AddGrammar(RegexParser.Parse(regex), stringTokenFactory);

        #endregion AddGrammar

        /// <summary>
        /// Obtains a lexer for the provided <paramref name="input"/> and <paramref name="diagnostics"/>.
        /// </summary>
        /// <param name="input">The input to be used by the lexer.</param>
        /// <param name="diagnostics">The diagnostic list to be used by the lexer.</param>
        /// <returns>The built lexer.</returns>
        public virtual ILexer<TTokenType> GetLexer(string input, DiagnosticList diagnostics) =>
            this.GetLexer(new StringCodeReader(input), diagnostics);

        /// <summary>
        /// Obtains a lexer for the provided <paramref name="reader"/> and <paramref name="diagnostics"/>.
        /// </summary>
        /// <param name="reader">The reader to be used by the lexer.</param>
        /// <param name="diagnostics">The diagnostic list to be used by the lexer.</param>
        /// <returns></returns>
        public virtual ILexer<TTokenType> GetLexer(ICodeReader reader, DiagnosticList diagnostics) =>
            new ModularLexer<TTokenType>(this.Modules, this._eofTokenType, reader, diagnostics);
    }
}