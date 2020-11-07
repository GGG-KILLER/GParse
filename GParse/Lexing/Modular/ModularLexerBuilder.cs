using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using GParse.IO;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// Defines a <see cref="ILexer{TokenTypeT}" /> builder
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class ModularLexerBuilder<TokenTypeT> : ILexerFactory<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The module tree
        /// </summary>
        protected readonly LexerModuleTree<TokenTypeT> _modules = new LexerModuleTree<TokenTypeT> ( );
        private readonly TokenTypeT _eofTokenType;

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
        /// <param name="module"></param>
        public virtual void AddModule ( ILexerModule<TokenTypeT> module ) => this._modules.AddChild ( module );

        /// <summary>
        /// Removes an module from the lexer (affects existing instances)
        /// </summary>
        /// <param name="module"></param>
        public virtual void RemoveModule ( ILexerModule<TokenTypeT> module ) => this._modules.RemoveChild ( module );

        #region AddLiteral

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        public virtual void AddLiteral ( String id, TokenTypeT type, String raw ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( id, type, raw, raw, false ) );

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
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( id, type, raw, raw, isTrivia ) );

        /// <summary>
        /// Defines a token as a literal string
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="raw">The raw value of the token</param>
        /// <param name="value">The value of this token</param>
        public virtual void AddLiteral ( String id, TokenTypeT type, String raw, Object? value ) =>
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( id, type, raw, value, false ) );

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
            this.AddModule ( new LiteralLexerModule<TokenTypeT> ( id, type, raw, value, isTrivia ) );

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
        public virtual void AddRegex ( String id, TokenTypeT type, String regex, String? prefix, Func<Match, Object>? converter ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, prefix, converter, false ) );

        /// <summary>
        /// Defines a token as a regex pattern
        /// </summary>
        /// <param name="id">The ID of the token</param>
        /// <param name="type">The type of the token</param>
        /// <param name="regex">The pattern that will match the raw token value</param>
        /// <param name="prefix">The constant prefix of the regex expression (if any)</param>
        /// <param name="converter">The function to convert the raw value into a desired type</param>
        public virtual void AddRegex ( String id, TokenTypeT type, Regex regex, String? prefix, Func<Match, Object>? converter ) =>
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
        public virtual void AddRegex ( String id, TokenTypeT type, String regex, String? prefix, Func<Match, Object>? converter, Boolean isTrivia ) =>
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
        public virtual void AddRegex ( String id, TokenTypeT type, Regex regex, String? prefix, Func<Match, Object>? converter, Boolean isTrivia ) =>
            this.AddModule ( new RegexLexerModule<TokenTypeT> ( id, type, regex, prefix, converter, isTrivia ) );

        #endregion AddRegex

        /// <summary>
        /// Compiles the current lexer tree into an actual lexer class.
        /// </summary>
        /// <returns></returns>
        public ILexerFactory<TokenTypeT> Compile ( )
        {
#if HAS_RUNTIMEFEATURE_DYNAMICCODE
            if ( !RuntimeFeature.IsDynamicCodeSupported )
            {
                return this;
            }
            else
#endif
            {
                throw new NotImplementedException ( );
            }
        }

        /// <inheritdoc/>
        public virtual ILexer<TokenTypeT> GetLexer ( String input, DiagnosticList diagnostics ) =>
            this.GetLexer ( new StringCodeReader ( input ), diagnostics );

        /// <inheritdoc/>
        public virtual ILexer<TokenTypeT> GetLexer ( ICodeReader reader, DiagnosticList diagnostics ) =>
            new ModularLexer<TokenTypeT> ( this._modules, this._eofTokenType, reader, diagnostics );
    }
}
