using System;
using GParse.IO;

namespace GParse.Lexing.Modules
{
    /// <summary>
    /// A module that defines a token with a fixed format
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class LiteralLexerModule<TokenTypeT> : ILexerModule<TokenTypeT>
    {
        private readonly String _id;
        private readonly TokenTypeT _type;
        private readonly Object _value;
        private readonly Boolean _isTrivia;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public String Name => $"Literal Module: '{this.Prefix}'";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public String Prefix { get; }

        /// <summary>
        /// Initializes the <see cref="LiteralLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        public LiteralLexerModule ( String id, TokenTypeT type, String raw ) : this ( id, type, raw, raw, false )
        {
        }

        /// <summary>
        /// Initializes the <see cref="LiteralLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        public LiteralLexerModule ( String id, TokenTypeT type, String raw, Object value ) : this ( id, type, raw, value, false )
        {
        }

        /// <summary>
        /// Initializes the <see cref="LiteralLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        /// <param name="isTrivia"></param>
        public LiteralLexerModule ( String id, TokenTypeT type, String raw, Boolean isTrivia ) : this ( id, type, raw, raw, isTrivia )
        {
        }

        /// <summary>
        /// Initializes the <see cref="LiteralLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <param name="isTrivia"></param>
        public LiteralLexerModule ( String id, TokenTypeT type, String raw, Object value, Boolean isTrivia )
        {
            this._id = id;
            this._type = type;
            this.Prefix = raw;
            this._value = value;
            this._isTrivia = isTrivia;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Boolean CanConsumeNext ( IReadOnlyCodeReader reader ) =>
            reader.IsNext ( this.Prefix );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        public Token<TokenTypeT> ConsumeNext ( ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            SourceLocation start = reader.Location;
            reader.Advance ( this.Prefix.Length );
            return new Token<TokenTypeT> ( this._id, this.Prefix, this._value, this._type, start.To ( reader.Location ), this._isTrivia );
        }
    }
}
