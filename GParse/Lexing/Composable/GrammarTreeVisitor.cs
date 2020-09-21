using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{

#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error
    /// <summary>
    /// The base class for a <see cref="GrammarNode{System.Char}"/> visitor.
    /// </summary>
    /// <typeparam name="TReturn">The type that will be returned by each visitor method.</typeparam>
    /// <typeparam name="TArgument">The type of the argument that will be passed to each visitor method.</typeparam>
    public abstract class GrammarTreeVisitor<TReturn, TArgument>
#pragma warning restore CS1658 // Warning is overriding an error
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
    {
        /// <summary>
        /// Visits an alternation node.
        /// </summary>
        /// <param name="alternation"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitAlternation ( Alternation<Char> alternation, TArgument argument );

        /// <summary>
        /// Visits an optional node.
        /// </summary>
        /// <param name="optional"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitOptional ( Optional<Char> optional, TArgument argument );

        /// <summary>
        /// Visits a repetition node.
        /// </summary>
        /// <param name="repetition"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitRepetition ( Repetition<Char> repetition, TArgument argument );

        /// <summary>
        /// Visits a sequence node.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitSequence ( Sequence<Char> sequence, TArgument argument );

        /// <summary>
        /// Visits a character range.
        /// </summary>
        /// <param name="characterRange"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitCharacterRange ( CharacterRange characterRange, TArgument argument );

        /// <summary>
        /// Visits a character set.
        /// </summary>
        /// <param name="characterSet"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitCharacterSet ( CharacterSet characterSet, TArgument argument );

        /// <summary>
        /// Visits a character terminal.
        /// </summary>
        /// <param name="characterTerminal"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitCharacterTerminal ( CharacterTerminal characterTerminal, TArgument argument );

        /// <summary>
        /// Visits a lazy repetition.
        /// </summary>
        /// <param name="lazy"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitLazy ( Lazy lazy, TArgument argument );


        /// <summary>
        /// Visits a lookahead.
        /// </summary>
        /// <param name="lookahead"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitLookahead ( Lookahead lookahead, TArgument argument );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namedBackreference"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNamedBackreference ( NamedBackreference namedBackreference, TArgument argument );

        /// <summary>
        /// Visits a named capture.
        /// </summary>
        /// <param name="namedCapture"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNamedCapture ( NamedCapture namedCapture, TArgument argument );

        /// <summary>
        /// Visits a negated alternation.
        /// </summary>
        /// <param name="negatedAlternation"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNegatedAlternation ( NegatedAlternation negatedAlternation, TArgument argument );

        /// <summary>
        /// Visits a negated character range.
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange, TArgument argument );

        /// <summary>
        /// Visits a negated character set.
        /// </summary>
        /// <param name="negatedCharacterSet"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNegatedCharacterSet ( NegatedCharacterSet negatedCharacterSet, TArgument argument );

        /// <summary>
        /// Visits a negated character node.
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal, TArgument argument );

        /// <summary>
        /// Visits a negated unicode category terminal.
        /// </summary>
        /// <param name="negatedUnicodeCategoryTerminal"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNegatedUnicodeCategoryTerminal ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal, TArgument argument );

        /// <summary>
        /// Visits a negative lookahead.
        /// </summary>
        /// <param name="negativeLookahead"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNegativeLookahead ( NegativeLookahead negativeLookahead, TArgument argument );

        /// <summary>
        /// Visits a numbered backreference.
        /// </summary>
        /// <param name="numberedBackreference"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNumberedBackreference ( NumberedBackreference numberedBackreference, TArgument argument );

        /// <summary>
        /// Visits a numbered capture.
        /// </summary>
        /// <param name="numberedCapture"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNumberedCapture ( NumberedCapture numberedCapture, TArgument argument );

        /// <summary>
        /// Visits a string terminal.
        /// </summary>
        /// <param name="characterTerminalString"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitStringTerminal ( StringTerminal characterTerminalString, TArgument argument );

        /// <summary>
        /// Visits a unicode category terminal.
        /// </summary>
        /// <param name="unicodeCategoryTerminal"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitUnicodeCategoryTerminal ( UnicodeCategoryTerminal unicodeCategoryTerminal, TArgument argument );

        /// <summary>
        /// Visits a grammar node.
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="argument">The argument data to be passed to the visitor methods.</param>
        /// <returns>The result of visiting this node.</returns>
        public virtual TReturn Visit ( GrammarNode<Char> grammarNode, TArgument argument )
        {
            if ( grammarNode is null )
                throw new ArgumentNullException ( nameof ( grammarNode ) );

            return grammarNode switch
            {
                Alternation<Char> alternation => this.VisitAlternation ( alternation, argument ),
                Optional<Char> optional => this.VisitOptional ( optional, argument ),
                Repetition<Char> repetition => this.VisitRepetition ( repetition, argument ),
                Sequence<Char> sequence => this.VisitSequence ( sequence, argument ),
                CharacterRange characterRange => this.VisitCharacterRange ( characterRange, argument ),
                CharacterSet characterSet => this.VisitCharacterSet ( characterSet, argument ),
                CharacterTerminal characterTerminal => this.VisitCharacterTerminal ( characterTerminal, argument ),
                Lazy lazy => this.VisitLazy ( lazy, argument ),
                Lookahead lookahead => this.VisitLookahead ( lookahead, argument ),
                NamedBackreference namedBackreference => this.VisitNamedBackreference ( namedBackreference, argument ),
                NamedCapture namedCapture => this.VisitNamedCapture ( namedCapture, argument ),
                NegatedAlternation negatedAlternation => this.VisitNegatedAlternation ( negatedAlternation, argument ),
                NegatedCharacterRange negatedCharacterRange => this.VisitNegatedCharacterRange ( negatedCharacterRange, argument ),
                NegatedCharacterSet negatedCharacterSet => this.VisitNegatedCharacterSet ( negatedCharacterSet, argument ),
                NegatedCharacterTerminal negatedCharacterTerminal => this.VisitNegatedCharacterTerminal ( negatedCharacterTerminal, argument ),
                NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal => this.VisitNegatedUnicodeCategoryTerminal ( negatedUnicodeCategoryTerminal, argument ),
                NegativeLookahead negativeLookahead => this.VisitNegativeLookahead ( negativeLookahead, argument ),
                NumberedBackreference numberedBackreference => this.VisitNumberedBackreference ( numberedBackreference, argument ),
                NumberedCapture numberedCapture => this.VisitNumberedCapture ( numberedCapture, argument ),
                StringTerminal characterTerminalString => this.VisitStringTerminal ( characterTerminalString, argument ),
                UnicodeCategoryTerminal unicodeCategoryTerminal => this.VisitUnicodeCategoryTerminal ( unicodeCategoryTerminal, argument ),
                _ => throw new NotSupportedException ( $"Node of type {grammarNode.GetType ( ).Name} is not supported." ),
            };
        }
    }
}