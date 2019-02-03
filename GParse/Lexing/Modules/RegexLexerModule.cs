using System;
using System.Text.RegularExpressions;
using GParse.Errors;
using GParse.IO;

namespace GParse.Lexing.Modules
{
    /// <summary>
    /// A module that defines a token through a regex pattern
    /// </summary>
    /// <typeparam name="TokenTypeT"></typeparam>
    public class RegexLexerModule<TokenTypeT> : ILexerModule<TokenTypeT>
    {
        private readonly String Id;
        private readonly TokenTypeT Type;
        private readonly String Expression;
        private readonly Regex Regex;
        private readonly Func<Match, Object> Converter;
        private readonly Boolean IsTrivia;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public String Name => $"Regex Lexer Module: {this.Expression}";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public String Prefix { get; }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        /// <param name="isTrivia"></param>
        public RegexLexerModule ( String id, TokenTypeT type, String regex, String prefix, Func<Match, Object> converter, Boolean isTrivia )
        {
            this.Converter = converter;
            this.Expression = regex;
            this.Id = id;
            this.IsTrivia = isTrivia;
            this.Prefix = prefix;
            this.Type = type;
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        /// <param name="isTrivia"></param>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex, String prefix, Func<Match, Object> converter, Boolean isTrivia )
        {
            this.Converter = converter;
            this.Regex = regex;
            this.Id = id;
            this.IsTrivia = isTrivia;
            this.Prefix = prefix;
            this.Type = type;
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        public RegexLexerModule ( String id, TokenTypeT type, String regex, String prefix, Func<Match, Object> converter ) : this ( id, type, regex, prefix, converter, false )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        /// <param name="converter"></param>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex, String prefix, Func<Match, Object> converter ) : this ( id, type, regex, prefix, converter, false )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        public RegexLexerModule ( String id, TokenTypeT type, String regex, String prefix ) : this ( id, type, regex, prefix, null )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        /// <param name="prefix"></param>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex, String prefix ) : this ( id, type, regex, prefix, null )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        public RegexLexerModule ( String id, TokenTypeT type, String regex ) : this ( id, type, regex, null )
        {
        }

        /// <summary>
        /// Initializes the <see cref="RegexLexerModule{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="regex"></param>
        public RegexLexerModule ( String id, TokenTypeT type, Regex regex ) : this ( id, type, regex, null )
        {
        }

        // Ideally modules should be stateless, but read CanConsumeNext for the explanation.
        private SourceLocation Start;
        private SourceLocation End;
        private Match StoredResult;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Boolean CanConsumeNext ( SourceCodeReader reader )
        {
            // Ideally CanConsumeNext should not leave the reader modified, but in this case since we'll
            // be called in sequence, it's good to not re-execute the matching again.
            this.Start = reader.Location;
            this.StoredResult = null;
            Match res = this.Expression != null
                ? reader.MatchRegex ( this.Expression )
                : reader.MatchRegex ( this.Regex );
            if ( res.Success )
            {
                this.End = reader.Location;
                this.StoredResult = res;

                reader.Rewind ( this.Start );
                return true;
            }

            reader.Rewind ( this.Start );
            return false;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        public Token<TokenTypeT> ConsumeNext ( SourceCodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            if ( this.StoredResult != null )
            {
                reader.Rewind ( this.End );
                return new Token<TokenTypeT> ( this.Id, this.StoredResult.Value, this.Converter != null ? this.Converter ( this.StoredResult ) : this.StoredResult.Value, this.Type, this.Start.To ( this.End ), this.IsTrivia );
            }
            else
                throw new FatalParsingException ( reader.Location, "Cannot consume a token when check wasn't successful." );
        }
    }
}
