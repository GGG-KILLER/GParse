using System;
using System.Linq;
using GParse.Composable;

namespace GParse.Lexing.Composable
{

#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error
    /// <summary>
    /// The class that contains the logic for converting a <see cref="GrammarNode{System.Char}"/> to a regex-like string
    /// </summary>
#pragma warning restore CS1658 // Warning is overriding an error
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
    public class GrammarNodeToStringConverter : GrammarTreeVisitor<String>
    {
        /// <summary>
        /// Initializes this grammar node converter
        /// </summary>
        protected GrammarNodeToStringConverter ( )
        {
        }

        /// <summary>
        /// Converts an alternation into a regex-like string
        /// </summary>
        /// <param name="alternation"></param>
        /// <returns></returns>
        protected override String VisitAlternation ( Alternation<Char> alternation ) =>
            $"(?:{String.Join ( ")|(?:", alternation.GrammarNodes.Select ( Convert ) )})";

        /// <summary>
        /// Converts a sequence into a regex-like string
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected override String VisitSequence ( Sequence<Char> sequence ) =>
            String.Join ( "", sequence.GrammarNodes.Select ( Convert ) );

        /// <summary>
        /// Converts a repetition into a regex-like string
        /// </summary>
        /// <param name="repetition"></param>
        /// <returns></returns>
        protected override String VisitRepetition ( Repetition<Char> repetition ) =>
            $"(?:{this.Visit ( repetition.InnerNode )}){{{repetition.Range.Minimum},{repetition.Range.Maximum}}}";

        /// <summary>
        /// Converts a negated character terminal into a regex-like string
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <returns></returns>
        protected override String VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal ) =>
            $"[^{negatedCharacterTerminal.Value}]";

        /// <summary>
        /// Converts a character terminal into a regex-like string
        /// </summary>
        /// <param name="characterTerminal"></param>
        /// <returns></returns>
        protected override String VisitCharacterTerminal ( CharacterTerminal characterTerminal ) =>
            $"{characterTerminal.Value}";

        /// <summary>
        /// Converts a negated character range into a regex-like string
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        /// <returns></returns>
        protected override String VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange ) =>
            $"[^{negatedCharacterRange.Start}-{negatedCharacterRange.End}]";

        /// <summary>
        /// Converts a character range into a regex-like string
        /// </summary>
        /// <param name="characterRange"></param>
        /// <returns></returns>
        protected override String VisitCharacterRange ( CharacterRange characterRange ) =>
            $"[{characterRange.Start}-{characterRange.End}]";

        /// <summary>
        /// Converts a character terminal string into a regex-like string
        /// </summary>
        /// <param name="characterTerminalString"></param>
        /// <returns></returns>
        protected override String VisitCharacterTerminalString ( StringTerminal characterTerminalString ) =>
            characterTerminalString.String;

        /// <summary>
        /// Converts a named capture into a regex-like string
        /// </summary>
        /// <param name="namedCapture"></param>
        /// <returns></returns>
        protected override String VisitNamedCapture ( NamedCapture namedCapture ) =>
            $"(?<{namedCapture.Name}>{this.Visit ( namedCapture.InnerNode )})";

        /// <summary>
        /// Converts a negated character set into a regex-like string
        /// </summary>
        /// <param name="negatedCharacterSet"></param>
        /// <returns></returns>
        protected override String VisitNegatedCharacterSet ( NegatedCharacterSet negatedCharacterSet ) =>
            $"[^{String.Join("", negatedCharacterSet.CharSet)}]";

        /// <summary>
        /// Converts a character set into a regex-like string
        /// </summary>
        /// <param name="characterSet"></param>
        /// <returns></returns>
        protected override String VisitCharacterSet ( CharacterSet characterSet ) =>
            $"[{String.Join ( "", characterSet.CharSet )}]";

        /// <summary>
        /// The global instance of the converter
        /// </summary>
        private static readonly GrammarNodeToStringConverter grammarNodeToStringConverter = new GrammarNodeToStringConverter ( );

        /// <summary>
        /// Converts a grammar node into a regex-like string
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static String Convert ( GrammarNode<Char> node ) =>
            grammarNodeToStringConverter.Visit ( node );
    }
}