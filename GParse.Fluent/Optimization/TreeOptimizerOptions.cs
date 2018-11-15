using System;

namespace GParse.Fluent.Optimization
{
    /// <summary>
    /// The options that the <see cref="MatchTreeOptimizer"/> uses
    /// </summary>
    public struct TreeOptimizerOptions
    {
        /// <summary>
        /// The Optimization flags for <see cref="Matchers.SequentialMatcher"/>
        /// </summary>
        [Flags]
        public enum SequentialMatcherFlags
        {
            /// <summary>
            /// Enables no optimizations
            /// </summary>
            None = 0,

            /// <summary>
            /// Flatten <see cref="Matchers.SequentialMatcher" />
            /// trees into a single level <see cref="Matchers.SequentialMatcher" />
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

        /// <summary>
        /// The flags for the sequential matcher
        /// </summary>
        public SequentialMatcherFlags SequentialMatcher;

        /// <summary>
        /// Flags for alternated matchers
        /// </summary>
        [Flags]
        public enum AlternatedMatcherFlags
        {
            /// <summary>
            /// Enables no optimizations
            /// </summary>
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
            /// Flattens all <see cref="AlternatedMatcher" /> into
            /// a single one
            /// </summary>
            Flatten = 32,

            /// <summary>
            /// Enables all optimizations
            /// </summary>
            All = JoinCharBasedMatchers | JoinIntersectingRanges | RemoveIntersectingChars | RemoveDuplicates | RangifyMatchers | Flatten
        }

        /// <summary>
        /// The alternated matcher
        /// </summary>
        public AlternatedMatcherFlags AlternatedMatcher;

        /// <summary>
        /// Ignore matcher flags
        /// </summary>
        [Flags]
        public enum IgnoreMatcherFlags
        {
            /// <summary>
            /// Enables no optimizations
            /// </summary>
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

            /// <summary>
            /// Enable all optimizations
            /// </summary>
            All = RemoveNestedIgores | RemoveNestedJoins
        }

        /// <summary>
        /// Ignore matcher flags
        /// </summary>
        public IgnoreMatcherFlags IgnoreMatcher;

        /// <summary>
        /// Join matcher flags
        /// </summary>
        [Flags]
        public enum JoinMatcherFlags
        {
            /// <summary>
            /// Enables no optimizations
            /// </summary>
            None = 0,

            /// <summary>
            /// Ignore any inner
            /// <see cref="Matchers.JoinMatcher" /> since the one
            /// at the root will do the job.
            /// </summary>
            IgnoreInnerJoins = 1,

            /// <summary>
            /// Enables all optimizations
            /// </summary>
            All = IgnoreInnerJoins
        }

        /// <summary>
        /// Join matcher flags
        /// </summary>
        public JoinMatcherFlags JoinMatcher;

        /// <summary>
        /// Negated matcher flags
        /// </summary>
        [Flags]
        public enum NegatedMatcherFlags
        {
            /// <summary>
            /// Enables no optimizations
            /// </summary>
            None = 0,

            /// <summary>
            /// Transform a negated matcher with a negated matcher
            /// as it's child into the child of the child negated matcher.
            /// </summary>
            RemoveDoubleNegations = 1,

            /// <summary>
            /// Enables all optimizations
            /// </summary>
            All = RemoveDoubleNegations
        }

        /// <summary>
        /// Negated matcher flags
        /// </summary>
        public NegatedMatcherFlags NegatedMatcher;

        /// <summary>
        /// Optional matcher flags
        /// </summary>
        [Flags]
        public enum OptionalMatcherFlags
        {
            /// <summary>
            /// Enables no optimizations
            /// </summary>
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

            /// <summary>
            /// Enables all optimizations
            /// </summary>
            All = JoinWithNestedRepeatMatcher
        }

        /// <summary>
        /// Optional matcher flags
        /// </summary>
        public OptionalMatcherFlags OptionalMatcher;

        /// <summary>
        /// An instance that enables all optimizations
        /// </summary>
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
