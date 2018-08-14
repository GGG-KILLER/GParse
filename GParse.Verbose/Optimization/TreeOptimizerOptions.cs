using System;

namespace GParse.Verbose.Optimization
{
    public struct TreeOptimizerOptions
    {
        [Flags]
        public enum SequentialMatcherFlags
        {
            None = 0,

            /// <summary>
            /// Flatten <see cref="Matchers.SequentialMatcher" /> trees
            /// into a single level <see cref="Matchers.SequentialMatcher" />
            /// </summary>
            Flatten = 1,

            /// <summary>
            /// Transforms sequential
            /// <see cref="Matchers.CharMatcher" /> into <see cref="Matchers.StringMatcher" />
            /// </summary>
            Stringify = 2,

            /// <summary>
            /// All <see cref="Matchers.SequentialMatcher" /> optimizations
            /// </summary>
            All = Flatten & Stringify
        }

        public SequentialMatcherFlags SequentialMatcher;

        [Flags]
        public enum AlternatedMatcherFlags
        {
            None = 0,

            /// <summary>
            /// Joins multiple <see cref="Matchers.CharMatcher" />
            /// into a <see cref="Matchers.CharListMatcher" />
            /// </summary>
            JoinCharBasedMatchers = 1,

            /// <summary>
            /// Joins multiple intersecting <see cref="Matchers.RangeMatcher" />
            /// </summary>
            JoinIntersectingRanges = 2,

            /// <summary>
            /// Removes any <see cref="Matchers.CharMatcher" /> or
            /// <see cref="Matchers.CharListMatcher" /> that
            /// belongs to any
            /// <see cref="Matchers.RangeMatcher" /> in the <see cref="Matchers.AlternatedMatcher" />
            /// </summary>
            RemoveIntersectingChars = 4,

            /// <summary>
            /// Removes duplicates from the <see cref="Matchers.AlternatedMatcher" />
            /// </summary>
            RemoveDuplicates = 8,

            /// <summary>
            /// Transforms sequential chars into ranges for
            /// performance (e.g.: [abcdef] will be turned into [a-f])
            /// </summary>
            RangifyMatchers = 16,

            /// <summary>
            /// Flattens all <see cref="AlternatedMatcher" /> into a
            /// single one
            /// </summary>
            Flatten = 32,

            All = JoinCharBasedMatchers | JoinIntersectingRanges | RemoveIntersectingChars | RemoveDuplicates | RangifyMatchers | Flatten
        }

        public AlternatedMatcherFlags AlternatedMatcher;

        [Flags]
        public enum IgnoreMatcherFlags
        {
            None = 0,

            /// <summary>
            /// Remove any nested
            /// <see cref="Matchers.IgnoreMatcher" /> since the
            /// one at the root will do the job
            /// </summary>
            RemoveNestedIgores = 1,

            /// <summary>
            /// Remove any nested
            /// <see cref="Matchers.JoinMatcher" /> since the
            /// joined contents will be ignored in the end
            /// </summary>
            RemoveNestedJoins = 2,

            All = RemoveNestedIgores | RemoveNestedJoins
        }

        public IgnoreMatcherFlags IgnoreMatcher;

        [Flags]
        public enum JoinMatcherFlags
        {
            None = 0,

            /// <summary>
            /// Ignore any inner
            /// <see cref="Matchers.JoinMatcher" /> since the one
            /// at the root will do the job.
            /// </summary>
            IgnoreInnerJoins = 1,

            All = IgnoreInnerJoins
        }

        public JoinMatcherFlags JoinMatcher;

        [Flags]
        public enum NegatedMatcherFlags
        {
            None = 0,

            /// <summary>
            /// Transform a negated matcher with a negated matcher
            /// as it's child into the child of the child negated matcher.
            /// </summary>
            RemoveDoubleNegations = 1,

            All = RemoveDoubleNegations
        }

        public NegatedMatcherFlags NegatedMatcher;

        [Flags]
        public enum OptionalMatcherFlags
        {
            None = 0,

            /// <summary>
            /// If the child of the
            /// <see cref="Matchers.OptionalMatcher" /> is a
            /// <see cref="Matchers.RepeatedMatcher" /> with a
            /// minimum match count less than 2, return a
            /// <see cref="Matchers.RepeatedMatcher" /> with a
            /// minimum match count of 0 instead.
            /// </summary>
            JoinWithNestedRepeatMatcher = 1,

            All = JoinWithNestedRepeatMatcher
        }

        public OptionalMatcherFlags OptionalMatcher;

        public static readonly TreeOptimizerOptions All = new TreeOptimizerOptions
        {
            SequentialMatcher = SequentialMatcherFlags.All,
            AlternatedMatcher = AlternatedMatcherFlags.All,
            IgnoreMatcher = IgnoreMatcherFlags.All,
            JoinMatcher = JoinMatcherFlags.All,
            NegatedMatcher = NegatedMatcherFlags.All
        };
    }
}
