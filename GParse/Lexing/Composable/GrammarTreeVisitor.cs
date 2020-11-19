using System;
using System.Diagnostics.CodeAnalysis;
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
        /// Visits a character terminal.
        /// </summary>
        /// <param name="characterTerminal"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitCharacterTerminal ( CharacterTerminal characterTerminal, TArgument argument );


        /// <summary>
        /// Visits a set.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        [SuppressMessage ( "Naming", "CA1716:Identifiers should not match keywords", Justification = "It's the class name." )]
        protected abstract TReturn VisitSet ( Set set, TArgument argument );

        /// <summary>
        /// Visits a lookahead.
        /// </summary>
        /// <param name="positiveLookahead"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitPositiveLookahead ( PositiveLookahead positiveLookahead, TArgument argument );

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
        /// Visits a negated character range.
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange, TArgument argument );

        /// <summary>
        /// Visits a negated character node.
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal, TArgument argument );

        /// <summary>
        /// Visits a negated set node.
        /// </summary>
        /// <param name="negatedSet"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitNegatedSet ( NegatedSet negatedSet, TArgument argument );

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
        /// Visits an optimized set.
        /// </summary>
        /// <param name="optimizedSet"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitOptimizedSet ( OptimizedSet optimizedSet, TArgument argument );

        /// <summary>
        /// Visits an optimized negated set.
        /// </summary>
        /// <param name="optimizedNegatedSet"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitOptimizedNegatedSet ( OptimizedNegatedSet optimizedNegatedSet, TArgument argument );

        /// <summary>
        /// Visits an any node.
        /// </summary>
        /// <param name="any"></param>
        /// <param name="argument">The argument data passed by the caller.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract TReturn VisitAny ( Any any, TArgument argument );

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
                Repetition<Char> repetition => this.VisitRepetition ( repetition, argument ),
                Sequence<Char> sequence => this.VisitSequence ( sequence, argument ),
                CharacterRange characterRange => this.VisitCharacterRange ( characterRange, argument ),
                Set set => this.VisitSet ( set, argument ),
                CharacterTerminal characterTerminal => this.VisitCharacterTerminal ( characterTerminal, argument ),
                PositiveLookahead positiveLookahead => this.VisitPositiveLookahead ( positiveLookahead, argument ),
                NamedBackreference namedBackreference => this.VisitNamedBackreference ( namedBackreference, argument ),
                NamedCapture namedCapture => this.VisitNamedCapture ( namedCapture, argument ),
                NegatedCharacterRange negatedCharacterRange => this.VisitNegatedCharacterRange ( negatedCharacterRange, argument ),
                NegatedSet negatedSet => this.VisitNegatedSet ( negatedSet, argument ),
                NegatedCharacterTerminal negatedCharacterTerminal => this.VisitNegatedCharacterTerminal ( negatedCharacterTerminal, argument ),
                NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal => this.VisitNegatedUnicodeCategoryTerminal ( negatedUnicodeCategoryTerminal, argument ),
                NegativeLookahead negativeLookahead => this.VisitNegativeLookahead ( negativeLookahead, argument ),
                NumberedBackreference numberedBackreference => this.VisitNumberedBackreference ( numberedBackreference, argument ),
                NumberedCapture numberedCapture => this.VisitNumberedCapture ( numberedCapture, argument ),
                StringTerminal characterTerminalString => this.VisitStringTerminal ( characterTerminalString, argument ),
                UnicodeCategoryTerminal unicodeCategoryTerminal => this.VisitUnicodeCategoryTerminal ( unicodeCategoryTerminal, argument ),
                OptimizedSet optimizedSet => this.VisitOptimizedSet ( optimizedSet, argument ),
                OptimizedNegatedSet optimizedNegatedSet => this.VisitOptimizedNegatedSet ( optimizedNegatedSet, argument ),
                Any any => this.VisitAny ( any, argument ),
                _ => throw new NotSupportedException ( $"Node of type {grammarNode.GetType ( ).Name} is not supported." ),
            };
        }
    }
}