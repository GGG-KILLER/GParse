using System;
using System.Globalization;
using System.Linq;
using System.Text;
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
    public sealed class GrammarNodeToStringConverter : GrammarTreeVisitor<String, GrammarNodeToStringConverter.ConversionArguments>
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
            /// <see cref="Set"/>, <see cref="NegatedCharacterRange"/>, <see cref="NegatedSet"/>,
            /// <see cref="NegatedCharacterTerminal"/> and <see cref="NegatedUnicodeCategoryTerminal"/>.
            /// </summary>
            public Boolean SuppressAlternationBrackets { get; }

            /// <summary>
            /// Initializes a new conversion argument struct.
            /// </summary>
            /// <param name="wrapOutput"><inheritdoc cref="WrapOutput" path="/summary"/></param>
            /// <param name="suppressAlternationBrackets"><inheritdoc cref="SuppressAlternationBrackets" path="/summary"/></param>
            public ConversionArguments ( Boolean wrapOutput, Boolean suppressAlternationBrackets )
            {
                this.WrapOutput = wrapOutput;
                this.SuppressAlternationBrackets = suppressAlternationBrackets;
            }
        }

        private static String WrapWithAlternationBrackets ( Boolean isNegated, String content, Boolean suppressAlternationBrackets )
        {
            if ( !suppressAlternationBrackets )
            {
                if ( isNegated )
                    return $"[^{content}]";
                else
                    return $"[{content}]";
            }
            return content;
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
        private GrammarNodeToStringConverter ( )
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
            var value = String.Join ( "|", alternation.GrammarNodes.Select ( node => this.Visit ( node, new ConversionArguments ( true, false ) ) ) );
            if ( argument.WrapOutput )
                value = $"(?:{value})";
            return value;
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
        protected override String VisitRepetition ( Repetition<Char> repetition, ConversionArguments argument )
        {
            var ret = $"(?:{this.Visit ( repetition.InnerNode, new ConversionArguments ( false, false ) )})";

            ret = repetition.Range switch
            {
                { Minimum: 0, Maximum: 1 } => ret + '?',
                { Minimum: 0, Maximum: null } => ret + '*',
                { Minimum: 1, Maximum: null } => ret + '+',
                _ => ret + $"{{{repetition.Range.Minimum},{repetition.Range.Maximum}}}",
            };

            if ( repetition.IsLazy )
                ret += '?';

            return ret;
        }

        /// <summary>
        /// Converts a negated character terminal into a regex-like string.
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal, ConversionArguments argument ) =>
            WrapWithAlternationBrackets ( true, CharUtils.RegexEscape ( negatedCharacterTerminal.Value ), argument.SuppressAlternationBrackets );

        /// <summary>
        /// Converts a character terminal into a regex-like string.
        /// </summary>
        /// <param name="characterTerminal"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitCharacterTerminal ( CharacterTerminal characterTerminal, ConversionArguments argument ) =>
            CharUtils.RegexEscape ( characterTerminal.Value );

        /// <summary>
        /// Converts a negated character range into a regex-like string.
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange, ConversionArguments argument ) =>
            WrapWithAlternationBrackets (
                true,
                $"{CharUtils.RegexEscape ( negatedCharacterRange.Range.Start )}-{CharUtils.RegexEscape ( negatedCharacterRange.Range.End )}",
                argument.SuppressAlternationBrackets );

        /// <summary>
        /// Converts a character range into a regex-like string.
        /// </summary>
        /// <param name="characterRange"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitCharacterRange ( CharacterRange characterRange, ConversionArguments argument ) =>
            WrapWithAlternationBrackets (
                false,
                $"{CharUtils.RegexEscape ( characterRange.Range.Start )}-{CharUtils.RegexEscape ( characterRange.Range.End )}",
                argument.SuppressAlternationBrackets );

        /// <summary>
        /// Converts a character terminal string into a regex-like string.
        /// </summary>
        /// <param name="characterTerminalString"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitStringTerminal ( StringTerminal characterTerminalString, ConversionArguments argument ) =>
            String.Join ( "", characterTerminalString.String.Select ( CharUtils.RegexEscape ) );

        /// <summary>
        /// Converts a named capture into a regex-like string.
        /// </summary>
        /// <param name="namedCapture"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNamedCapture ( NamedCapture namedCapture, ConversionArguments argument ) =>
            $"(?<{namedCapture.Name}>{this.Visit ( namedCapture.InnerNode, new ConversionArguments ( false, false ) )})";

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
        protected override String VisitNegatedUnicodeCategoryTerminal ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal, ConversionArguments argument ) =>
            $"\\P{{{RegexUtils.CharacterCategories.ToString ( negatedUnicodeCategoryTerminal.Category )}}}";

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

        /// <summary>
        /// Converts an any node into a regex string.
        /// </summary>
        /// <param name="any"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitAny ( Any any, ConversionArguments argument ) => @"[\S\s]";

        /// <summary>
        /// Converts a set into a regex string.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitSet ( Set set, ConversionArguments argument )
        {
            var builder = new StringBuilder ( );
            foreach ( var character in set.Characters )
                builder.Append ( CharUtils.ToReadableString ( character ) );
            foreach ( Math.Range<Char> range in CharUtils.UnflattenRanges ( set.FlattenedRanges ) )
                builder.Append ( $"{CharUtils.ToReadableString ( range.Start )}-{CharUtils.ToReadableString ( range.End )}" );
            foreach ( UnicodeCategory category in CharUtils.CategoriesFromFlagSet ( set.UnicodeCategoryFlagSet ) )
                builder.Append ( $"\\p{{{RegexUtils.CharacterCategories.ToString ( category )}}}" );
            return WrapWithAlternationBrackets ( false, builder.ToString ( ), argument.SuppressAlternationBrackets );
        }

        /// <summary>
        /// Converts a negated set into a regex string.
        /// </summary>
        /// <param name="negatedSet"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override String VisitNegatedSet ( NegatedSet negatedSet, ConversionArguments argument )
        {
            var builder = new StringBuilder ( );
            foreach ( var character in negatedSet.Characters )
                builder.Append ( CharUtils.ToReadableString ( character ) );
            foreach ( Math.Range<Char> range in CharUtils.UnflattenRanges ( negatedSet.Ranges ) )
                builder.Append ( $"{CharUtils.ToReadableString ( range.Start )}-{CharUtils.ToReadableString ( range.End )}" );
            foreach ( UnicodeCategory category in CharUtils.CategoriesFromFlagSet ( negatedSet.UnicodeCategoryFlagSet ) )
                builder.Append ( $"\\p{{{RegexUtils.CharacterCategories.ToString ( category )}}}" );
            return WrapWithAlternationBrackets ( true, builder.ToString ( ), argument.SuppressAlternationBrackets );
        }
    }
}