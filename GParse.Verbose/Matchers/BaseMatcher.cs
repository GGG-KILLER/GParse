using System;
using GParse.Common.IO;
using GParse.Verbose.Abstractions;

namespace GParse.Verbose.Matchers
{
    public abstract class BaseMatcher : IPatternMatcher
    {
        #region Pattern Matchers Composition

        #region Pattern Repetition

        /// <summary>
        /// Makes sure this pattern will only match once until
        /// <see cref="ResetInternalState" /> is called (this is
        /// different from the "once" in the
        /// <see cref="Verbose.Matching" /> class, which means it
        /// won't consume that sequence twice even if it can)
        /// isn't called.
        /// </summary>
        /// <returns></returns>
        public BaseMatcher Once ( ) => new OnceMatcher ( this );

        /// <summary>
        /// Makes this pattern optional
        /// </summary>
        /// <returns></returns>
        public BaseMatcher Optional ( ) => new OptionalMatcher ( this );

        /// <summary>
        /// Alias for <see cref="Optional" />
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public static BaseMatcher operator ~ ( BaseMatcher matcher ) => matcher.Optional ( );

        /// <summary>
        /// Makes so that this pattern is executed as many times
        /// as posible.
        /// </summary>
        /// <returns></returns>
        public BaseMatcher Infinite ( ) => new InfiniteMatcher ( this );

        /// <summary>
        /// Enables this pattern to be matched at most
        /// <paramref name="limit" /> times
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public BaseMatcher Repeat ( Int32 limit ) => new RepeatedMatcher ( this, limit );

        /// <summary>
        /// Enables this pattern to be matched infinite or a
        /// limited number of times (-1 for infinite)
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static BaseMatcher operator * ( BaseMatcher matcher, Int32 limit )
            => limit == -1 ? matcher.Infinite ( ) : matcher.Repeat ( limit );

        /// <summary>
        /// Negates the current pattern
        /// </summary>
        /// <returns></returns>
        public BaseMatcher Negate ( ) => new NegatedMatcher ( this );

        /// <summary>
        /// Alias for <see cref="Negate" />
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static BaseMatcher operator - ( BaseMatcher operand ) => operand.Negate ( );

        public static BaseMatcher operator ! ( BaseMatcher operand ) => operand.Negate ( );

        #endregion Pattern Repetition

        #region Sequentiation

        /// <summary>
        /// Makes sure this pattern will be followed by <paramref name="matcher" />
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public BaseMatcher Then ( BaseMatcher matcher )
        {
            BaseMatcher[] arr;
            if ( this is AllMatcher all )
            {
                // Create a new array with an expanded size
                arr = new BaseMatcher[all.PatternMatchers.Length + 1];
                // Copy over the old array
                Array.Copy ( all.PatternMatchers, arr, all.PatternMatchers.Length );
                // Then insert the element to be added at the end
                arr[all.PatternMatchers.Length] = matcher;
            }
            else
                arr = new[] { this, matcher };
            return new AllMatcher ( arr );
        }

        /// <summary>
        /// Alias for <see cref="Then(BaseMatcher)" />
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static BaseMatcher operator + ( BaseMatcher lhs, BaseMatcher rhs ) => lhs.Then ( rhs );

        /// <summary>
        /// Alias for <see cref="Then(BaseMatcher)" />
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static BaseMatcher operator & ( BaseMatcher lhs, BaseMatcher rhs ) => lhs.Then ( rhs );

        /// <summary>
        /// Makes sure that the what's next won't match <paramref name="matcher" />
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public BaseMatcher NotThen ( BaseMatcher matcher ) => this.Then ( !matcher );

        /// <summary>
        /// Alias for <see cref="NotThen(BaseMatcher)" />
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static BaseMatcher operator - ( BaseMatcher lhs, BaseMatcher rhs ) => lhs.NotThen ( rhs );

        /// <summary>
        /// Matches either the current pattern or the <paramref name="alternative" />
        /// </summary>
        /// <param name="alternative"></param>
        /// <returns></returns>
        public BaseMatcher Or ( BaseMatcher alternative )
        {
            BaseMatcher[] arr;
            if ( this is AnyMatcher any )
            {
                arr = new BaseMatcher[any.PatternMatchers.Length + 1];
                Array.Copy ( any.PatternMatchers, arr, any.PatternMatchers.Length );
                arr[any.PatternMatchers.Length] = alternative;
            }
            else
                arr = new[] { this, alternative };
            return new AnyMatcher ( arr );
        }

        /// <summary>
        /// Alias of <see cref="Or(BaseMatcher)" />
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static BaseMatcher operator | ( BaseMatcher lhs, BaseMatcher rhs ) => lhs.Or ( rhs );

        #endregion Sequentiation

        /// <summary>
        /// Tells the parser to store the name of this match so
        /// that transformations
        /// </summary>
        /// <param name="name"></param>
        /// <param name="RuleEnter"></param>
        /// <param name="RuleMatched"></param>
        /// <param name="RuleExit"></param>
        /// <returns></returns>
        public BaseMatcher As ( String name, Action<String> RuleEnter, Action<String, String[]> RuleMatched, Action<String> RuleExit )
            => new RuleWrapper ( this, name, RuleEnter, RuleMatched, RuleExit );

        #region Content Modification

        public BaseMatcher Ignore ( ) => new IgnoreMatcher ( this );

        public BaseMatcher Join ( ) => new JoinMatcher ( this );

        #endregion Content Modification

        #endregion Pattern Matchers Composition

        #region IPatternMatcher API

        public abstract Boolean IsMatch ( SourceCodeReader reader, out Int32 length, Int32 offset = 0 );

        public abstract String[] Match ( SourceCodeReader reader );

        //public virtual String[] Match ( SourceCodeReader reader )
        //{
        //    if ( this.IsMatch ( reader, out var len, 0 ) )
        //        return new[] { reader.ReadString ( len ) };
        //    throw new ParseException ( reader.Location, "Invalid expression." );
        //}

        public virtual void ResetInternalState ( ) { /* noop */ }

        #endregion IPatternMatcher API
    }
}
