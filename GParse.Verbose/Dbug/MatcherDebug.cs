using System;
using System.Collections.Generic;
using System.Linq;
using GParse.Verbose.Matchers;

namespace GParse.Verbose.Dbug
{
    public static class MatcherDebug
    {
        public static Action<Object> LogLine
        {
            set => DebugMatcher.LogLine = value;
            get => DebugMatcher.LogLine;
        }

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
            else if ( matcher is RulePlaceholder placeholder )
            {
                return $"RulePlaceholder<{placeholder.Name}>";
            }
            else
            {
                return null;
            }
        }

        public static void PrintMatcherTree ( BaseMatcher matcher, Int32 depth = 0, List<String> printed = null )
        {
            var indentation = new String ( ' ', depth * 4 );
            printed = printed ?? new List<String> ( );
            if ( matcher is AllMatcher allMatcher )
            {
                LogLine?.Invoke ( $"{indentation}AllMatcher = {{" );
                foreach ( BaseMatcher subMatcher in allMatcher.PatternMatchers )
                    PrintMatcherTree ( subMatcher, depth + 1, printed );
                LogLine?.Invoke ( $"{indentation}}}" );
            }
            else if ( matcher is AnyMatcher anyMatcher )
            {
                LogLine?.Invoke ( $"{indentation}AnyMatcher = {{" );
                foreach ( BaseMatcher subMatcher in anyMatcher.PatternMatchers )
                    PrintMatcherTree ( subMatcher, depth + 1, printed );
                LogLine?.Invoke ( $"{indentation}}}" );
            }
            else if ( matcher is CharMatcher charMatcher )
            {
                LogLine?.Invoke ( $"{indentation}CharMatcher<{charMatcher.Filter}>" );
            }
            else if ( matcher is FilterFuncMatcher funcMatcher )
            {
                LogLine?.Invoke ( $"{indentation}FilterFuncMatcher<{funcMatcher.Filter}>" );
            }
            else if ( matcher is InfiniteMatcher infiniteMatcher )
            {
                LogLine?.Invoke ( $"{indentation}InfiniteMatcher = {{" );
                PrintMatcherTree ( infiniteMatcher.PatternMatcher, depth + 1, printed );
                LogLine?.Invoke ( $"{indentation}}}" );
            }
            else if ( matcher is MultiCharMatcher multiCharMatcher )
            {
                LogLine?.Invoke ( $"{indentation}MultiCharMatcher<{String.Join ( ", ", multiCharMatcher.Whitelist )}>" );
            }
            else if ( matcher is NegatedMatcher negatedMatcher )
            {
                LogLine?.Invoke ( $"{indentation}NegatedMatcher = {{" );
                PrintMatcherTree ( negatedMatcher.PatternMatcher, depth + 1, printed );
                LogLine?.Invoke ( $"{indentation}}}" );
            }
            else if ( matcher is OptionalMatcher optionalMatcher )
            {
                LogLine?.Invoke ( $"{indentation}OptionalMatcher = {{" );
                PrintMatcherTree ( optionalMatcher.PatternMatcher, depth + 1, printed );
                LogLine?.Invoke ( $"{indentation}}}" );
            }
            else if ( matcher is RepeatedMatcher repeatedMatcher )
            {
                LogLine?.Invoke ( $"{indentation}RepeatedMatcher<{repeatedMatcher.Limit}> = {{" );
                PrintMatcherTree ( repeatedMatcher.PatternMatcher, depth + 1, printed );
                LogLine?.Invoke ( $"{indentation}}}" );
            }
            else if ( matcher is RuleWrapper ruleWrapper )
            {
                LogLine?.Invoke ( $"{indentation}RuleWrapper<{ruleWrapper.Name}> = {{" );
                PrintMatcherTree ( ruleWrapper.PatternMatcher, depth + 1, printed );
                LogLine?.Invoke ( $"{indentation}}}" );
            }
            else if ( matcher is StringMatcher stringMatcher )
            {
                LogLine?.Invoke ( $"{indentation}StringMatcher<{stringMatcher.Filter}>" );
            }
            else if ( matcher is CharRangeMatcher charRangeMatcher )
            {
                LogLine?.Invoke ( $"{indentation}CharRangeMatcher<{charRangeMatcher.Start}({( Int32 ) charRangeMatcher.Start:X2}) ~ {charRangeMatcher.End}({( Int32 ) charRangeMatcher.End:X2})>" );
            }
            else if ( matcher is DebugMatcher debugMatcher )
            {
                LogLine?.Invoke ( $"{indentation}DebugMatcher ={{" );
                PrintMatcherTree ( debugMatcher.PatternMatcher, depth + 1, printed );
                LogLine?.Invoke ( $"{indentation}}}" );
            }
            else if ( matcher is RulePlaceholder placeholder )
            {
                if ( printed.Contains ( placeholder.Name ) )
                {
                    LogLine?.Invoke ( $"{indentation}RulePlaceholder<{placeholder.Name}>" );
                }
                else
                {
                    printed.Add ( placeholder.Name );
                    LogLine?.Invoke ( $"{indentation}RulePlaceholder<{placeholder.Name}> = {{" );
                    PrintMatcherTree ( placeholder.Parser.RawRule ( placeholder.Name ), depth + 1, printed );
                    LogLine?.Invoke ( $"{indentation}}}" );
                }
            }
        }

        public static BaseMatcher GetDebug ( BaseMatcher matcher )
        {
            if ( matcher is AllMatcher allMatcher )
            {
                BaseMatcher[] matchers = allMatcher.PatternMatchers;
                for ( var i = 0; i < matchers.Length; i++ )
                    matchers[i] = GetDebug ( matchers[i] );
                return new DebugMatcher ( matcher );
            }
            else if ( matcher is AnyMatcher anyMatcher )
            {
                BaseMatcher[] matchers = anyMatcher.PatternMatchers;
                for ( var i = 0; i < matchers.Length; i++ )
                    matchers[i] = GetDebug ( matchers[i] );
                return new DebugMatcher ( matcher );
            }
            else if ( matcher is InfiniteMatcher infiniteMatcher )
            {
                return new DebugMatcher ( new InfiniteMatcher ( GetDebug ( infiniteMatcher.PatternMatcher ) ) );
            }
            else if ( matcher is NegatedMatcher negatedMatcher )
            {
                return new DebugMatcher ( new NegatedMatcher ( GetDebug ( negatedMatcher.PatternMatcher ) ) );
            }
            else if ( matcher is OptionalMatcher optionalMatcher )
            {
                return new DebugMatcher ( new OptionalMatcher ( GetDebug ( optionalMatcher.PatternMatcher ) ) );
            }
            else if ( matcher is RepeatedMatcher repeatedMatcher )
            {
                return new DebugMatcher ( new RepeatedMatcher ( GetDebug ( repeatedMatcher.PatternMatcher ), repeatedMatcher.Limit ) );
            }
            else if ( matcher is RuleWrapper ruleWrapper )
            {
                return new DebugMatcher ( new RuleWrapper (
                    GetDebug ( ruleWrapper.PatternMatcher ),
                    ruleWrapper.Name,
                    ruleWrapper.RuleEnter,
                    ruleWrapper.RuleMatched,
                    ruleWrapper.RuleExit
                ) );
            }
            else if ( matcher is DebugMatcher debugMatcher )
            {
                return matcher;
            }
            else
            {
                return new DebugMatcher ( matcher );
            }
        }

        public static String GetRule ( BaseMatcher matcher )
        {
            if ( matcher is AllMatcher allMatcher )
            {
                return $"({String.Join ( ", ", allMatcher.PatternMatchers.Select ( m => GetRule ( m ) ) )})";
            }
            else if ( matcher is AnyMatcher anyMatcher )
            {
                return $"({String.Join ( " | ", anyMatcher.PatternMatchers.Select ( m => GetRule ( m ) ) )})";
            }
            else if ( matcher is CharMatcher charMatcher )
            {
                return $"'{charMatcher.Filter}'";
            }
            else if ( matcher is FilterFuncMatcher funcMatcher )
            {
                return $"? {funcMatcher.Filter.Target?.GetType ( )?.FullName + "." ?? ""}{funcMatcher.Filter.Method.Name} ?";
            }
            else if ( matcher is InfiniteMatcher infiniteMatcher )
            {
                return $"{{ {GetRule ( infiniteMatcher.PatternMatcher )} }}";
            }
            else if ( matcher is MultiCharMatcher multiCharMatcher )
            {
                return $"( '{String.Join ( "' | '", multiCharMatcher.Whitelist )}' )";
            }
            else if ( matcher is NegatedMatcher negatedMatcher )
            {
                return $"-{GetRule ( negatedMatcher.PatternMatcher )}";
            }
            else if ( matcher is OptionalMatcher optionalMatcher )
            {
                return $"[{GetRule ( optionalMatcher.PatternMatcher )}]";
            }
            else if ( matcher is RepeatedMatcher repeatedMatcher )
            {
                return ( $"{GetRule ( repeatedMatcher.PatternMatcher )} * {repeatedMatcher.Limit}" );
            }
            else if ( matcher is RuleWrapper ruleWrapper )
            {
                return $"{ruleWrapper.Name} = {GetRule ( ruleWrapper.PatternMatcher )};";
            }
            else if ( matcher is StringMatcher stringMatcher )
            {
                return ( $"'{stringMatcher.Filter}'" );
            }
            else if ( matcher is CharRangeMatcher charRangeMatcher )
            {
                var start = charRangeMatcher.Strict ? charRangeMatcher.Start : charRangeMatcher.Start + 1;
                var end = charRangeMatcher.Strict ? charRangeMatcher.End : charRangeMatcher.End - 1;
                return ( $"? interval {( charRangeMatcher.Strict ? '(' : '[' )}0x{start:X2}, 0x{end:X2}{( charRangeMatcher.Strict ? ')' : ']' )} ?" );
            }
            else if ( matcher is DebugMatcher debugMatcher )
            {
                return GetRule ( debugMatcher.PatternMatcher );
            }
            else if ( matcher is RulePlaceholder placeholder )
            {
                return placeholder.Name;
            }
            else
            {
                return null;
            }
        }
    }
}
