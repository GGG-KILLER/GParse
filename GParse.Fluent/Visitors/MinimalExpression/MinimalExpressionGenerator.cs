using System;
using System.Collections.Generic;
using GParse.Fluent.Abstractions;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Visitors.MinimalExpression
{
    public class MinimalExpressionsGenerator : IMatcherTreeVisitor<MinimalExpressionNode>
    {
        private readonly FluentParser Parser;
        private readonly Boolean Negated;

        public MinimalExpressionsGenerator ( FluentParser parser, Boolean negated = false )
        {
            this.Parser = parser;
            this.Negated = negated;
        }

        public MinimalExpressionNode Visit ( SequentialMatcher sequentialMatcher )
        {
            var root = sequentialMatcher.PatternMatchers[0].Accept ( this );
            for ( var i = 1; i < sequentialMatcher.PatternMatchers.Length; i++ )
            {
                var child = sequentialMatcher.PatternMatchers[i].Accept ( this );
                root.AddChild ( child );
                root = child;
            }
            return root;
        }

        public MinimalExpressionNode Visit ( AlternatedMatcher alternatedMatcher )
        {
            var node = MinimalExpressionNode.CreateNew ( );
            for ( var i = 0; i < alternatedMatcher.PatternMatchers.Length; i++ )
                node.AddChild ( alternatedMatcher.PatternMatchers[i].Accept ( this ) );
            return node;
        }

        public MinimalExpressionNode Visit ( CharMatcher charMatcher )
        {
            return new MinimalExpressionNode ( charMatcher.StringFilter );
        }

        public MinimalExpressionNode Visit ( RangeMatcher RangeMatcher )
        {
            var range = RangeMatcher.Range;
            if ( this.Negated )
            {
                if ( range.Start > Char.MinValue )
                    return new MinimalExpressionNode ( ( ( Char ) ( range.Start - 1 ) ).ToString ( ) );
                else if ( range.End < Char.MaxValue )
                    return new MinimalExpressionNode ( ( ( Char ) ( range.End + 1 ) ).ToString ( ) );
                else
                    throw new InvalidOperationException ( "Cannot find a character outside of this range." );
            }
            else
                return new MinimalExpressionNode ( ( ( Char ) range.Start ).ToString ( ) );
        }

        public MinimalExpressionNode Visit ( EOFMatcher eofMatcher )
        {
            return this.Negated
                ? new MinimalExpressionNode ( "a" )
                // Just needs to be a leaf (it'll be a pain to
                // check if it is though, so we don't)
                : MinimalExpressionNode.CreateNew ( );
        }

        public MinimalExpressionNode Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            var strs = new List<String> ( );
            for ( var i = Char.MinValue; i <= Char.MaxValue; i++ )
                if ( this.Negated ^ filterFuncMatcher.Filter ( i ) )
                    return new MinimalExpressionNode ( i.ToString ( ) );

            throw new InvalidOperationException ( $"Cannot get a minimal expression for a predicate which {( this.Negated ? "" : "doesn't " )}accepts any chars." );
        }

        public MinimalExpressionNode Visit ( IgnoreMatcher ignoreMatcher )
        {
            return ignoreMatcher.PatternMatcher.Accept ( this );
        }

        public MinimalExpressionNode Visit ( JoinMatcher joinMatcher )
        {
            return joinMatcher.PatternMatcher.Accept ( this );
        }

        public MinimalExpressionNode Visit ( MarkerMatcher markerMatcher )
        {
            return markerMatcher.PatternMatcher.Accept ( this );
        }

        public MinimalExpressionNode Visit ( CharListMatcher charListMatcher )
        {
            if ( this.Negated )
            {
                for ( var i = Char.MinValue; i <= Char.MaxValue; i++ )
                    if ( Array.IndexOf ( charListMatcher.Whitelist, i ) == -1 )
                        return new MinimalExpressionNode ( i.ToString ( ) );
            }
            return new MinimalExpressionNode ( charListMatcher.Whitelist[0].ToString ( ) );
        }

        public MinimalExpressionNode Visit ( NegatedMatcher negatedMatcher )
        {
            return negatedMatcher.PatternMatcher.Accept ( new MinimalExpressionsGenerator ( this.Parser, !this.Negated ) );
        }

        public MinimalExpressionNode Visit ( OptionalMatcher optionalMatcher )
        {
            if ( this.Negated )
                throw new InvalidOperationException ( "Cannot negate an optional matcher." );

            return new MinimalExpressionNode (
                // Matches optional
                optionalMatcher.PatternMatcher.Accept ( this ),
                // Doesn't matches optional
                MinimalExpressionNode.CreateNew ( ) );
        }

        public MinimalExpressionNode Visit ( RepeatedMatcher repeatedMatcher )
        {
            // If we can, don't match anything.
            if ( repeatedMatcher.Range.Start == 0 )
                return MinimalExpressionNode.CreateNew ( );

            var flat = repeatedMatcher.PatternMatcher.Accept ( this ).Flatten ( );
            var str = flat[0];
            var node = new MinimalExpressionNode ( str );
            for ( var i = 1; i < repeatedMatcher.Range.Start; i++ )
            {
                var subNode = new MinimalExpressionNode ( str );
                node.AddChild ( subNode );
                node = subNode;
            }
            return node;
        }

        public MinimalExpressionNode Visit ( RulePlaceholder rulePlaceholder )
        {
            return this.Parser.RawRule ( rulePlaceholder.Name ).Accept ( this );
        }

        public MinimalExpressionNode Visit ( RuleWrapper ruleWrapper )
        {
            return ruleWrapper.PatternMatcher.Accept ( this );
        }

        public MinimalExpressionNode Visit ( StringMatcher stringMatcher )
        {
            return new MinimalExpressionNode ( this.Negated
                ? ( ( Char ) ( stringMatcher.StringFilter[0] + 1 ) ) + stringMatcher.StringFilter.Substring ( 1 )
                : stringMatcher.StringFilter );
        }

        public MinimalExpressionNode Visit ( SavingMatcher savingMatcher )
        {
            throw new NotSupportedException ( );
        }

        public MinimalExpressionNode Visit ( LoadingMatcher loadingMatcher )
        {
            throw new NotSupportedException ( );
        }
    }
}
