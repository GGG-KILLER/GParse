using System;
using System.Collections.Generic;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Dbug
{
    public static class MatcherDebug
    {
        public static ILogger Logger = new DummyLogger ( );

        public static String GetMatcherName ( BaseMatcher matcher )
        {
            if ( matcher is AllMatcher allMatcher )
            {
                return $"AllMatcher";
            }
            else if ( matcher is AnyMatcher anyMatcher )
            {
                return $"AnyMatcher";
            }
            else if ( matcher is CharMatcher charMatcher )
            {
                return $"CharMatcher<{charMatcher.Filter}>";
            }
            else if ( matcher is FilterFuncMatcher funcMatcher )
            {
                return $"FilterFuncMatcher<{funcMatcher.Filter}>";
            }
            else if ( matcher is MultiCharMatcher multiCharMatcher )
            {
                return $"MultiCharMatcher<{String.Join ( ", ", multiCharMatcher.Whitelist )}>";
            }
            else if ( matcher is NegatedMatcher negatedMatcher )
            {
                return $"NegatedMatcher";
            }
            else if ( matcher is OptionalMatcher optionalMatcher )
            {
                return $"OptionalMatcher";
            }
            else if ( matcher is RepeatedMatcher repeatedMatcher )
            {
                return $"RepeatedMatcher<{repeatedMatcher.Minimum}, {repeatedMatcher.Maximum}>";
            }
            else if ( matcher is RuleWrapper ruleWrapper )
            {
                return $"RuleWrapper<{ruleWrapper.Name}>";
            }
            else if ( matcher is StringMatcher stringMatcher )
            {
                return $"StringMatcher<{stringMatcher.StringFilter}>";
            }
            else if ( matcher is CharRangeMatcher charRangeMatcher )
            {
                return $"CharRangeMatcher<{charRangeMatcher.Start}({( Int32 ) charRangeMatcher.Start:X2}) ~ {charRangeMatcher.End}({( Int32 ) charRangeMatcher.End:X2})>";
            }
            else if ( matcher is DebugMatcher debugMatcher )
            {
                return $"DebugMatcher";
            }
            else if ( matcher is RulePlaceholder placeholder )
            {
                return $"RulePlaceholder<{placeholder.Name}>";
            }
            else
            {
                return null;
            }
        }

        public static void PrintMatcherTree ( BaseMatcher matcher, List<String> printed = null )
        {
            printed = printed ?? new List<String> ( );
            if ( matcher is AllMatcher allMatcher )
            {
                Logger.Indent ( nameof ( AllMatcher ) );
                foreach ( BaseMatcher subMatcher in allMatcher.PatternMatchers )
                    PrintMatcherTree ( subMatcher, printed );
                Logger.Outdent ( );
            }
            else if ( matcher is AnyMatcher anyMatcher )
            {
                Logger.Indent ( nameof ( AnyMatcher ) );
                foreach ( BaseMatcher subMatcher in anyMatcher.PatternMatchers )
                    PrintMatcherTree ( subMatcher, printed );
                Logger.Outdent ( );
            }
            else if ( matcher is CharMatcher charMatcher )
            {
                Logger.WriteLine ( $"CharMatcher<{charMatcher.Filter}>" );
            }
            else if ( matcher is FilterFuncMatcher funcMatcher )
            {
                Logger.WriteLine ( $"FilterFuncMatcher<{funcMatcher.Filter}>" );
            }
            else if ( matcher is MultiCharMatcher multiCharMatcher )
            {
                Logger.WriteLine ( $"MultiCharMatcher<{String.Join ( ", ", multiCharMatcher.Whitelist )}>" );
            }
            else if ( matcher is NegatedMatcher negatedMatcher )
            {
                Logger.Indent ( nameof ( NegatedMatcher ) );
                PrintMatcherTree ( negatedMatcher.PatternMatcher, printed );
                Logger.Outdent ( );
            }
            else if ( matcher is OptionalMatcher optionalMatcher )
            {
                Logger.Indent ( nameof ( OptionalMatcher ) );
                PrintMatcherTree ( optionalMatcher.PatternMatcher, printed );
                Logger.Outdent ( );
            }
            else if ( matcher is RepeatedMatcher repeatedMatcher )
            {
                Logger.Indent ( $"RepeatedMatcher<{repeatedMatcher.Minimum}, {repeatedMatcher.Maximum}>" );
                PrintMatcherTree ( repeatedMatcher.PatternMatcher, printed );
                Logger.Outdent ( );
            }
            else if ( matcher is RuleWrapper ruleWrapper )
            {
                Logger.Indent ( $"RuleWrapper<{ruleWrapper.Name}>" );
                PrintMatcherTree ( ruleWrapper.PatternMatcher, printed );
                Logger.Outdent ( );
            }
            else if ( matcher is StringMatcher stringMatcher )
            {
                Logger.WriteLine ( $"StringMatcher<{stringMatcher.StringFilter}>" );
            }
            else if ( matcher is CharRangeMatcher charRangeMatcher )
            {
                Logger.WriteLine ( $"CharRangeMatcher<{charRangeMatcher.Start}({( Int32 ) charRangeMatcher.Start:X2}) ~ {charRangeMatcher.End}({( Int32 ) charRangeMatcher.End:X2})>" );
            }
            else if ( matcher is DebugMatcher debugMatcher )
            {
                Logger.Indent ( nameof ( DebugMatcher ) );
                PrintMatcherTree ( debugMatcher.PatternMatcher, printed );
                Logger.Outdent ( );
            }
            else if ( matcher is RulePlaceholder placeholder )
            {
                if ( printed.Contains ( placeholder.Name ) )
                {
                    Logger.WriteLine ( $"RulePlaceholder<{placeholder.Name}>" );
                }
                else
                {
                    printed.Add ( placeholder.Name );
                    Logger.Indent ( $"RulePlaceholder<{placeholder.Name}>" );
                    PrintMatcherTree ( placeholder.Parser.RawRule ( placeholder.Name ), printed );
                    Logger.Outdent ( );
                }
            }
        }

        public static BaseMatcher GetDebugTree ( BaseMatcher matcher )
        {
            return new DebugTreeCreator ( )
                .Visit ( matcher );
        }

        public static String GetEBNF ( BaseMatcher matcher )
        {
            return new EBNFReconstructor ( )
                .Visit ( matcher );
        }

        public static String GetExpression ( BaseMatcher matcher )
        {
            return new ExpressionReconstructor ( )
                .Visit ( matcher );
        }
    }
}
