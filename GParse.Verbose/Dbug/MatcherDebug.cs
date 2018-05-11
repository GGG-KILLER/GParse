using System;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Dbug
{
    public static class MatcherDebug
    {
        public static String GetMatcher ( BaseMatcher matcher )
        {
            if ( matcher is AllMatcher allMatcher )
            {
                return ( $"AllMatcher" );
            }
            else if ( matcher is AnyMatcher anyMatcher )
            {
                return ( $"AnyMatcher" );
            }
            else if ( matcher is CharMatcher charMatcher )
            {
                return ( $"CharMatcher<{charMatcher.Filter}>" );
            }
            else if ( matcher is FilterFuncMatcher funcMatcher )
            {
                return ( $"FilterFuncMatcher<{funcMatcher.Filter}>" );
            }
            else if ( matcher is InfiniteMatcher infiniteMatcher )
            {
                return ( $"InfiniteMatcher" );
            }
            else if ( matcher is MultiCharMatcher multiCharMatcher )
            {
                return ( $"MultiCharMatcher<{String.Join ( ", ", multiCharMatcher.Whitelist )}>" );
            }
            else if ( matcher is NegatedMatcher negatedMatcher )
            {
                return ( $"NegatedMatcher" );
            }
            else if ( matcher is OptionalMatcher optionalMatcher )
            {
                return ( $"OptionalMatcher" );
            }
            else if ( matcher is RepeatedMatcher repeatedMatcher )
            {
                return ( $"RepeatedMatcher<{repeatedMatcher.Limit}>" );
            }
            else if ( matcher is RuleWrapper ruleWrapper )
            {
                return ( $"RuleWrapper<{ruleWrapper.Name}>" );
            }
            else if ( matcher is StringMatcher stringMatcher )
            {
                return ( $"StringMatcher<{stringMatcher.Filter}>" );
            }
            else if ( matcher is CharRangeMatcher charRangeMatcher )
            {
                return ( $"CharRangeMatcher<{charRangeMatcher.Start}({( Int32 ) charRangeMatcher.Start:X2}) ~ {charRangeMatcher.End}({( Int32 ) charRangeMatcher.End:X2})>" );
            }
            else if ( matcher is DebugMatcher debugMatcher )
            {
                return ( $"DebugMatcher" );
            }
            else
            {
                return null;
            }
        }

        public static void PrintMatcherTree ( BaseMatcher matcher, Int32 depth = 0 )
        {
            var indentation = new String ( ' ', depth * 4 );
            if ( matcher is AllMatcher allMatcher )
            {
                Console.WriteLine ( $"{indentation}AllMatcher = {{" );
                foreach ( BaseMatcher subMatcher in allMatcher.PatternMatchers )
                    PrintMatcherTree ( subMatcher, depth + 1 );
                Console.WriteLine ( $"{indentation}}}" );
            }
            else if ( matcher is AnyMatcher anyMatcher )
            {
                Console.WriteLine ( $"{indentation}AnyMatcher = {{" );
                foreach ( BaseMatcher subMatcher in anyMatcher.PatternMatchers )
                    PrintMatcherTree ( subMatcher, depth + 1 );
                Console.WriteLine ( $"{indentation}}}" );
            }
            else if ( matcher is CharMatcher charMatcher )
            {
                Console.WriteLine ( $"{indentation}CharMatcher<{charMatcher.Filter}>" );
            }
            else if ( matcher is FilterFuncMatcher funcMatcher )
            {
                Console.WriteLine ( $"{indentation}FilterFuncMatcher<{funcMatcher.Filter}>" );
            }
            else if ( matcher is InfiniteMatcher infiniteMatcher )
            {
                Console.WriteLine ( $"{indentation}InfiniteMatcher = {{" );
                PrintMatcherTree ( infiniteMatcher.PatternMatcher, depth + 1 );
                Console.WriteLine ( $"{indentation}}}" );
            }
            else if ( matcher is MultiCharMatcher multiCharMatcher )
            {
                Console.WriteLine ( $"{indentation}MultiCharMatcher<{String.Join ( ", ", multiCharMatcher.Whitelist )}>" );
            }
            else if ( matcher is NegatedMatcher negatedMatcher )
            {
                Console.WriteLine ( $"{indentation}NegatedMatcher = {{" );
                PrintMatcherTree ( negatedMatcher.PatternMatcher, depth + 1 );
                Console.WriteLine ( $"{indentation}}}" );
            }
            else if ( matcher is OptionalMatcher optionalMatcher )
            {
                Console.WriteLine ( $"{indentation}OptionalMatcher = {{" );
                PrintMatcherTree ( optionalMatcher.PatternMatcher, depth + 1 );
                Console.WriteLine ( $"{indentation}}}" );
            }
            else if ( matcher is RepeatedMatcher repeatedMatcher )
            {
                Console.WriteLine ( $"{indentation}RepeatedMatcher<{repeatedMatcher.Limit}> = {{" );
                PrintMatcherTree ( repeatedMatcher.PatternMatcher, depth + 1 );
                Console.WriteLine ( $"{indentation}}}" );
            }
            else if ( matcher is RuleWrapper ruleWrapper )
            {
                Console.WriteLine ( $"{indentation}RuleWrapper<{ruleWrapper.Name}> = {{" );
                PrintMatcherTree ( ruleWrapper.PatternMatcher, depth + 1 );
                Console.WriteLine ( $"{indentation}}}" );
            }
            else if ( matcher is StringMatcher stringMatcher )
            {
                Console.WriteLine ( $"{indentation}StringMatcher<{stringMatcher.Filter}>" );
            }
            else if ( matcher is CharRangeMatcher charRangeMatcher )
            {
                Console.WriteLine ( $"{indentation}CharRangeMatcher<{charRangeMatcher.Start}({( Int32 ) charRangeMatcher.Start:X2}) ~ {charRangeMatcher.End}({( Int32 ) charRangeMatcher.End:X2})>" );
            }
            else if ( matcher is DebugMatcher debugMatcher )
            {
                Console.WriteLine ( $"{indentation}DebugMatcher ={{" );
                PrintMatcherTree ( debugMatcher.PatternMatcher, depth + 1 );
                Console.WriteLine ( $"{indentation}}}" );
            }
        }

        public static BaseMatcher GetDebug ( BaseMatcher matcher ) => new DebugMatcher ( matcher );
    }
}
