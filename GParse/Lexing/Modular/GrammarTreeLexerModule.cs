using System;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing.Composable;
using Tsu;

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
        /// <param name="startLocation">The location the reader has started parsing at.</param>
        /// <param name="spanMatch">The match result.</param>
        /// <param name="diagnostics">The diagnostic list.</param>
        /// <returns>The token if parsing was successful.</returns>
        public delegate Option<Token<TTokenType>> SpanTokenFactory(
            int startLocation,
            SpanMatch spanMatch,
            DiagnosticList diagnostics);

        /// <summary>
        /// The delegate that converts a <see cref="StringMatch"/> into a <see cref="Token{TTokenType}"/>.
        /// </summary>
        /// <param name="startLocation">The location the reader has started parsing at.</param>
        /// <param name="stringMatch">The match result.</param>
        /// <param name="diagnostics">The diagnostic list.</param>
        /// <returns>The token if parsing was successful.</returns>
        /// <returns></returns>
        public delegate Option<Token<TTokenType>> StringTokenFactory(
            int startLocation,
            StringMatch stringMatch,
            DiagnosticList diagnostics);

        private readonly GrammarNode<char> _grammarNode;
        private readonly SpanTokenFactory? _spanTokenFactory;
        private readonly StringTokenFactory? _stringTokenFactory;

        /// <inheritdoc/>
        public string Name => $"Grammar Node Lexer Module";

        /// <inheritdoc/>
        public string? Prefix { get; }

        private GrammarTreeLexerModule(GrammarNode<char> grammarNode)
        {
            if (grammarNode is null)
                throw new ArgumentNullException(nameof(grammarNode));
            grammarNode = GrammarTreeOptimizer.Optimize(grammarNode) ?? grammarNode;
            Prefix = GrammarTreePrefixObtainer.Calculate(grammarNode);
            _grammarNode = grammarNode;
        }

        /// <summary>
        /// Initializes a new grammar node lexer module.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="spanTokenFactory"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="grammarNode"/> is <see langword="null"/> or <paramref name="spanTokenFactory"/> is <see langword="null"/>.
        /// </exception>
        public GrammarTreeLexerModule(
            GrammarNode<char> grammarNode,
            SpanTokenFactory spanTokenFactory)
            : this(grammarNode)
        {
            _spanTokenFactory = spanTokenFactory
                ?? throw new ArgumentNullException(nameof(spanTokenFactory));
        }

        /// <summary>
        /// Initializes a new grammar node lexer module.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="stringTokenFactory"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="grammarNode"/> is <see langword="null"/> or <paramref name="stringTokenFactory"/> is <see langword="null"/>.
        /// </exception>
        public GrammarTreeLexerModule(
            GrammarNode<char> grammarNode,
            StringTokenFactory stringTokenFactory)
            : this(grammarNode)
        {
            _stringTokenFactory = stringTokenFactory
                ?? throw new ArgumentNullException(nameof(stringTokenFactory));
        }

        /// <inheritdoc/>
        public Option<Token<TTokenType>> TryConsume(ICodeReader reader, DiagnosticList diagnostics)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            var start = reader.Position;
            if (_spanTokenFactory is not null)
            {
                var match = GrammarTreeInterpreter.MatchSpan(reader, _grammarNode);
                if (match.IsMatch)
                    return _spanTokenFactory(start, match, diagnostics);
            }
            else
            {
                var match = GrammarTreeInterpreter.MatchString(reader, _grammarNode);
                if (match.IsMatch)
                    return _stringTokenFactory!(start, match, diagnostics);
            }

            return Option.None<Token<TTokenType>>();
        }
    }
}