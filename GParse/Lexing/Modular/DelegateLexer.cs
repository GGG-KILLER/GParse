using System;
using System.Runtime.CompilerServices;
using GParse.Errors;
using GParse.IO;
using GParse.Math;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// A delegate-based lexer.
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public class DelegateLexer<TTokenType> : BaseLexer<TTokenType>
        where TTokenType : notnull
    {
        /// <summary>
        /// Initializes a new delegate lexer.
        /// </summary>
        /// <param name="delegateTree"></param>
        /// <param name="endOfFileTokenType"></param>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        public DelegateLexer(
            LexerDelegateTree<TTokenType> delegateTree,
            TTokenType endOfFileTokenType,
            ICodeReader reader,
            DiagnosticList diagnostics)
            : base(reader, diagnostics)
        {
            DelegateTree = delegateTree ?? throw new ArgumentNullException(nameof(delegateTree));
            EndOfFileTokenType = endOfFileTokenType;
        }

        /// <summary>
        /// This lexer's delegate tree.
        /// </summary>
        protected LexerDelegateTree<TTokenType> DelegateTree { get; }

        /// <summary>
        /// The token type to emit an end-of-file token with.
        /// </summary>
        protected TTokenType EndOfFileTokenType { get; }

        /// <inheritdoc/>
        protected override Token<TTokenType> GetNextToken()
        {
            var reader = Reader;
            var diagnostics = Diagnostics;
            var start = reader.Position;

            try
            {
                if (EndOfFile)
                    return new Token<TTokenType>("EOF", EndOfFileTokenType, new Range<int>(start));

                foreach (var lexerDelegate in DelegateTree.GetSortedCandidates(reader))
                {
                    var result = lexerDelegate(reader, diagnostics);
                    if (result.IsSome)
                        return result.Value;
                    Reader.Restore(start);
                }

                var fallback = DelegateTree.FallbackDelegate;
                if (fallback is not null)
                {
                    var result = fallback(reader, diagnostics);
                    if (result.IsSome)
                        return result.Value;
                }

                throw new FatalParsingException(start, "No registered modules can parse the rest of the input.");
            }
            catch when (restore(reader, start))
            {
                throw;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool restore(ICodeReader reader, int start)
            {
                reader.Restore(start);
                return false;
            }
        }
    }
}
