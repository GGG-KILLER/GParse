using System;
using System.Diagnostics.CodeAnalysis;
using GParse.IO;
using GParse.Math;

namespace GParse.Lexing.Modular
{
    /// <summary>
    /// A module that defines a token with a fixed format
    /// </summary>
    /// <typeparam name="TTokenType"></typeparam>
    public sealed class LiteralLexerModule<TTokenType> : ILexerModule<TTokenType>
        where TTokenType : notnull
    {
        private readonly String _id;
        private readonly TTokenType _type;
        private readonly Object? _value;
        private readonly Boolean _isTrivia;

        /// <inheritdoc />
        public String Name => $"Literal Module: '{this.Prefix}'";

        /// <inheritdoc />
        public String Prefix { get; }

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
        public LiteralLexerModule(String id, TTokenType type, Boolean isTrivia, Object? value, String text)
        {
            if (String.IsNullOrEmpty(text))
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty", nameof(text));

            this._id = id ?? throw new ArgumentNullException(nameof(id));
            this._type = type ?? throw new ArgumentNullException(nameof(type));
            this.Prefix = text;
            this._value = value;
            this._isTrivia = isTrivia;
        }

        /// <inheritdoc/>
        public Boolean TryConsume(ICodeReader reader, DiagnosticList diagnostics, [NotNullWhen(true)] out Token<TTokenType>? token)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.IsNext(this.Prefix))
            {
                var start = reader.Position;
                reader.Advance(this.Prefix.Length);
                token = new Token<TTokenType>(
                    this._id,
                    this._type,
                    new Range<Int32>(start, reader.Position),
                    this._isTrivia,
                    this._value,
                    this.Prefix);
                return true;
            }

            token = null;
            return false;
        }
    }
}