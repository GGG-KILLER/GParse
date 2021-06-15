using System.Collections.Generic;

namespace GParse
{
    internal static class EnumerableUtil
    {
        /// <summary>
        /// Yields a single value as an enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> Yield<T>(T value)
        {
            yield return value;
        }
    }
}