namespace GParse.Common.Lexing
{
    public enum TokenType
    {
        /// <summary>
        /// char
        /// </summary>
        Char,

        /// <summary>
        /// comments
        /// </summary>
        Comment,

        /// <summary>
        /// EOF
        /// </summary>
        EOF,

        /// <summary>
        /// var/class/struct/whatever names
        /// </summary>
        Identifier,

        /// <summary>
        /// keywords
        /// </summary>
        Keyword,

        /// <summary>
        /// [
        /// </summary>
        LBracket,

        /// <summary>
        /// {
        /// </summary>
        LCurly,

        /// <summary>
        /// (
        /// </summary>
        LParen,

        /// <summary>
        /// numbers
        /// </summary>
        Number,

        /// <summary>
        /// operators
        /// </summary>
        Operator,

        /// <summary>
        /// ]
        /// </summary>
        RBracket,

        /// <summary>
        /// }
        /// </summary>
        RCurly,

        /// <summary>
        /// )
        /// </summary>
        RParen,

        /// <summary>
        /// string
        /// </summary>
        String,

        /// <summary>
        /// . , ; : ?
        /// </summary>
        Punctuation,

        /// <summary>
        /// ' ', '\t', '\n', '\r'
        /// </summary>
        Whitespace,

        /// <summary>
        /// Any other kind of token
        /// </summary>
        Other,
    }
}
