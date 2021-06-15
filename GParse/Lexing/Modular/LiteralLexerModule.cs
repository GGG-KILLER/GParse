using System;
using System.Diagnostics.CodeAnalysis;
using GParse.IO;
using GParse.Math;
using Tsu;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// A module that defines a token with a fixed format
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public sealed class LiteralLexerModule<TTokenType> : ILexerModule<TTokenType>
        where TTokenType : notnull
    {
        private readonly string _id;
        private readonly TTokenType _type;
        private readonly object? _value;
        private readonly bool _isTrivia;

        /// <inheritdoc />
        public string Name => $"Literal Module: '{Prefix}'";

        /// <inheritdoc />
        public string Prefix { get; }

        /// <summary>
        /// Initializes the <see cref="LiteralLexerModule{TTokenType}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="isTrivia"></param>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="id"/> or <paramref name="type"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="text"/> is null or empty.</exception>
        public LiteralLexerModule(string id, TTokenType type, bool isTrivia, object? value, string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty", nameof(text));

            _id = id ?? throw new ArgumentNullException(nameof(id));
            _type = type ?? throw new ArgumentNullException(nameof(type));
            Prefix = text;
            _value = value;
            _isTrivia = isTrivia;
        }

        /// <inheritdoc/>
        public Option<Token<TTokenType>> TryConsume(ICodeReader reader, DiagnosticList diagnostics)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var start = reader.Position;
            reader.Advance(Prefix.Length);
            return new Token<TTokenType>(
                _id,
                _type,
                new Range<int>(start, reader.Position),
                _isTrivia,
                _value,
                Prefix);
        }
    }
}