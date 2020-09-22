using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Composable;
using GParse.Utilities;

namespace GParse.Lexing.Composable
{

#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error
    /// <summary>
    /// The class that contains the logic for converting a <see cref="GrammarNode{System.Char}"/> to a regex-like string
    /// </summary>
#pragma warning restore CS1658 // Warning is overriding an error
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
    public class GrammarNodeToStringConverter : GrammarTreeVisitor<String, GrammarNodeToStringConverter.ConversionArguments>
    {
        /// <summary>
        /// The arguments for the converion methods.
        /// </summary>
        public readonly struct ConversionArguments
        {
            /// <summary>
            /// Whether to wrap the output with a non-capturing group.
            /// </summary>
            public Boolean WrapOutput { get; }

            /// <summary>
            /// Whether to add alternation brackets to the output for <see cref="CharacterRange"/>,
            /// <see cref="CharacterSet"/>, <see cref="NegatedCharacterRange"/>, <see cref="NegatedCharacterSet"/>,
            /// <see cref="NegatedCharacterTerminal"/> and <see cref="NegatedUnicodeCategoryTerminal"/>.
            /// </summary>
            public Boolean OmmitAlternationBrackets { get; }

            /// <summary>
            /// Initializes a new conversion argument struct.
            /// </summary>
            /// <param name="wrapOutput"><inheritdoc cref="WrapOutput" path="/summary"/></param>
            /// <param name="ommitAlternationBrackets"><inheritdoc cref="OmmitAlternationBrackets" path="/summary"/></param>
            public ConversionArguments ( Boolean wrapOutput, Boolean ommitAlternationBrackets )
            {
                this.WrapOutput = wrapOutput;
                this.OmmitAlternationBrackets = ommitAlternationBrackets;
            }
        }

        /// <summary>
        /// The global instance of the converter.
        /// </summary>
        private static readonly GrammarNodeToStringConverter grammarNodeToStringConverter = new GrammarNodeToStringConverter ( );

        /// <summary>
        /// Converts a grammar node into a regex-like string.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static String Convert ( GrammarNode<Char> node ) =>
            grammarNodeToStringConverter.Visit ( node, new ConversionArguments ( false, false ) );

        /// <summary>
        /// Initializes this grammar node converter
        /// </summary>
        protected GrammarNodeToStringConverter ( )
        {
        }

        /// <summary>
        /// Converts an alternation into a regex-like string.
        /// </summary>
        /// <param name="alternation"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitAlternation ( Alternation<Char> alternation, ConversionArguments argument )
        {
            if ( alternation.GrammarNodes.All ( node => node is CharacterRange or CharacterSet or CharacterTerminal or UnicodeCategoryTerminal ) )
            {
                IEnumerable<String> nodes = alternation.GrammarNodes.Select ( node => this.Visit ( node, new ConversionArguments ( false, true ) ) );
                return $"[{String.Join ( "", nodes )}]";
            }
            else
            {
                var value = String.Join ( "|", alternation.GrammarNodes.Select ( node => this.Visit ( node, new ConversionArguments ( true, false ) ) ) );
                if ( !argument.WrapOutput )
                    return value;
                else
                    return $"(?:{value})";
            }
        }

        /// <summary>
        /// Converts a sequence into a regex-like string.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitSequence ( Sequence<Char> sequence, ConversionArguments argument )
        {
            var value = String.Join ( "", sequence.GrammarNodes.Select ( node => this.Visit ( node, new ConversionArguments ( false, false ) ) ) );
            if ( !argument.WrapOutput )
                return value;
            else
                return $"(?:{value})";
        }

        /// <summary>
        /// Converts a repetition into a regex-like string.
        /// </summary>
        /// <param name="repetition"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitRepetition ( Repetition<Char> repetition, ConversionArguments argument ) =>
            $"(?:{this.Visit ( repetition.InnerNode, new ConversionArguments ( false, false ) )}){{{repetition.Range.Minimum},{repetition.Range.Maximum}}}";

        /// <summary>
        /// Converts a negated character terminal into a regex-like string.
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal, ConversionArguments argument )
        {
            if ( argument.OmmitAlternationBrackets )
                return CharUtils.ToReadableString ( negatedCharacterTerminal.Value );
            else
                return $"[^{CharUtils.ToReadableString ( negatedCharacterTerminal.Value )}]";
        }

        /// <summary>
        /// Converts a character terminal into a regex-like string.
        /// </summary>
        /// <param name="characterTerminal"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitCharacterTerminal ( CharacterTerminal characterTerminal, ConversionArguments argument ) =>
            CharUtils.ToReadableString ( characterTerminal.Value );

        /// <summary>
        /// Converts a negated character range into a regex-like string.
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange, ConversionArguments argument )
        {
            if ( argument.OmmitAlternationBrackets )
                return $"{CharUtils.ToReadableString ( negatedCharacterRange.Start )}-{CharUtils.ToReadableString ( negatedCharacterRange.End )}";
            else
                return $"[^{CharUtils.ToReadableString ( negatedCharacterRange.Start )}-{CharUtils.ToReadableString ( negatedCharacterRange.End )}]";
        }

        /// <summary>
        /// Converts a character range into a regex-like string.
        /// </summary>
        /// <param name="characterRange"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitCharacterRange ( CharacterRange characterRange, ConversionArguments argument )
        {
            if ( argument.OmmitAlternationBrackets )
                return $"{CharUtils.ToReadableString ( characterRange.Start )}-{CharUtils.ToReadableString ( characterRange.End )}";
            else
                return $"[{CharUtils.ToReadableString ( characterRange.Start )}-{CharUtils.ToReadableString ( characterRange.End )}]";
        }

        /// <summary>
        /// Converts a character terminal string into a regex-like string.
        /// </summary>
        /// <param name="characterTerminalString"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitStringTerminal ( StringTerminal characterTerminalString, ConversionArguments argument ) =>
            characterTerminalString.String;

        /// <summary>
        /// Converts a named capture into a regex-like string.
        /// </summary>
        /// <param name="namedCapture"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNamedCapture ( NamedCapture namedCapture, ConversionArguments argument ) =>
            $"(?<{namedCapture.Name}>{this.Visit ( namedCapture.InnerNode, new ConversionArguments ( false, false ) )})";

        /// <summary>
        /// Converts a negated character set into a regex-like string.
        /// </summary>
        /// <param name="negatedCharacterSet"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNegatedCharacterSet ( NegatedCharacterSet negatedCharacterSet, ConversionArguments argument )
        {
            if ( argument.OmmitAlternationBrackets )
                return String.Join ( "", negatedCharacterSet.CharSet.Select ( ch => CharUtils.ToReadableString ( ch ) ) );
            else
                return $"[^{String.Join ( "", negatedCharacterSet.CharSet.Select ( ch => CharUtils.ToReadableString ( ch ) ) )}]";
        }

        /// <summary>
        /// Converts a character set into a regex-like string.
        /// </summary>
        /// <param name="characterSet"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitCharacterSet ( CharacterSet characterSet, ConversionArguments argument )
        {
            if ( argument.OmmitAlternationBrackets )
                return String.Join ( "", characterSet.CharSet.Select ( ch => CharUtils.ToReadableString ( ch ) ) );
            else
                return $"[{String.Join ( "", characterSet.CharSet.Select ( ch => CharUtils.ToReadableString ( ch ) ) )}]";
        }

        /// <summary>
        /// Converts an optional node into a regex string.
        /// </summary>
        /// <param name="optional"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitOptional ( Optional<Char> optional, ConversionArguments argument ) =>
            this.Visit ( optional.InnerNode, new ConversionArguments ( true, false ) ) + '?';

        /// <summary>
        /// Converts a lazy node into a regex string.
        /// </summary>
        /// <param name="lazy"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitLazy ( Lazy lazy, ConversionArguments argument ) =>
            this.Visit ( lazy.InnerNode, new ConversionArguments ( true, false ) ) + '?';

        /// <summary>
        /// Converts a lookahead into a regex string.
        /// </summary>
        /// <param name="lookahead"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitLookahead ( Lookahead lookahead, ConversionArguments argument ) =>
            $"(?={this.Visit ( lookahead.InnerNode, new ConversionArguments ( false, false ) )})";

        /// <summary>
        /// Converts a named backreference into a regex string.
        /// </summary>
        /// <param name="namedBackreference"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNamedBackreference ( NamedBackreference namedBackreference, ConversionArguments argument ) =>
            $"\\k<{namedBackreference.Name}>";

        /// <summary>
        /// Converts a negated alternation into a regex string.
        /// </summary>
        /// <param name="negatedAlternation"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNegatedAlternation ( NegatedAlternation negatedAlternation, ConversionArguments argument ) =>
            $"[^{String.Join ( "", negatedAlternation.GrammarNodes.Select ( node => this.Visit ( node, new ConversionArguments ( false, true ) ) ) )}]";

        /// <summary>
        /// Converts a negated unicode category terminal into a regex string.
        /// </summary>
        /// <param name="negatedUnicodeCategoryTerminal"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNegatedUnicodeCategoryTerminal ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal, ConversionArguments argument )
        {
            var categoryName = RegexUtils.CharacterCategories.ToString ( negatedUnicodeCategoryTerminal.Category );
            if ( argument.OmmitAlternationBrackets )
                return $"\\p{{{categoryName}}}";
            else
                return $"\\P{{{categoryName}}}";
        }

        /// <summary>
        /// Converts a negative lookahead into a regex string.
        /// </summary>
        /// <param name="negativeLookahead"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNegativeLookahead ( NegativeLookahead negativeLookahead, ConversionArguments argument ) =>
            $"(?!{this.Visit ( negativeLookahead.InnerNode, new ConversionArguments ( false, false ) )})";

        /// <summary>
        /// Converts a numbered backreference into a regex string.
        /// </summary>
        /// <param name="numberedBackreference"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNumberedBackreference ( NumberedBackreference numberedBackreference, ConversionArguments argument ) =>
            $"\\{numberedBackreference.Position}";

        /// <summary>
        /// Converts a numbered capture into a regex string.
        /// </summary>
        /// <param name="numberedCapture"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNumberedCapture ( NumberedCapture numberedCapture, ConversionArguments argument ) =>
            $"({this.Visit ( numberedCapture.InnerNode, new ConversionArguments ( false, false ) )})";

        /// <summary>
        /// Converts a unicode category terminal into a regex string.
        /// </summary>
        /// <param name="unicodeCategoryTerminal"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitUnicodeCategoryTerminal ( UnicodeCategoryTerminal unicodeCategoryTerminal, ConversionArguments argument ) =>
            $"\\p{{{RegexUtils.CharacterCategories.ToString ( unicodeCategoryTerminal.Category )}}}";
    }
}