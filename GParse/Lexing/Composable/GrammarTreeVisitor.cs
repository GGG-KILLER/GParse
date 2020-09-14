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
        /// Visits an alternation node
        /// </summary>
        /// <param name="alternation"></param>
        /// <returns></returns>
        protected abstract T VisitAlternation ( Alternation<Char> alternation );

        /// <summary>
        /// Visits a sequence node
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected abstract T VisitSequence ( Sequence<Char> sequence );

        /// <summary>
        /// Visits a repetition node
        /// </summary>
        /// <param name="repetition"></param>
        /// <returns></returns>
        protected abstract T VisitRepetition ( Repetition<Char> repetition );

        /// <summary>
        /// Visits a negated character node
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract T VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal );

        /// <summary>
        /// Visits a character terminal
        /// </summary>
        /// <param name="characterTerminal"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract T VisitCharacterTerminal ( CharacterTerminal characterTerminal );

        /// <summary>
        /// Visits a negated character range
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract T VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange );

        /// <summary>
        /// Visits a character range
        /// </summary>
        /// <param name="characterRange"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract T VisitCharacterRange ( CharacterRange characterRange );

        /// <summary>
        /// Visits a character terminal string
        /// </summary>
        /// <param name="characterTerminalString"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract T VisitCharacterTerminalString ( StringTerminal characterTerminalString );

        /// <summary>
        /// Visits a named capture
        /// </summary>
        /// <param name="namedCapture"></param>
        /// <returns></returns>
        protected abstract T VisitNamedCapture ( NamedCapture namedCapture );

        /// <summary>
        /// Visits a negated character set
        /// </summary>
        /// <param name="negatedCharacterSet"></param>
        /// <returns></returns>
        protected abstract T VisitNegatedCharacterSet ( NegatedCharacterSet negatedCharacterSet );

        /// <summary>
        /// Visits a character set
        /// </summary>
        /// <param name="characterSet"></param>
        /// <returns></returns>
        protected abstract T VisitCharacterSet ( CharacterSet characterSet );

        /// <summary>
        /// Visits a grammar node
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
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
                StringTerminal characterTerminalString => this.VisitCharacterTerminalString ( characterTerminalString ),
                NamedCapture namedCapture => this.VisitNamedCapture ( namedCapture ),
                NegatedCharacterSet negatedCharacterSet => this.VisitNegatedCharacterSet ( negatedCharacterSet ),
                CharacterSet characterSet => this.VisitCharacterSet ( characterSet ),
                _ => throw new NotSupportedException ( )
            };
        }
    }
}