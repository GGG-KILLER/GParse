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
    /// <typeparam name="TTokenType"></typeparam>
    public sealed class GrammarTreeLexerModule<TTokenType> : ILexerModule<TTokenType>
        where TTokenType : notnull
    {
        /// <summary>
        /// The delegate that converts a <see cref="SpanMatch"/> into a <see cref="Token{TTokenType}"/>.
        /// </summary>
        /// <param name="spanMatch">The match result.</param>
        /// <param name="diagnostics">The diagnostic list.</param>
        /// <param name="token">The parsed token.</param>
        /// <returns>Whether the token parsing was successful.</returns>
        public delegate Boolean SpanTokenFactory(SpanMatch spanMatch, DiagnosticList diagnostics, [NotNullWhen(true)] out Token<TTokenType>? token);

        /// <summary>
        /// The delegate that converts a <see cref="StringMatch"/> into a <see cref="Token{TTokenType}"/>.
        /// </summary>
        /// <param name="stringMatch"></param>
        /// <param name="diagnostics"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public delegate Boolean StringTokenFactory(StringMatch stringMatch, DiagnosticList diagnostics, [NotNullWhen(true)] out Token<TTokenType>? token);

        private readonly GrammarNode<Char> _grammarNode;
        private readonly SpanTokenFactory? _spanTokenFactory;
        private readonly StringTokenFactory? _stringTokenFactory;

        /// <inheritdoc/>
        public String Name => $"Grammar Node Lexer Module";

        /// <inheritdoc/>
        public String? Prefix { get; }

        private GrammarTreeLexerModule(GrammarNode<Char> grammarNode)
        {
            if (grammarNode is null)
                throw new ArgumentNullException(nameof(grammarNode));
            grammarNode = GrammarTreeOptimizer.Optimize(grammarNode) ?? grammarNode;
            this.Prefix = GrammarTreePrefixObtainer.Calculate(grammarNode);
            this._grammarNode = grammarNode;
        }

        /// <summary>
        /// Initializes a new grammar node lexer module.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="spanTokenFactory"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="grammarNode"/> is <see langword="null"/> or <paramref name="spanTokenFactory"/> is <see langword="null"/>.
        /// </exception>
        public GrammarTreeLexerModule(GrammarNode<Char> grammarNode, SpanTokenFactory spanTokenFactory)
            : this(grammarNode)
        {
            this._spanTokenFactory = spanTokenFactory ?? throw new ArgumentNullException(nameof(spanTokenFactory));
        }

        /// <summary>
        /// Initializes a new grammar node lexer module.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="stringTokenFactory"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="grammarNode"/> is <see langword="null"/> or <paramref name="stringTokenFactory"/> is <see langword="null"/>.
        /// </exception>
        public GrammarTreeLexerModule(GrammarNode<Char> grammarNode, StringTokenFactory stringTokenFactory)
            : this(grammarNode)
        {
            this._stringTokenFactory = stringTokenFactory ?? throw new ArgumentNullException(nameof(stringTokenFactory));
        }

        /// <inheritdoc/>
        public Boolean TryConsume(
            ICodeReader reader,
            DiagnosticList diagnostics,
            [NotNullWhen(true)] out Token<TTokenType>? token)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            if (this._spanTokenFactory is not null)
            {
                SpanMatch match = GrammarTreeInterpreter.MatchSpan(reader, this._grammarNode);
                if (!match.IsMatch || !this._spanTokenFactory(match, diagnostics, out token))
                {
                    token = null;
                    return false;
                }
            }
            else
            {
                StringMatch match = GrammarTreeInterpreter.MatchString(reader, this._grammarNode);
                if (!match.IsMatch || !this._stringTokenFactory!(match, diagnostics, out token))
                {
                    token = null;
                    return false;
                }
            }

            return true;
        }
    }
}