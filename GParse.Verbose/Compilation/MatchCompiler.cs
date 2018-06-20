using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GParse.Common.IO;
using GParse.Verbose.Matchers;
using GParse.Verbose.Parsing;
using GParse.Verbose.Visitors;

namespace GParse.Verbose
{

    public delegate MatchResult CompiledRuleDelegate ( SourceCodeReader reader );

    public class MatchCompiler : MatcherTreeVisitor<Expression>
    {
        private readonly Dictionary<String, CompiledRuleDelegate> CompilationCache = new Dictionary<String, CompiledRuleDelegate> ( );
        // Parameters
        private readonly ParameterExpression Reader = Expression.Parameter ( typeof ( SourceCodeReader ), "reader" );
        // Body
        private readonly List<ParameterExpression> Locals = new List<ParameterExpression> ( );
        private readonly List<Expression> Body = new List<Expression> ( );
        private readonly Dictionary<String, LabelTarget> Labels = new Dictionary<String, LabelTarget> ( );
        // Returns
        private readonly ParameterExpression ReturnList = Expression.Variable ( typeof ( List<String> ), "return" );
        private readonly LabelTarget Return = Expression.Label ( typeof ( String[] ) );

        public override Expression Visit ( AllMatcher allMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( AnyMatcher anyMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( CharMatcher charMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( CharRangeMatcher charRangeMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( FilterFuncMatcher filterFuncMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( MultiCharMatcher multiCharMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( RulePlaceholder rulePlaceholder )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( StringMatcher stringMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( IgnoreMatcher ignoreMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( JoinMatcher joinMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( NegatedMatcher negatedMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( OptionalMatcher optionalMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( RepeatedMatcher repeatedMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( RuleWrapper ruleWrapper )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( MarkerMatcher markerMatcher )
        {
            throw new NotImplementedException ( );
        }

        public override Expression Visit ( EOFMatcher eofMatcher )
        {
            throw new NotImplementedException ( );
        }
    }
}
