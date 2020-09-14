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

    public abstract class GrammarTreeFolder : GrammarTreeVisitor<GrammarNode<Char>?>
    {
        /// <summary>
        /// Folds an alternation node
        /// </summary>
        /// <param name="alternation"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitAlternation ( Alternation<Char> alternation )
        {
            GrammarNode<Char>[] nodes = alternation.GrammarNodes.Select ( this.Visit )
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
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitSequence ( Sequence<Char> sequence )
        {
            GrammarNode<Char>[] nodes = sequence.GrammarNodes.Select ( this.Visit )
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
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected override GrammarNode<Char>? VisitRepetition ( Repetition<Char> repetition )
        {
            GrammarNode<Char>? innerNode = this.Visit ( repetition.InnerNode );

            return innerNode is null || innerNode == repetition.InnerNode
                   ? repetition
                   : new Repetition<Char> ( innerNode, repetition.Range );
        }

        /// <summary>
        /// Folds a negated character node
        /// </summary>
        /// <param name="negatedCharacterTerminal"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract override GrammarNode<Char>? VisitNegatedCharacterTerminal ( NegatedCharacterTerminal negatedCharacterTerminal );

        /// <summary>
        /// Folds a character terminal
        /// </summary>
        /// <param name="characterTerminal"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract override GrammarNode<Char>? VisitCharacterTerminal ( CharacterTerminal characterTerminal );

        /// <summary>
        /// Folds a negated character range
        /// </summary>
        /// <param name="negatedCharacterRange"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract override GrammarNode<Char>? VisitNegatedCharacterRange ( NegatedCharacterRange negatedCharacterRange );

        /// <summary>
        /// Folds a character range
        /// </summary>
        /// <param name="characterRange"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract override GrammarNode<Char>? VisitCharacterRange ( CharacterRange characterRange );

        /// <summary>
        /// Folds a character terminal string
        /// </summary>
        /// <param name="characterTerminalString"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract override GrammarNode<Char>? VisitStringTerminal ( StringTerminal characterTerminalString );

        /// <summary>
        /// Folds a named capture
        /// </summary>
        /// <param name="namedCapture"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        protected abstract override GrammarNode<Char>? VisitNamedCapture ( NamedCapture namedCapture );

        /// <summary>
        /// Folds a grammar node
        /// </summary>
        /// <param name="grammarNode"></param>
        /// <returns>
        /// <list type="number">
        /// <item>The original node if it's to be kept</item>
        /// <item>A different node to replace the original node with</item>
        /// <item>Null if the node is to be removed</item>
        /// </list>
        /// </returns>
        public override GrammarNode<Char>? Visit ( GrammarNode<Char> grammarNode ) =>
            base.Visit ( grammarNode );
    }
}