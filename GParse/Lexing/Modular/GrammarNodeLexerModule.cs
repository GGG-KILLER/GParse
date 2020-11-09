using System;
using System.Diagnostics.CodeAnalysis;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing.Composable;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public sealed class GrammarNodeLexerModule<TokenTypeT> : ILexerModule<TokenTypeT>
        where TokenTypeT : notnull
    {
        /// <summary>
        /// The delegate that converts a <see cref="SpanMatch"/> into a <see cref="Token{TokenTypeT}"/>.
        /// </summary>
        /// <param name="spanMatch">The match result.</param>
        /// <param name="diagnostics">The diagnostic list.</param>
        /// <param name="token">The parsed token.</param>
        /// <returns>Whether the token parsing was successful.</returns>
        public delegate Boolean SpanTokenFactory ( SpanMatch spanMatch, DiagnosticList diagnostics, [NotNullWhen ( true )] out Token<TokenTypeT>? token );

        /// <summary>
        /// The delegate that converts a <see cref="StringMatch"/> into a <see cref="Token{TokenTypeT}"/>.
        /// </summary>
        /// <param name="stringMatch"></param>
        /// <param name="diagnostics"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public delegate Boolean StringTokenFactory ( StringMatch stringMatch, DiagnosticList diagnostics, [NotNullWhen ( true )] out Token<TokenTypeT>? token );

        private readonly GrammarNode<Char> _grammarNode;
        private readonly SpanTokenFactory? _spanTokenFactory;
        private readonly StringTokenFactory? _stringTokenFactory;

        /// <inheritdoc/>
        public String Name => $"Grammar Node Lexer Module";

        /// <inheritdoc/>
        public String? Prefix { get; }

        private GrammarNodeLexerModule ( GrammarNode<Char> grammarNode )
        {
            grammarNode = GrammarTreeOptimizer.Optimize ( grammarNode ) ?? grammarNode;
            this.Prefix = GrammarTreePrefixObtainer.Calculate ( grammarNode );
            this._grammarNode = grammarNode;
        }

        /// <summary>
        /// Initializes a new grammar node lexer module.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="spanTokenFactory"></param>
        public GrammarNodeLexerModule ( GrammarNode<Char> grammarNode, SpanTokenFactory spanTokenFactory )
            : this ( grammarNode )
        {
            this._spanTokenFactory = spanTokenFactory;
        }

        /// <summary>
        /// Initializes a new grammar node lexer module.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="stringTokenFactory"></param>
        public GrammarNodeLexerModule ( GrammarNode<Char> grammarNode, StringTokenFactory stringTokenFactory )
            : this ( grammarNode )
        {
            this._stringTokenFactory = stringTokenFactory;
        }

        /// <inheritdoc/>
        public Boolean TryConsume (
            ICodeReader reader,
            DiagnosticList diagnostics,
            [NotNullWhen ( true )] out Token<TokenTypeT>? token )
        {
            if ( this._spanTokenFactory is not null )
            {
                SpanMatch match = GrammarTreeInterpreter.Span ( reader, this._grammarNode );
                if ( !match.IsMatch || !this._spanTokenFactory ( match, diagnostics, out token ) )
                {
                    token = null;
                    return false;
                }
            }
            else
            {
                StringMatch match = GrammarTreeInterpreter.String ( reader, this._grammarNode );
                if ( !match.IsMatch || !this._stringTokenFactory! ( match, diagnostics, out token ) )
                {
                    token = null;
                    return false;
                }
            }

            return true;
        }
    }
}
