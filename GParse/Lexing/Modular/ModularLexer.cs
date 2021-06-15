using System;
using System.Runtime.CompilerServices;
using GParse.Errors;
using GParse.IO;
using GParse.Math;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// A modular lexer created by the <see cref="ModularLexerBuilder{TTokenType}" />
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public class ModularLexer<TTokenType> : BaseLexer<TTokenType>
        where TTokenType : notnull
    {
        /// <summary>
        /// This lexer's module tree
        /// </summary>
        protected LexerModuleTree<TTokenType> ModuleTree { get; }

        /// <summary>
        /// The token type to use when emitting an EOF token.
        /// </summary>
        protected TTokenType EndOfFileTokenType { get; }

        /// <summary>
        /// Initializes a new lexer.
        /// You can call this directly but you shouldn't.
        /// Call <see cref="ModularLexerBuilder{TTokenType}.GetLexer(ICodeReader, DiagnosticList)"/> instead.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="endOfFileTokenType"></param>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        public ModularLexer(
            LexerModuleTree<TTokenType> tree,
            TTokenType endOfFileTokenType,
            ICodeReader reader,
            DiagnosticList diagnostics)
            : base(reader, diagnostics)
        {
            ModuleTree = tree ?? throw new ArgumentNullException(nameof(tree));
            EndOfFileTokenType = endOfFileTokenType;
        }

        /// <summary>
        /// Consumes a token accounting for trivia tokens.
        /// </summary>
        /// <returns></returns>
        protected override Token<TTokenType> GetNextToken()
        {
            var reader = Reader;
            var diagnostics = Diagnostics;
            var start = reader.Position;
            try
            {
                if (EndOfFile)
                    return new Token<TTokenType>("EOF", EndOfFileTokenType, new Range<int>(reader.Position));

                foreach (var module in ModuleTree.GetSortedCandidates(reader))
                {
                    var tokenOpt = module.TryConsume(reader, diagnostics);
                    if (tokenOpt.IsSome)
                        return tokenOpt.Value;

                    reader.Restore(start);
                }

                var fallback = ModuleTree.FallbackModule;
                if (fallback is not null)
                {
                    var tokenOpt = fallback.TryConsume(reader, Diagnostics);
                    if (tokenOpt.IsSome)
                        return tokenOpt.Value;
                }

                throw new FatalParsingException(reader.Position, "No registered modules can consume the rest of the input.");
            }
            catch when (restore(reader, start))
            {
                throw;
            }

            // We use this method so that when an exception is thrown, we don't
            // rewind the stack but still restore the reader's position.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool restore(ICodeReader reader, int position)
            {
                reader.Restore(position);
                return false;
            }
        }
    }
}