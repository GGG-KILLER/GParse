using System;
using GParse.Verbose.Matchers;

namespace GParse.Verbose
{
    public static class Match
    {
        /// <summary>
        /// Matches a single character a single time
        /// </summary>
        /// <param name="ch">Character to match</param>
        /// <returns></returns>
        public static BaseMatcher Char ( Char ch ) => new CharMatcher ( ch );

        /// <summary>
        /// Matches a single char from the list a single time
        /// </summary>
        /// <param name="chs"></param>
        /// <returns></returns>
        public static BaseMatcher Chars ( params Char[] chs ) => new MultiCharMatcher ( chs );

        /// <summary>
        /// Matches a single char inside the range provided.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="strict">
        /// Whether to use Start &lt; value &lt; End instead of
        /// Start ≤ value ≤ End
        /// </param>
        /// <returns></returns>
        public static BaseMatcher CharRange ( Char start, Char end, Boolean strict = false )
            => new CharRangeMatcher ( start, end, strict );

        /// <summary>
        /// Matches a single string a single time
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static BaseMatcher String ( String str ) => new StringMatcher ( str );

        /// <summary>
        /// Matches a single char a single time that passes the
        /// <paramref name="func" /> check
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static BaseMatcher ByFilter ( Func<Char, Boolean> func ) => new FilterFuncMatcher ( func );
    }
}
