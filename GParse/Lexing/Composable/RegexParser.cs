using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using GParse.Composable;
using GParse.IO;
using GParse.Math;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// A <see cref="GrammarNode{T}"/> regex parser.
    /// </summary>
    public class RegexParser
    {
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        private static Boolean IsDecimalChar ( Char ch ) =>
            CharUtils.IsInRange ( '0', ch, '9' );

        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        private static Boolean IsHexChar ( Char ch ) =>
            CharUtils.IsInRange ( 'a', CharUtils.AsciiLowerCase ( ch ), 'f' ) || IsDecimalChar ( ch );

        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        private static Boolean IsWordChar ( Char ch ) =>
            CharUtils.IsInRange ( 'a', CharUtils.AsciiLowerCase ( ch ), 'z' ) || IsDecimalChar ( ch ) || ch == '_';

        /// <summary>
        /// Parses a regex expression from a reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="delimitator">The regex expression delimitator (if any).</param>
        /// <returns></returns>
        public static GrammarNode<Char> Parse ( ICodeReader reader, Char? delimitator = null ) =>
            new RegexParser ( reader, delimitator ).MainParse ( );

        /// <summary>
        /// Parses a regex from a string.
        /// </summary>
        /// <param name="pattern">The regex to be parsed.</param>
        /// <returns></returns>
        public static GrammarNode<Char> Parse ( String pattern )
        {
            var reader = new StringCodeReader ( pattern );
            GrammarNode<Char> tree = Parse ( reader );
            if ( reader.Position != reader.Length )
                throw new RegexParseException ( reader.Location, "Input wasn't entirely consumed." );
            return tree;
        }

        private readonly ICodeReader _reader;
        private readonly Char? _delimitator;
        private Int32 lastCaptureGroupNumber;

        private RegexParser ( ICodeReader reader, Char? delimitator )
        {
            this._reader = reader ?? throw new ArgumentNullException ( nameof ( reader ) );
            this._delimitator = delimitator;
        }

        /// <summary>
        /// SPECIAL_CHAR
        ///     : [.$^{\[(|)*+?\\]
        ///     ;
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        private Boolean IsSpecialChar ( Char ch ) =>
            ch is '.' or '$' or '^' or '{' or '[' or '(' or '|' or ')' or '*' or '+' or '?' or '\\'
            || ch == this._delimitator;

        /// <summary>
        /// HEX_ESCAPE
        ///     : '\\x' HEX_DIGIT+
        ///     ;
        /// HEX_DIGIT
        ///     : [a-fA-F0-9]
        ///     ;
        /// </summary>
        /// <returns></returns>
        /// <exception cref="RegexParseException">Thrown when the parsed hexadecimal number is invalid.</exception>
        private Char ParseHexNumber ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
#if HAS_SPAN
            ReadOnlySpan<Char> number = reader.ReadSpanWhile ( ch => IsHexChar ( ch ) );
#else
            var number = reader.ReadStringWhile ( ch => IsHexChar ( ch ) );
#endif
            if ( number.Length < 1 || number.Length > 4 || !UInt16.TryParse ( number, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var parsedNumber ) )
                throw new RegexParseException ( start.To ( reader.Location ), $"Invalid hexadecimal escape." );

            return ( Char ) parsedNumber;
        }

        /// <summary>
        /// ESCAPE
        /// 	: HEX_ESCAPE
        /// 	| SIMPLE_ESCAPE
        /// 	;
        ///
        /// SIMPLE_ESCAPE
        /// 	: '\\a'
        /// /*  | '\\b' */
        /// 	| '\\f'
        /// 	| '\\n'
        /// 	| '\\r'
        /// 	| '\\t'
        /// 	| '\\v'
        /// 	| '\\' SPECIAL_CHAR
        /// 	;
        /// </summary>
        /// <returns></returns>
        private Char? ParseEscapedChar ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation pos = reader.Location;
            if ( reader.IsNext ( '\\' ) )
                reader.Advance ( 1 );
            else
                return null;

            switch ( reader.Read ( ) )
            {
                case 'a': return '\a';
                //case 'b': return '\b';
                case 'f': return '\f';
                case 'n': return '\n';
                case 'r': return '\r';
                case 't': return '\t';
                case 'v': return '\v';

                // Numeric escapes:
                case 'x': return this.ParseHexNumber ( );

                // Regex special chars:
                case Char ch when this.IsSpecialChar ( ch ):
                    return ch;

                default:
                    reader.Restore ( pos );
                    return null;
            }
        }

        /// <summary>
        /// character_class
        /// 	: '.'
        /// 	| '\\' (
        /// 		'd'
        /// 		| 'D'
        /// 		| 'w'
        /// 		| 'W'
        /// 		| 's'
        /// 		| 'S'
        /// 	)
        /// 	| UNICODE_CLASS_OR_BLOCK
        /// 	;
        /// 
        /// UNICODE_CLASS_OR_BLOCK:
        /// 	: '\\' ('p' | 'P') '{' UNICODE_CLASS_OR_BLOCK_NAME '}'
        /// 	;
        /// 
        /// UNICODE_CLASS_OR_BLOCK_NAME
        /// 	: (WORD_CHAR | '-')+ /* &lt;any of the names in UnicodeCharacterCategoriesAndCodeBlocks.xml&gt; */
        /// 	;
        /// </summary>
        /// <returns></returns>
        private GrammarNode<Char>? ParseCharacterClass ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;

            switch ( reader.Read ( ) )
            {
                case '\\':
                {
                    var negated = false;
                    switch ( reader.Read ( ) )
                    {
                        case 'D': negated = true; goto case 'd';
                        case 'd': return negated ? CharacterClasses.NotDigit : CharacterClasses.Digit;

                        case 'W': negated = true; goto case 'w';
                        case 'w': return negated ? CharacterClasses.NotWord : CharacterClasses.Word;

                        case 'S': negated = true; goto case 's';
                        case 's': return negated ? CharacterClasses.NotWhitespace : CharacterClasses.Whitespace;

                        case 'P': negated = true; goto case 'p';
                        case 'p':
                        {
                            if ( !reader.IsNext ( '{' ) )
                                throw new RegexParseException ( start.To ( reader.Location ), "Invalid \\p{X} escape." );
                            reader.Advance ( 1 ); // skip over the {

                            ReadOnlySpan<Char> name = reader.ReadSpanUntil ( '}' );

                            if ( !reader.IsNext ( '}' ) )
                                throw new RegexParseException ( start.To ( reader.Location ), "Unfinished \\p{X} escape." );
                            reader.Advance ( 1 ); // skip over the }

                            if ( CharacterClasses.Unicode.TryParse ( name, out GrammarNode<Char>? node ) )
                            {
                                return negated
                                    ? node.Negate ( )
                             : node;
                            }
                            throw new RegexParseException ( start.To ( reader.Location ), $"Invalid unicode class or code block name: {name.ToString ( )}." );
                        }
                    }

                    break;
                }

                case '.': return CharacterClasses.Dot;
            }

            reader.Restore ( start );
            return null;
        }

        /// <summary>
        /// CHAR
        /// 	: ESCAPE
        /// 	| ~SPECIAL_CHAR
        /// 	;
        /// </summary>
        /// <returns></returns>
        private Char? ParseChar ( )
        {
            ICodeReader reader = this._reader;
            if ( this.ParseEscapedChar ( ) is not Char ch )
            {
                if ( reader.Peek ( ) is not Char ch2
                     || this.IsSpecialChar ( ch2 ) )
                {
                    return null;
                }
                else
                {
                    reader.Advance ( 1 );
                    ch = ch2;
                }
            }

            return ch;
        }

        /// <summary>
        /// ALTERNATION_CHAR
        /// 	: ESCAPE
        /// 	| [^\]\\]
        /// 	;
        /// </summary>
        /// <returns></returns>
        private Char? ParseAlternationChar ( Boolean allowRightBracket = false )
        {
            ICodeReader reader = this._reader;
            if ( this.ParseEscapedChar ( ) is not Char ch )
            {
                if ( reader.Peek ( ) is not Char ch2
                    || ( !allowRightBracket && ch2 == ']' )
                    || ch2 == '\\' )
                {
                    return null;
                }
                else
                {
                    reader.Advance ( 1 );
                    ch = ch2;
                }
            }

            return ch;
        }


        /// <summary>
        /// alternation_set
        /// 	: '[' '^'? ']'? alternation_element+ ']'
        /// 	;
        /// 
        /// alternation_element
        /// 	: character_class
        /// 	| character_range
        /// 	| ALTERNATION_CHAR
        /// 	;
        /// 
        /// character_range
        /// 	: ALTERNATION_CHAR '-' ALTERNATION_CHAR
        /// 	;
        /// </summary>
        /// <returns></returns>
        private GrammarNode<Char> ParseSet ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
            var negated = false;
            var elements = new List<SetElement> ( );

            // Skip over the [
            reader.Advance ( 1 );
            if ( reader.IsNext ( '^' ) )
            {
                reader.Advance ( 1 );
                negated = true;
            }

            var firstChar = true;
            do
            {
                if ( !reader.IsNext ( '.' ) && this.ParseCharacterClass ( ) is GrammarNode<Char> node )
                {
                    addNode ( node );
                }
                else
                {
                    if ( this.ParseAlternationChar ( firstChar ) is not Char rangeStart )
                    {
                        goto unexpectedChar;
                    }

                    if ( reader.IsNext ( '-' ) )
                    {
                        reader.Advance ( 1 );
                        if ( reader.IsNext ( ']' ) )
                        {
                            elements.Add ( rangeStart );
                            elements.Add ( '-' );
                            break;
                        }
                        else if ( this.ParseAlternationChar ( ) is Char rangeEnd )
                        {
                            elements.Add ( new Range<Char> ( rangeStart, rangeEnd ) );
                        }
                        else
                        {
                            goto unexpectedChar;
                        }
                    }
                    else
                    {
                        elements.Add ( rangeStart );
                    }
                }

                firstChar = false;
            }
            while ( !reader.IsNext ( ']' ) );

            // Skip over the ]
            reader.Advance ( 1 );

            if ( elements.Count < 1 )
                throw new RegexParseException ( start.To ( reader.Location ), "Invalid empty set." );

            return negated
                ? new NegatedSet ( elements.ToArray ( ) )
                : new Set ( elements.ToArray ( ) );

        unexpectedChar:
            SourceLocation invalidCharStart = reader.Location;
            if ( reader.Read ( ) is Char invalidChar )
            {
                throw new RegexParseException ( invalidCharStart.To ( reader.Location ), $"Invalid set character '{invalidChar}'." );
            }

            throw new RegexParseException ( start.To ( reader.Location ), "Unfinished set." );

            void addNode ( GrammarNode<Char> node )
            {
                switch ( node )
                {
                    case CharacterTerminal terminal:
                        elements!.Add ( terminal );
                        break;
                    case CharacterRange characterRange:
                        elements!.Add ( characterRange );
                        break;
                    case UnicodeCategoryTerminal unicodeCategoryTerminal:
                        elements!.Add ( unicodeCategoryTerminal );
                        break;
                    case Set set:
                    {
                        foreach ( var ch in set.Characters )
                            elements!.Add ( ch );
                        foreach ( Range<Char> range in set.Ranges )
                            elements!.Add ( range );
                        foreach ( UnicodeCategory unicodeCategory in set.UnicodeCategories )
                            elements!.Add ( unicodeCategory );
                        break;
                    }
                    case NegatedCharacterRange negatedCharacterRange:
                        elements!.Add ( negatedCharacterRange );
                        break;
                    case NegatedCharacterTerminal negatedCharacterTerminal:
                        elements!.Add ( negatedCharacterTerminal );
                        break;
                    case NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal:
                        elements!.Add ( negatedUnicodeCategoryTerminal );
                        break;
                    case NegatedSet negatedSet:
                        elements!.Add ( negatedSet );
                        break;
                    default:
                        throw new InvalidOperationException ( $"Invalid set node: {node?.GetType ( ).Name ?? "null"}." );
                }
            }
        }

        /// <summary>
        /// lookahead
        /// 	: '(?' ('=' | '!') expression ')'
        /// 	;
        /// </summary>
        /// <param name="prefixRead">Whether the prefix '(?' was read.</param>
        /// <returns></returns>
        private GrammarNode<Char>? ParseLookahead ( Boolean prefixRead = false )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
            if ( !prefixRead )
            {
                if ( reader.IsNext ( "(?" ) )
                    reader.Advance ( 2 );
                else
                    return null;
            }

            var type = reader.Read ( );
            if ( type is null || type is not ( '=' or '!' ) )
                goto ambiguousFail;

            GrammarNode<Char> innerNode = this.MainParse ( );
            if ( !reader.IsNext ( ')' ) )
                throw new RegexParseException ( start.To ( reader.Location ), "Unfinished lookahead." );
            reader.Advance ( 1 );

            var isNegative = type == '!';
            return isNegative
                ? new NegativeLookahead ( innerNode )
                : new Lookahead ( innerNode );

        ambiguousFail: // A fail that can happen because of ambiguity in the syntax.
            reader.Restore ( start );
            return null;
        }

        /// <summary>
        /// non_capturing_group
        /// 	: '(?:' expression ')'
        /// 	;
        /// </summary>
        /// <param name="prefixRead">Whether </param>
        /// <returns></returns>
        private GrammarNode<Char>? ParseNonCapturingGroup ( Boolean prefixRead = false )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;

            if ( !prefixRead )
            {
                if ( reader.IsNext ( "(?" ) )
                    reader.Advance ( 2 );
                else
                    return null;
            }

            if ( !reader.IsNext ( ':' ) )
            {
                reader.Restore ( start );
                return null;
            }
            reader.Advance ( 1 ); // skip over the :

            GrammarNode<Char> innerNode = this.MainParse ( );
            if ( !reader.IsNext ( ')' ) )
                throw new RegexParseException ( start.To ( reader.Location ), "Unfinished non-capturing group." );
            reader.Advance ( 1 );

            return innerNode;
        }

        /// <summary>
        /// '\\' ('0'..'9')+
        /// </summary>
        /// <returns></returns>
        private NumberedBackreference? ParseNumberedBackreference ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
            if ( !reader.IsNext ( '\\' ) )
                return null;
            reader.Advance ( 1 );

#if HAS_SPAN
            ReadOnlySpan<Char> number = reader.ReadSpanWhile ( ch => IsDecimalChar ( ch ) );
#else
            var number = reader.ReadStringWhile ( ch => IsDecimalChar ( ch ) );
#endif
            if ( number.Length == 0 )
            {
                reader.Restore ( start );
                return null;
            }
            if ( number.Length > 3 || !Byte.TryParse ( number, NumberStyles.None, CultureInfo.InvariantCulture, out var parsedNumber ) )
                throw new RegexParseException ( start.To ( reader.Location ), "Invalid backreference." );

            return new NumberedBackreference ( parsedNumber );
        }

        /// <summary>
        /// '\k&lt;' WORD_CHAR+ '&gt;'
        /// </summary>
        /// <returns></returns>
        private NamedBackreference? ParseNamedBackreference ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
            if ( !reader.IsNext ( "\\k" ) )
                return null;
            reader.Advance ( 2 );

            if ( !reader.IsNext ( '<' ) )
                throw new RegexParseException ( start.To ( reader.Location ), "Expected opening '<' for named backreference." );
            reader.Advance ( 1 );

            var name = reader.ReadStringWhile ( ch => IsWordChar ( ch ) );

            if ( name.Length < 1 )
                throw new RegexParseException ( start.To ( reader.Location ), "Invalid named backreference name." );
            if ( !reader.IsNext ( '>' ) )
                throw new RegexParseException ( start.To ( reader.Location ), "Expected closing '>' in named backreference." );
            reader.Advance ( 1 );

            return new NamedBackreference ( name );
        }

        /// <summary>
        /// numbered_capture_group
        /// 	: '(' expression ')'
        /// 	;
        /// </summary>
        /// <returns></returns>
        private NumberedCapture? ParseNumberedCaptureGroup ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
            if ( !reader.IsNext ( '(' ) )
                return null;
            reader.Advance ( 1 );

            GrammarNode<Char> innerNode = this.MainParse ( );

            if ( !reader.IsNext ( ')' ) )
                throw new RegexParseException ( start.To ( reader.Location ), "Expected closing ')' for capture group." );
            reader.Advance ( 1 );

            return new NumberedCapture ( ++this.lastCaptureGroupNumber, innerNode );
        }

        /// <summary>
        /// named_capture_group
        /// 	: '(?&lt;' WORD_CHAR+ '&lt;' expression ')'
        /// 	;
        /// </summary>
        /// <returns></returns>
        private NamedCapture? ParseNamedCaptureGroup ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
            if ( !reader.IsNext ( "(?<" ) )
                return null;
            reader.Advance ( 3 );

            var name = reader.ReadStringWhile ( ch => IsWordChar ( ch ) );

            if ( name.Length < 1 )
                throw new RegexParseException ( start.To ( reader.Location ), "Invalid named capture group name." );
            if ( !reader.IsNext ( '>' ) )
                throw new RegexParseException ( start.To ( reader.Location ), "Expected closing '>' for named capture group name." );
            reader.Advance ( 1 );

            GrammarNode<Char> innerNode = this.MainParse ( );

            if ( !reader.IsNext ( ')' ) )
                throw new RegexParseException ( start.To ( reader.Location ), "Expected closing ')' for named capture group." );
            reader.Advance ( 1 );

            return new NamedCapture ( name, innerNode );
        }

        /// <summary>
        /// Parses an atom.
        /// </summary>
        /// <returns></returns>
        private GrammarNode<Char>? ParseAtom ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
            if ( this.ParseCharacterClass ( ) is GrammarNode<Char> characterClass )
            {
                return characterClass;
            }
            else if ( reader.IsNext ( '\\' ) )
            {
                if ( reader.IsAt ( 'k', 1 ) )
                {
                    return this.ParseNamedBackreference ( );
                }
                else if ( this.ParseNumberedBackreference ( ) is GrammarNode<Char> numberedBackreference )
                {
                    return numberedBackreference;
                }
                else
                {
                    if ( this.ParseEscapedChar ( ) is Char escapedChar )
                    {
                        return new CharacterTerminal ( escapedChar );
                    }
                    else
                    {
                        reader.Advance ( 2 );
                        throw new RegexParseException ( start.To ( reader.Location ), "Invalid escape sequence." );
                    }
                }
            }
            else if ( reader.IsNext ( '(' ) )
            {
                if ( reader.IsAt ( '?', 1 ) )
                {
                    if ( reader.IsAt ( ':', 2 ) )
                    {
                        return this.ParseNonCapturingGroup ( );
                    }
                    else if ( reader.IsAt ( '<', 2 ) )
                    {
                        return this.ParseNamedCaptureGroup ( );
                    }
                    else if ( this.ParseLookahead ( ) is GrammarNode<Char> lookahead )
                    {
                        return lookahead;
                    }
                    else
                    {
                        SourceLocation pos = reader.Location;
                        reader.Advance ( 2 );
                        throw new RegexParseException ( pos.To ( reader.Location ), "Unrecognized group type." );
                    }
                }
                else
                {
                    return this.ParseNumberedCaptureGroup ( );
                }
            }
            else if ( reader.IsNext ( '[' ) )
            {
                return this.ParseSet ( );
            }
            else if ( this.ParseChar ( ) is Char ch )
            {
                return new CharacterTerminal ( ch );
            }
            return null;
        }

        private RepetitionRange? ParseRepetitionRange ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
#if HAS_SPAN
            ReadOnlySpan<Char> rawMinimum = reader.ReadSpanWhile ( ch => IsDecimalChar ( ch ) );
#else
            var rawMinimum = reader.ReadStringWhile ( ch => IsDecimalChar ( ch ) );
#endif
            if ( !UInt32.TryParse ( rawMinimum, NumberStyles.None, CultureInfo.InvariantCulture, out var minimum ) )
                goto repetitionParseFail;

            UInt32? maximum = minimum;
            if ( reader.IsNext ( ',' ) )
            {
                reader.Advance ( 1 );
                if ( reader.IsNext ( '}' ) )
                {
                    maximum = null;
                }
                else
                {
#if HAS_SPAN
                    ReadOnlySpan<Char> rawMaximum = reader.ReadSpanWhile ( ch => IsDecimalChar ( ch ) );
#else
                    var rawMaximum = reader.ReadStringWhile ( ch => IsDecimalChar ( ch ) );
#endif
                    if ( !UInt32.TryParse ( rawMaximum, NumberStyles.None, CultureInfo.InvariantCulture, out var tmpMaximum ) )
                        goto repetitionParseFail;

                    maximum = tmpMaximum;

                }
            }

            return new RepetitionRange ( minimum, maximum );

        repetitionParseFail:
            reader.Restore ( start );
            return null;
        }

        private GrammarNode<Char>? ParseSuffixed ( )
        {
            ICodeReader reader = this._reader;
            SourceLocation start = reader.Location;
            GrammarNode<Char>? node = this.ParseAtom ( );

            while ( reader.Peek ( ) is Char ch && isRepetitionChar ( ch ) )
            {
                switch ( ch )
                {
                    case '?':
                    case '*':
                    case '+':
                    {
                        var op = reader.Read ( )!;
                        if ( node is null )
                            throw new RegexParseException ( start.To ( reader.Location ), $"Repetiton operator '{op}' following nothing." );
                        var isLazy = reader.IsNext ( '?' );
                        node = new Repetition<Char> ( node!, op switch
                        {
                            '?' => new RepetitionRange ( 0, 1 ),
                            '*' => new RepetitionRange ( 0, null ),
                            '+' => new RepetitionRange ( 1, null ),
                            _ => throw new InvalidOperationException ( "Unreachable." )
                        }, isLazy );

                        if ( isLazy )
                            reader.Advance ( 1 );

                        checkRepetitionChar ( );
                        break;
                    }

                    case '{':
                    {
                        SourceLocation repStart = reader.Location;
                        if ( reader.Peek ( 2 ) is not Char peek || !IsDecimalChar ( peek ) )
                            break;
                        reader.Advance ( 1 ); // Skip over the '{'

                        RepetitionRange? range = this.ParseRepetitionRange ( );
                        if ( range is null || !reader.IsNext ( '}' ) )
                            goto repetitionParseFail;

                        var isLazy = reader.IsNext ( '?' );
                        node = new Repetition<Char> ( node!, range.Value, isLazy );
                        if ( isLazy )
                            reader.Advance ( 1 );

                        checkRepetitionChar ( );
                        break;

                    repetitionParseFail:
                        reader.Restore ( repStart );
                        break;
                    }
                }
            }

            return node;

            [MethodImpl ( MethodImplOptions.AggressiveInlining )]
            static Boolean isRepetitionChar ( Char ch ) => ch is '?' or '*' or '+' or '{';
            Boolean hasValidRepetitionRangeNext ( )
            {
                SourceLocation start = reader.Location;
                reader.Advance ( 1 ); // Skip over '{'
                RepetitionRange? res = this.ParseRepetitionRange ( );
                reader.Restore ( start );
                return res.HasValue;
            }
            [MethodImpl ( MethodImplOptions.AggressiveInlining )]
            void checkRepetitionChar ( )
            {
                if ( reader.Peek ( ) is Char peek && isRepetitionChar ( peek ) && ( peek != '{' || !hasValidRepetitionRangeNext ( ) ) )
                    throw new RegexParseException ( reader.Location, $"Invalid nested repetition operator '{peek}'. To nest repetitions wrap them in a group of any kind." );
            }
        }

        private GrammarNode<Char> ParseSequence ( )
        {
            var nodes = new List<GrammarNode<Char>> ( );
            while ( this.ParseSuffixed ( ) is GrammarNode<Char> node )
                nodes.Add ( node );
            return nodes.Count == 1
                ? nodes[0]
                : new Sequence<Char> ( nodes );
        }

        private GrammarNode<Char> MainParse ( )
        {
            ICodeReader reader = this._reader;
            GrammarNode<Char> node = this.ParseSequence ( );
            while ( reader.IsNext ( '|' ) )
            {
                reader.Advance ( 1 );
                GrammarNode<Char> right = this.ParseSequence ( );
                node = new Alternation<Char> ( node, right );
            }
            return node;
        }
    }
}
