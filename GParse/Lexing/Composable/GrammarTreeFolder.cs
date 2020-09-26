﻿using System;
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
        /// Folds an alternation node
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
            GrammarNode<Char>[] nodes = alternation.GrammarNodes.Select ( node => this.Visit ( alternation, argument ) )
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
        /// Folds a sequence node
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
        /// Folds a repetition node
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
            GrammarNode<Char>? innerNode = this.Visit ( repetition.InnerNode, argument );

            return innerNode is null || innerNode == repetition.InnerNode
                   ? repetition
                   : new Repetition<Char> ( innerNode, repetition.Range, repetition.IsLazy );
        }

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