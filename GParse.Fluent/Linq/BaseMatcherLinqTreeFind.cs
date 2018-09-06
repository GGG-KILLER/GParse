using System;
using System.Collections.Generic;
using GParse.Fluent.Matchers;

namespace GParse.Fluent.Linq
{
    public static class BaseMatcherLinqTreeFind
    {
        public static BaseMatcher FindInTree ( this BaseMatcher matcher, Predicate<BaseMatcher> predicate )
        {
            if ( predicate == null )
                throw new ArgumentNullException ( nameof ( predicate ) );

            if ( predicate ( matcher ) )
            {
                return matcher;
            }
            else if ( matcher is AlternatedMatcher alternated )
            {
                BaseMatcher found;
                foreach ( BaseMatcher subMatcher in alternated.PatternMatchers )
                    if ( ( found = subMatcher.FindInTree ( predicate ) ) != null )
                        return found;
            }
            else if ( matcher is SequentialMatcher sequential )
            {
                BaseMatcher found;
                foreach ( BaseMatcher subMatcher in sequential.PatternMatchers )
                    if ( ( found = subMatcher.FindInTree ( predicate ) ) != null )
                        return found;
            }
            else if ( matcher is MatcherWrapper wrapper )
            {
                return FindInTree ( wrapper.PatternMatcher, predicate );
            }

            return null;
        }

        public static IEnumerable<BaseMatcher> FindAllInTree ( this BaseMatcher matcher, Predicate<BaseMatcher> predicate )
        {
            if ( predicate == null )
                throw new ArgumentNullException ( nameof ( predicate ) );

            if ( predicate ( matcher ) )
                yield return matcher;

            if ( matcher is AlternatedMatcher alternated )
            {
                foreach ( BaseMatcher subMatcher in alternated.PatternMatchers )
                    foreach ( BaseMatcher found in subMatcher.FindAllInTree ( predicate ) )
                        yield return found;
            }
            else if ( matcher is SequentialMatcher sequential )
            {
                foreach ( BaseMatcher subMatcher in sequential.PatternMatchers )
                    foreach ( BaseMatcher found in subMatcher.FindAllInTree ( predicate ) )
                        yield return found;
            }
            else if ( matcher is MatcherWrapper wrapper )
            {
                foreach ( BaseMatcher found in wrapper.PatternMatcher.FindAllInTree ( predicate ) )
                    yield return found;
            }
        }
    }
}
