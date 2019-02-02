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
        private readonly String ID;
        private readonly TokenTypeT Type;
        private readonly Object Value;
        private readonly Boolean IsTrivia;

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
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        public LiteralLexerModule ( String ID, TokenTypeT type, String raw ) : this ( ID, type, raw, raw, false )
        {
        }

        /// <summary>
        /// Initializes the <see cref="LiteralLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        public LiteralLexerModule ( String ID, TokenTypeT type, String raw, Object value ) : this ( ID, type, raw, value, false )
        {
        }

        /// <summary>
        /// Initializes the <see cref="LiteralLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        /// <param name="isTrivia"></param>
        public LiteralLexerModule ( String ID, TokenTypeT type, String raw, Boolean isTrivia ) : this ( ID, type, raw, raw, isTrivia )
        {
        }

        /// <summary>
        /// Initializes the <see cref="LiteralLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <param name="isTrivia"></param>
        public LiteralLexerModule ( String ID, TokenTypeT type, String raw, Object value, Boolean isTrivia )
        {
            this.ID = ID;
            this.Type = type;
            this.Prefix = raw;
            this.Value = value;
            this.IsTrivia = isTrivia;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Boolean CanConsumeNext ( SourceCodeReader reader ) =>
            reader.IsNext ( this.Prefix );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        public Token<TokenTypeT> ConsumeNext ( SourceCodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            SourceLocation start = reader.Location;
            reader.Advance ( this.Prefix.Length );
            return new Token<TokenTypeT> ( this.ID, this.Prefix, this.Value, this.Type, start.To ( reader.Location ), this.IsTrivia );
        }
    }
}
