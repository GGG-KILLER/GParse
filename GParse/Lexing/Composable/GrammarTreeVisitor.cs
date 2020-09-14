using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error
    /// <summary>
    /// The base class for a <see cref="GrammarNode{System.Char}"/> visitor
    /// </summary>
    /// <typeparam name="T">The type that will be returned by each visitor method</typeparam>
#pragma warning restore CS1658 // Warning is overriding an error
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute

    public abstract class GrammarTreeVisitor<T>
    {
        /// <summary>
        /// Visits an alternation node.
        /// </summary>
        /// <param name="alternation"></param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitAlternation ( Alternation<Char> alternation );

        /// <summary>
        /// Visits a sequence node.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitSequence ( Sequence<Char> sequence );

        /// <summary>
        /// Visits a repetition node.
        /// </summary>
        /// <param name="repetition"></param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitRepetition ( Repetition<Char> repetition );

        /// <summary>
        /// Visits a negated character node.
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <returns>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal );

        /// <summary>
        /// Visits a character terminal.
        /// </summary>
        /// <param name="characterTerminal"></param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitCharacterTerminal ( CharacterTerminal characterTerminal );

        /// <summary>
        /// Visits a negated character range.
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange );

        /// <summary>
        /// Visits a character range.
        /// </summary>
        /// <param name="characterRange"></param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitCharacterRange ( CharacterRange characterRange );

        /// <summary>
        /// Visits a string terminal.
        /// </summary>
        /// <param name="characterTerminalString"></param>
        /// <returns>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitStringTerminal ( StringTerminal characterTerminalString );

        /// <summary>
        /// Visits a named capture.
        /// </summary>
        /// <param name="namedCapture"></param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitNamedCapture ( NamedCapture namedCapture );

        /// <summary>
        /// Visits a negated character set.
        /// </summary>
        /// <param name="negatedCharacterSet"></param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitNegatedCharacterSet ( NegatedCharacterSet negatedCharacterSet );

        /// <summary>
        /// Visits a character set.
        /// </summary>
        /// <param name="characterSet"></param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract T VisitCharacterSet ( CharacterSet characterSet );

        /// <summary>
        /// Visits a grammar node.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <returns>
        /// <returns>The result of visiting this node.</returns>
        public virtual T Visit ( GrammarNode<Char> grammarNode )
        {
            return grammarNode switch
            {
                Alternation<Char> alternation => this.VisitAlternation ( alternation ),
                Sequence<Char> sequence => this.VisitSequence ( sequence ),
                Repetition<Char> repetition => this.VisitRepetition ( repetition ),
                NegatedCharacterTerminal negatedCharacterTerminal => this.VisitNegatedCharacterTerminal ( negatedCharacterTerminal ),
                CharacterTerminal characterTerminal => this.VisitCharacterTerminal ( characterTerminal ),
                NegatedCharacterRange negatedCharacterRange => this.VisitNegatedCharacterRange ( negatedCharacterRange ),
                CharacterRange characterRange => this.VisitCharacterRange ( characterRange ),
                StringTerminal characterTerminalString => this.VisitStringTerminal ( characterTerminalString ),
                NamedCapture namedCapture => this.VisitNamedCapture ( namedCapture ),
                NegatedCharacterSet negatedCharacterSet => this.VisitNegatedCharacterSet ( negatedCharacterSet ),
                CharacterSet characterSet => this.VisitCharacterSet ( characterSet ),
                _ => throw new NotSupportedException ( )
            };
        }
    }
}