using System;
using System.Linq;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error
    /// <summary>
    /// The base class for a <see cref="GrammarNode{System.Char}"/> tree folder
    /// </summary>
#pragma warning restore CS1658 // Warning is overriding an error
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute

    public abstract class GrammarTreeFolder<TArgument> : GrammarTreeVisitor<GrammarNode<Char>?, TArgument>
    {
        /// <summary>
        /// Folds a character range node.
        /// </summary>
        /// <param name="characterRange"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitCharacterRange ( CharacterRange characterRange, TArgument argument ) => characterRange;

        /// <summary>
        /// Folds a character terminal node.
        /// </summary>
        /// <param name="characterTerminal"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitCharacterTerminal ( CharacterTerminal characterTerminal, TArgument argument ) => characterTerminal;

        /// <summary>
        /// Folds a set node.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitSet ( Set set, TArgument argument ) => set;

        /// <summary>
        /// Folds a named backreference node.
        /// </summary>
        /// <param name="namedBackreference"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitNamedBackreference ( NamedBackreference namedBackreference, TArgument argument ) => namedBackreference;

        /// <summary>
        /// Folds a negated character range node.
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange, TArgument argument ) => negatedCharacterRange;

        /// <summary>
        /// Folds a negated character terminal node.
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal, TArgument argument ) => negatedCharacterTerminal;

        /// <summary>
        /// Folds a negated set node.
        /// </summary>
        /// <param name="negatedSet"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitNegatedSet ( NegatedSet negatedSet, TArgument argument ) => negatedSet;

        /// <summary>
        /// Folds a negated unicode category terminal node.
        /// </summary>
        /// <param name="negatedUnicodeCategoryTerminal"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitNegatedUnicodeCategoryTerminal ( NegatedUnicodeCategoryTerminal negatedUnicodeCategoryTerminal, TArgument argument ) => negatedUnicodeCategoryTerminal;

        /// <summary>
        /// Folds a numbered backreference node.
        /// </summary>
        /// <param name="numberedBackreference"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitNumberedBackreference ( NumberedBackreference numberedBackreference, TArgument argument ) => numberedBackreference;

        /// <summary>
        /// Folds a string terminal node.
        /// </summary>
        /// <param name="characterTerminalString"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitStringTerminal ( StringTerminal characterTerminalString, TArgument argument ) => characterTerminalString;

        /// <summary>
        /// Folds a unicode category terminal node.
        /// </summary>
        /// <param name="unicodeCategoryTerminal"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitUnicodeCategoryTerminal ( UnicodeCategoryTerminal unicodeCategoryTerminal, TArgument argument ) => unicodeCategoryTerminal;

        /// <summary>
        /// Folds an any node.
        /// </summary>
        /// <param name="any"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitAny ( Any any, TArgument argument ) => any;

        /// <summary>
        /// Folds an alternation node.
        /// </summary>
        /// <param name="alternation"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitAlternation ( Alternation<Char> alternation, TArgument argument )
        {
            if ( alternation is null )
                throw new ArgumentNullException ( nameof ( alternation ) );

            GrammarNode<Char>[] nodes = alternation.GrammarNodes.Select ( node => this.Visit ( node, argument ) )
                                                                .Where ( node => node != null )
                                                                .Select ( node => node! )
                                                                .ToArray ( );
            if ( nodes.Length == 0 )
                return null;
            else if ( nodes.Length == 1 )
                return nodes[0];
            else if ( nodes.SequenceEqual ( alternation.GrammarNodes ) )
                return alternation;
            else
                return new Alternation<Char> ( nodes );
        }

        /// <summary>
        /// Folds a sequence node.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitSequence ( Sequence<Char> sequence, TArgument argument )
        {
            if ( sequence is null )
                throw new ArgumentNullException ( nameof ( sequence ) );

            GrammarNode<Char>[] nodes = sequence.GrammarNodes.Select ( node => this.Visit ( node, argument ) )
                                                             .Where ( node => node != null )
                                                             .Select ( node => node! )
                                                             .ToArray ( );

            if ( nodes.Length == 0 )
                return null;
            else if ( nodes.Length == 1 )
                return nodes[0];
            else if ( nodes.SequenceEqual ( sequence.GrammarNodes ) )
                return sequence;
            else
                return new Sequence<Char> ( nodes );
        }

        /// <summary>
        /// Folds a repetition node.
        /// </summary>
        /// <param name="repetition"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitRepetition ( Repetition<Char> repetition, TArgument argument )
        {
            if ( repetition is null )
                throw new ArgumentNullException ( nameof ( repetition ) );

            GrammarNode<Char>? innerNode = this.Visit ( repetition.InnerNode, argument );

            if ( innerNode is null )
                return null;
            else if ( innerNode == repetition.InnerNode )
                return repetition;
            else
                return new Repetition<Char> ( innerNode, repetition.Range, repetition.IsLazy );
        }

        /// <summary>
        /// Folds a lookahead node.
        /// </summary>
        /// <param name="lookahead"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitLookahead ( Lookahead lookahead, TArgument argument )
        {
            if ( lookahead is null )
                throw new ArgumentNullException ( nameof ( lookahead ) );

            GrammarNode<Char>? innerNode = this.Visit ( lookahead.InnerNode, argument );

            if ( innerNode is null )
                return null;
            else if ( innerNode == lookahead.InnerNode )
                return lookahead;
            else
                return new Lookahead ( innerNode );
        }

        /// <summary>
        /// Folds a named capture node.
        /// </summary>
        /// <param name="namedCapture"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitNamedCapture ( NamedCapture namedCapture, TArgument argument )
        {
            if ( namedCapture is null )
                throw new ArgumentNullException ( nameof ( namedCapture ) );

            GrammarNode<Char>? innerNode = this.Visit ( namedCapture.InnerNode, argument );

            if ( innerNode is null )
                return null;
            else if ( innerNode == namedCapture.InnerNode )
                return namedCapture;
            else
                return new NamedCapture ( namedCapture.Name, innerNode );
        }

        /// <summary>
        /// Folds a negative lookahead node.
        /// </summary>
        /// <param name="negativeLookahead"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitNegativeLookahead ( NegativeLookahead negativeLookahead, TArgument argument )
        {
            if ( negativeLookahead is null )
                throw new ArgumentNullException ( nameof ( negativeLookahead ) );

            GrammarNode<Char>? innerNode = this.Visit ( negativeLookahead.InnerNode, argument );

            if ( innerNode is null )
                return null;
            else if ( innerNode == negativeLookahead.InnerNode )
                return negativeLookahead;
            else
                return new NegativeLookahead ( innerNode );
        }

        /// <summary>
        /// Folds a numbered capture node.
        /// </summary>
        /// <param name="numberedCapture"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitNumberedCapture ( NumberedCapture numberedCapture, TArgument argument )
        {
            if ( numberedCapture is null )
                throw new ArgumentNullException ( nameof ( numberedCapture ) );

            GrammarNode<Char>? innerNode = this.Visit ( numberedCapture.InnerNode, argument );

            if ( innerNode is null )
                return null;
            else if ( innerNode == numberedCapture.InnerNode )
                return numberedCapture;
            else
                return new NumberedCapture ( numberedCapture.Position, innerNode );
        }

        /// <summary>
        /// Folds an optimized set.
        /// </summary>
        /// <param name="optimizedSet"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char> VisitOptimizedSet ( OptimizedSet optimizedSet, TArgument argument ) => optimizedSet;

        /// <summary>
        /// Folds an optimized negated set.
        /// </summary>
        /// <param name="optimizedNegatedSet"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char> VisitOptimizedNegatedSet ( OptimizedNegatedSet optimizedNegatedSet, TArgument argument ) => optimizedNegatedSet;

        /// <summary>
        /// Folds a grammar node
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <param name="argument">The argument to be passed to the visitor method.</param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        public override GrammarNode<Char>? Visit ( GrammarNode<Char> grammarNode, TArgument argument ) =>
            base.Visit ( grammarNode, argument );
    }
}