using GParse.Lexing.Errors;
using GParse.Lexing.IO;
using GParse.Lexing.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GParse.Lexing
{
    public abstract class LexerBase
    {
        /// <summary>
        /// Settings to lex chars
        /// </summary>
        protected CharLexSettings charSettings;

        /// <summary>
        /// Whether \n should be consumed automatically giving it
        /// no special meaning
        /// </summary>
        protected Boolean consumeNewlinesAutomatically;

        /// <summary>
        /// Whether whitespace characters should be stored in Tokens
        /// </summary>
        protected Boolean storeWhitespaces;

        /// <summary>
        /// The ID of the whitespace tokens (only used if
        /// <see cref="storeWhitespaces" /> is enabled).
        /// </summary>
        protected String whitespaceID;

        /// <summary>
        /// Last token returned when lexing
        /// </summary>
        protected Token lastToken;

        /// <summary>
        /// Settings to lex numbers
        /// </summary>
        protected IntegerLexSettings numberSettings;

        /// <summary>
        /// The reader
        /// </summary>
        protected readonly SourceCodeReader reader;

        /// <summary>
        /// Settings to lex strings
        /// </summary>
        protected  StringLexSettings stringSettings;

        /// <summary>
        /// The token manager for tokens that don't need parsing
        /// </summary>
        protected readonly TokenManager tokenManager;

        /// <summary>
        /// Shorthand for reader.Location
        /// </summary>
        protected SourceLocation Location => this.reader.Location;

        /// <summary>
        /// Initializes a new <see cref="LexerBase" />
        /// </summary>
        /// <param name="input">The input string</param>
        protected LexerBase ( String input )
        {
            this.reader = new SourceCodeReader ( input );
            this.tokenManager = new TokenManager ( );
        }

        /// <summary>
        /// Consumes a token
        /// </summary>
        /// <returns></returns>
        public Char Consume ( )
        {
            return ( Char ) this.reader.ReadChar ( );
        }

        /// <summary>
        /// Lex the string
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Token> Lex ( )
        {
            this.reader.Reset ( );
            while ( !this.reader.EOF ( ) )
            {
                Token tok = null;

                // Attempt to read tokens that don't require any parsing
                if ( this.tokenManager.TryReadToken ( this.reader, out tok ) )
                {
                    tok = UpgradeToken ( tok );
                }
                // Then attempt to go through the default
                // tokenizing route defined by the user
                else if ( this.TryReadToken ( out tok ) )
                {
                    // We should assume here that the inherited
                    // class is already returning it's own custom
                    // tokens therefore there's no need for them
                    // to be upgraded therefore we don't do anything.
                }
                // Consume newlines if allowed
                else if ( this.consumeNewlinesAutomatically && this.Consume ( "\n" ) )
                {
                    SourceLocation start = this.Location;

                    if ( this.storeWhitespaces )
                    {
                        tok = CreateToken ( this.whitespaceID, "\n", "\n", TokenType.Whitespace, start.To ( this.Location ) );
                    }
                }
                else if ( this.consumeNewlinesAutomatically && this.Consume ( "\r\n" ) )
                {
                    SourceLocation start = this.Location;

                    if ( this.storeWhitespaces )
                    {
                        tok = CreateToken ( this.whitespaceID, "\r\n", "\r\n", TokenType.Whitespace, start.To ( this.Location ) );
                    }
                }
                else if ( this.CharIsWhitepace ( ( Char ) this.reader.Peek ( ) ) )
                {
                    SourceLocation start = this.Location;
                    var ws = this.reader.ReadStringWhile ( this.CharIsWhitepace );

                    if ( this.storeWhitespaces )
                    {
                        tok = CreateToken ( this.whitespaceID, ws, ws, TokenType.Whitespace, start.To ( this.Location ) );
                    }
                }
                // Throw exception on unexpected char
                else
                {
                    var peek = this.reader.Peek ( );
                    throw new LexUnexpectedCharException ( $"Unexpected char {( Char ) peek}({peek})", ( Char ) peek );
                }

                // People can override CreateToken to do some
                // useful things such as storing whitespace in
                // other tokens
                if ( tok != null )
                {
                    this.lastToken = tok;
                    yield return tok;
                }
            }

            yield return this.CreateToken ( "Eof", "", "", TokenType.EOF, this.Location.To ( this.Location ) );
        }

        #region Char Filters

        /// <summary>
        /// Whether the char is a binary digit
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual Boolean CharIsBin ( Char ch ) => ch == '0' || ch == '1';

        /// <summary>
        /// Whether the char is a decimal digit
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual Boolean CharIsDec ( Char ch ) => '0' <= ch && ch <= '9';

        /// <summary>
        /// Whether the char is a hexadecimal digit
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual Boolean CharIsHex ( Char ch ) => ( '0' <= ch && ch <= '9' ) || ( 'A' <= ch && ch <= 'F' );

        /// <summary>
        /// Whether the char is a octal digit
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual Boolean CharIsOct ( Char ch ) => '0' <= ch && ch <= '8';

        /// <summary>
        /// Whether <paramref name="ch" /> is a whitespace
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual Boolean CharIsWhitepace ( Char ch ) => Char.IsWhiteSpace ( ch );

        #endregion Char Filters

        #region Char Reading

        /// <summary>
        /// The char escape definition
        /// </summary>
        private struct CharEscape
        {
            public Int32 Base;
            public Func<Char,Boolean> Filter;
            public Int32 MaxLen;
            public String Prefix;

            public CharEscape ( Int32 @base, Func<Char, Boolean> filter, Int32 maxLen, String prefix )
            {
                this.Base = @base;
                this.Filter = filter ?? throw new ArgumentNullException ( nameof ( filter ) );
                this.MaxLen = maxLen;
                this.Prefix = prefix;
            }
        }

        /// <summary>
        /// Attempts to read a character
        /// </summary>
        /// <param name="Delimiter"></param>
        /// <param name="conf"></param>
        /// <returns></returns>
        /// <exception cref="LexException"></exception>
        protected (String Raw, Char Value) ReadChar ( String Delimiter, CharLexSettings conf = default )
        {
            if ( conf == default )
                conf = this.charSettings;

            // Set these so we don't have to when returning false
            // Attempt to read constant escapes (longests first)
            foreach ( KeyValuePair<String, Char> kv in conf.ConstantEscapes.OrderByDescending ( kv => kv.Key.Length ) )
            {
                if ( this.Consume ( kv.Key ) )
                {
                    // Ensure delimiter at the end
                    this.Expect ( Delimiter );

                    return (Raw: kv.Key, kv.Value);
                }
            }

            // These are all the bases C# supports
            var escapes = new CharEscape[]
            {
                new CharEscape ( 2, this.CharIsBin, conf.BinaryEscapeMaxLengh, conf.BinaryEscapePrefix ),
                new CharEscape ( 8, this.CharIsOct, conf.OctalEscapeMaxLengh, conf.OctalEscapePrefix ),
                new CharEscape ( 10, this.CharIsDec, conf.DecimalEscapeMaxLengh, conf.DecimalEscapePrefix ),
                new CharEscape ( 16, this.CharIsHex, conf.HexadecimalEscapeMaxLengh, conf.HexadecimalEscapePrefix )
            };

            // Read the prefixes by longest first
            foreach ( CharEscape escape in escapes.Where ( escape => !String.IsNullOrEmpty ( escape.Prefix ) )
                .OrderByDescending ( escape => escape.Prefix.Length ) )
            {
                if ( this.Consume ( escape.Prefix ) )
                {
                    var len = 0;
                    var strch = this.reader.ReadStringWhile ( ch => escape.Filter ( ch ) && len++ < escape.MaxLen );

                    if ( strch != "" )
                    {
                        this.Expect ( Delimiter );
                        return (
                            Raw: $"{escape.Prefix}{strch}",
                            Value: ( Char ) Convert.ToInt32 ( strch, escape.Base )
                        );
                    }
                    break;
                }
            }

            return (
                Raw: this.reader.PeekString ( 1 ),
                Value: ( Char ) this.reader.ReadChar ( )
            );
        }

        /// <summary>
        /// Attempts to read a character
        /// </summary>
        /// <param name="Delimiter"></param>
        /// <param name="Value"></param>
        /// <param name="Raw"></param>
        /// <param name="conf"></param>
        /// <returns></returns>
        protected Boolean TryReadChar ( String Delimiter, out Char Value, out String Raw, CharLexSettings conf = default )
        {
            // Save starting position
            this.reader.Save ( );

            // Set these just in case
            Value = default;
            Raw = default;

            try
            {
                (Raw, Value) = this.ReadChar ( Delimiter, conf );
                this.reader.DiscardSave ( );
                return true;
            }
            catch ( Exception e )
            {
                Debug.WriteLine ( e );
                this.reader.Load ( );
                return false;
            }
        }

        #endregion Char Reading

        #region String Reading

        /// <summary>
        /// Attempts to read a string
        /// </summary>
        /// <param name="Delimiter"></param>
        /// <param name="conf"></param>
        /// <param name="isMultiline"></param>
        /// <returns></returns>
        /// <exception cref="LexException"></exception>
        protected (String Raw, String Value) ReadString ( String Delimiter, Boolean isMultiline = false, StringLexSettings conf = default )
        {
            if ( conf == default )
                conf = this.stringSettings;

            var stringVal = new StringBuilder ( );
            var stringRaw = new StringBuilder ( );
            var crlfescape = conf.NewlineEscape + "\r\n";
            var lfescape = conf.NewlineEscape + "\n";

            while ( !this.reader.IsNext ( Delimiter ) )
            {
                if ( this.reader.IsNext ( Delimiter ) )
                    break;

                if ( conf.NewlineEscape != null )
                {
                    if ( this.Consume ( crlfescape ) )
                    {
                        stringRaw.Append ( crlfescape );
                        stringVal.Append ( "\r\n" );
                        continue;
                    }
                    else if ( this.Consume ( lfescape ) )
                    {
                        stringRaw.Append ( lfescape );
                        stringVal.Append ( "\n" );
                        continue;
                    }
                }

                if ( ( this.Consume ( "\n" ) || this.Consume ( "\r\n" ) ) && !isMultiline )
                {
                    throw new LexException ( "Unfinished non-multiline string.", this.Location );
                }

                if ( this.TryReadChar ( "", out var val, out var raw ) )
                {
                    stringRaw.Append ( raw );
                    stringVal.Append ( val );
                    continue;
                }

                throw new LexException ( "Failed to read string.", this.Location );
            }

            this.Expect ( Delimiter );

            return (
                Raw: stringRaw.ToString ( ),
                Value: stringVal.ToString ( )
            );
        }

        /// <summary>
        /// Attempts to read a string
        /// </summary>
        /// <param name="Delimiter"></param>
        /// <param name="Value"></param>
        /// <param name="Raw"></param>
        /// <param name="conf"></param>
        /// <param name="isMultiline"></param>
        /// <returns></returns>
        protected Boolean TryReadString ( String Delimiter, out String Value, out String Raw, Boolean isMultiline = false, StringLexSettings conf = default )
        {
            // Set these just in case
            Value = null;
            Raw = null;

            // Save the starting locagion
            this.reader.Save ( );

            try
            {
                (Raw, Value) = this.ReadString ( Delimiter, isMultiline, conf );
                this.reader.DiscardSave ( );
                return true;
            }
            catch ( Exception )
            {
                this.reader.Load ( );
                return false;
            }
        }

        #endregion String Reading

        #region Integer Reading

        private struct IntegerLiteralDef
        {
            public Int32 Base;
            public Boolean Default;
            public Func<Char, Boolean> Filter;
            public String Prefix;

            public IntegerLiteralDef ( Int32 @base, Boolean @default, Func<Char, Boolean> filter, String prefix )
            {
                this.Base = @base;
                this.Default = @default;
                this.Filter = filter;
                this.Prefix = prefix;
            }
        }

        /// <summary>
        /// This provides basic multi-base integer reading, not
        /// supporting exponents nor anything that is not a prefix
        /// followed by a sequence of possible digits.
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        protected (String Raw, Int64 Value) ReadInteger ( IntegerLexSettings conf = default )
        {
            String nstr;
            if ( conf == default )
                conf = this.numberSettings;

            var literalDefs = new IntegerLiteralDef[]
            {
                new IntegerLiteralDef ( 2, conf.DefaultType == IntegerLexSettings.NumberType.Binary, this.CharIsBin, conf.BinaryPrefix ),
                new IntegerLiteralDef ( 8, conf.DefaultType == IntegerLexSettings.NumberType.Octal, this.CharIsOct, conf.OctalPrefix ),
                new IntegerLiteralDef ( 10, conf.DefaultType == IntegerLexSettings.NumberType.Decimal, this.CharIsDec, conf.DecimalPrefix ),
                new IntegerLiteralDef ( 16, conf.DefaultType == IntegerLexSettings.NumberType.Hexadecimal,this.CharIsHex, conf.HexadecimalPrefix )
            };

            foreach ( IntegerLiteralDef def in literalDefs
                .Where ( def => !String.IsNullOrEmpty ( def.Prefix ) )
                .OrderByDescending ( def => def.Prefix.Length ) )
            {
                if ( this.Consume ( def.Prefix ) )
                {
                    nstr = this.reader.ReadStringWhile ( def.Filter );
                    if ( nstr.Length == 0 )
                        throw new LexException ( "Invalid integer literal.", this.Location );

                    return (
                        Raw: def.Prefix + nstr,
                        Value: Convert.ToInt64 ( nstr, def.Base )
                    );
                }
            }

            if ( literalDefs.Any ( def => def.Default ) )
            {
                IntegerLiteralDef defaultDef = literalDefs.First ( def => def.Default );
                nstr = this.reader.ReadStringWhile ( defaultDef.Filter );
                if ( nstr.Length == 0 )
                    throw new LexException ( "Invalid integer literal.", this.Location );

                return (
                    Raw: nstr,
                    Value: Convert.ToInt64 ( nstr, defaultDef.Base )
                );
            }

            throw new LexException ( "No integer literal found.", this.Location );
        }

        /// <summary>
        /// Attempts to read a number
        /// </summary>
        /// <param name="Raw"></param>
        /// <param name="Value"></param>
        /// <param name="conf"></param>
        /// <returns></returns>
        protected Boolean TryReadInteger ( out String Raw, out Int64 Value, IntegerLexSettings conf = default )
        {
            this.reader.Save ( );
            Raw = null;
            Value = 0;

            try
            {
                (Raw, Value) = this.ReadInteger ( conf );
                this.reader.DiscardSave ( );
                return true;
            }
            catch ( Exception )
            {
                this.reader.Load ( );
                return false;
            }
        }

        #endregion Integer Reading

        #region Utilities

        /// <summary>
        /// Will throw an exception if the next character is not
        /// in <paramref name="chars" />
        /// </summary>
        /// <param name="chars"></param>
        protected void Expect ( params Char[] chars )
        {
            if ( !chars.Contains ( ( Char ) this.reader.Peek ( ) ) )
                throw new LexException ( $"Expected Any('{String.Join ( "', '", chars )}') but got {( Char ) this.reader.Peek ( )}.", this.Location );
            this.reader.Advance ( 1 );
        }

        /// <summary>
        /// Will throw an exception if <paramref name="seq" />
        /// isn't next(
        /// <see cref="SourceCodeReader.IsNext ( String )" /> )
        /// </summary>
        /// <param name="seq"></param>
        protected void Expect ( String seq )
        {
            if ( !this.reader.IsNext ( seq ) )
                throw new LexException ( $"Expected '{seq}' but got '{this.reader.PeekString ( seq.Length )}'.", this.Location );
            this.reader.Advance ( seq.Length );
        }

        /// <summary>
        /// Will consume the next character if it's found in <paramref name="chars" />
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        protected Boolean Consume ( params Char[] chars )
        {
            if ( chars.Contains ( ( Char ) this.reader.Peek ( ) ) )
            {
                this.reader.Advance ( 1 );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Will consume the string if it's next
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected Boolean Consume ( String str )
        {
            if ( this.reader.IsNext ( str ) )
            {
                this.reader.Advance ( str.Length );
                return true;
            }
            return false;
        }

        #endregion Utilities

        protected abstract Boolean TryReadToken ( out Token tok );

        /// <summary>
        /// This is added here for people who'd like to subclass
        /// the token class
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        protected virtual Token CreateToken ( in String ID, in String raw, in Object value, in TokenType type, in SourceRange range )
        {
            return new Token ( ID, raw, value, type, range );
        }

        /// <summary>
        /// This was designed so that inherited classes can use
        /// their own tokens by upgrading the base
        /// <see cref="Token" /> returned by our APIs
        /// </summary>
        /// <param name="token">token to be upgraded</param>
        /// <returns></returns>
        protected virtual Token UpgradeToken ( in Token token ) => token;
    }
}
