using System;
using System.Text.RegularExpressions;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing.Composable;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// Defines a <see cref="ILexer{TokenTypeT}" /> builder
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class ModularLexerBuilder<TokenTypeT> : ILexerFactory<TokenTypeT>
        where TokenTypeT : notnull
    {
        private readonly TokenTypeT _eofTokenType;

        /// <inheritdoc cref="LexerModuleTree{TokenTypeT}.FallbackModule"/>
        public ILexerModule<TokenTypeT>? Fallback
        {
            get => this.Modules.FallbackModule;
            set => this.Modules.FallbackModule = value;
        }

        /// <summary>
        /// The lexer module tree.
        /// </summary>
        protected LexerModuleTree<TokenTypeT> Modules { get; } = new LexerModuleTree<TokenTypeT> ( );

        /// <summary>
        /// Initializes a new modular lexer builder.
        /// </summary>
        /// <param name="eofTokenType"></param>
        public ModularLexerBuilder ( TokenTypeT eofTokenType )
        {
            this._eofTokenType = eofTokenType;
        }

        /// <summary>
        /// Adds a module to the lexer (affects existing instances)
        /// </summary>
        /// <param name="lexerModule"></param>
        public virtual void AddModule ( ILexerModule<TokenTypeT> lexerModule )
        {
            if ( lexerModule is null )
                throw new ArgumentNullException ( nameof ( lexerModule ) );
            this.Modules.AddChild ( lexerModule );
        }

        /// <summary>
        /// Removes an module from the lexer (affects existing instances)
        /// </summary>
        /// <param name="lexerModule"></param>
        public virtual void RemoveModule ( ILexerModule<TokenTypeT> lexerModule )
        {
            if ( lexerModule is null )
                throw new ArgumentNullException ( nameof ( lexerModule ) );
            this.Modules.RemoveChild ( lexerModule );
        }

        #region AddLiteral

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        public virtual void AddLiteral ( String id, TokenTypeT type, String raw ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( id, type, false, raw, raw ) );

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        /// <param name="isTrivia">
        /// Whether this token is considered trivia (will not show up in the enumerated token sequence but
        /// inside <see cref="Token{TokenTypeT}.Trivia" /> instead)
        /// </param>
        public virtual void AddLiteral ( String id, TokenTypeT type, String raw, Boolean isTrivia ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( id, type, isTrivia, raw, raw ) );

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        /// <param name="value">The value of this token</param>
        public virtual void AddLiteral ( String id, TokenTypeT type, String raw, Object? value ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( id, type, false, value, raw ) );

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        /// <param name="value">The value of this token</param>
        /// <param name="isTrivia">
        /// Whether this token is considered trivia (will not show up in the enumerated token sequence but
        /// inside <see cref="Token{TokenTypeT}.Trivia" /> instead)
        /// </param>
        public virtual void AddLiteral ( String id, TokenTypeT type, String raw, Object? value, Boolean isTrivia ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( id, type, isTrivia, value, raw ) );

        #endregion AddLiteral

        #region AddRegex

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        public virtual void AddRegex ( String id, TokenTypeT type, String regex ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, null, null, false ) );

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        public virtual void AddRegex ( String id, TokenTypeT type, Regex regex ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, null, null, false ) );

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        public virtual void AddRegex ( String id, TokenTypeT type, String regex, String? prefix ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, prefix, null, false ) );

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        public virtual void AddRegex ( String id, TokenTypeT type, Regex regex, String? prefix ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, prefix, null, false ) );

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        /// <param name="converter">The function to convert the raw value into a desired type</param>
        public virtual void AddRegex ( String id, TokenTypeT type, String regex, String? prefix, Func<Match, DiagnosticList, Object>? converter ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, prefix, converter, false ) );

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        /// <param name="converter">The function to convert the raw value into a desired type</param>
        public virtual void AddRegex ( String id, TokenTypeT type, Regex regex, String? prefix, Func<Match, DiagnosticList, Object>? converter ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, prefix, converter, false ) );

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
        /// inside <see cref="Token{TokenTypeT}.Trivia" /> instead)
        /// </param>
        public virtual void AddRegex ( String id, TokenTypeT type, String regex, String? prefix, Func<Match, DiagnosticList, Object>? converter, Boolean isTrivia ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, prefix, converter, isTrivia ) );

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
        /// inside <see cref="Token{TokenTypeT}.Trivia" /> instead)
        /// </param>
        public virtual void AddRegex ( String id, TokenTypeT type, Regex regex, String? prefix, Func<Match, DiagnosticList, Object>? converter, Boolean isTrivia ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, prefix, converter, isTrivia ) );

        #endregion AddRegex

        #region AddGrammar

        /// <summary>
        /// Defines a token as a grammar tree.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="spanTokenFactory"></param>
        public virtual void AddGrammar ( GrammarNode<Char> node, GrammarTreeLexerModule<TokenTypeT>.SpanTokenFactory spanTokenFactory ) =>
            this.AddModule ( new GrammarTreeLexerModule<TokenTypeT> ( node, spanTokenFactory ) );

        /// <summary>
        /// Defines a token as a grammar tree.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="stringTokenFactory"></param>
        public virtual void AddGrammar ( GrammarNode<Char> node, GrammarTreeLexerModule<TokenTypeT>.StringTokenFactory stringTokenFactory ) =>
            this.AddModule ( new GrammarTreeLexerModule<TokenTypeT> ( node, stringTokenFactory ) );

        /// <summary>
        /// Defines a token as a regex grammar tree.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="spanTokenFactory">The function responsible for converting a match into a token.</param>
        public virtual void AddGrammar ( String regex, GrammarTreeLexerModule<TokenTypeT>.SpanTokenFactory spanTokenFactory ) =>
            this.AddGrammar ( RegexParser.Parse ( regex ), spanTokenFactory );

        /// <summary>
        /// Defines a token as a regex grammar tree.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="stringTokenFactory">The function responsible for converting a match into a token.</param>
        public virtual void AddGrammar ( String regex, GrammarTreeLexerModule<TokenTypeT>.StringTokenFactory stringTokenFactory ) =>
            this.AddGrammar ( RegexParser.Parse ( regex ), stringTokenFactory );

        #endregion AddGrammar

        /// <inheritdoc/>
        public virtual ILexer<TokenTypeT> GetLexer ( String input, DiagnosticList diagnostics ) =>
            this.GetLexer ( new StringCodeReader ( input ), diagnostics );

        /// <inheritdoc/>
        public virtual ILexer<TokenTypeT> GetLexer ( ICodeReader reader, DiagnosticList diagnostics ) =>
            new ModularLexer<TokenTypeT> ( this.Modules, this._eofTokenType, reader, diagnostics );
    }
}
