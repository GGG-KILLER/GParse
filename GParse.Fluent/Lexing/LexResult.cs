using System;
using GParse.Common.Errors;

namespace GParse.Fluent.Lexing
{
    /// <summary>
    /// The result of a lexing operation
    /// </summary>
    public readonly struct LexResult
    {
        /// <summary>
        /// Whether this operation was a success
        /// </summary>
        public readonly Boolean Success;

        /// <summary>
        /// The matched text
        /// </summary>
        public readonly String Match;

        /// <summary>
        /// The error
        /// </summary>
        public readonly LexingException Error;

        /// <summary>
        /// Initializes this result
        /// </summary>
        /// <param name="match"></param>
        public LexResult ( String match )
        {
            this.Success = true;
            this.Match = match;
            this.Error = null;
        }

        /// <summary>
        /// Initializes this result
        /// </summary>
        /// <param name="error"></param>
        public LexResult ( LexingException error )
        {
            this.Success = false;
            this.Match = default;
            this.Error = error;
        }
    }
}
