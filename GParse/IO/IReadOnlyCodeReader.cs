using System;
using System.Text.RegularExpressions;

namespace GParse.IO
{
    /// <summary>
    /// Defines a read-only stream reader meant for reading code, which provides line and column location
    /// info.
    /// </summary>
    public interface IReadOnlyCodeReader
    {
        #region Position Management

        /// <summary>
        /// Current line.
        /// </summary>
        Int32 Line { get; }

        /// <summary>
        /// Current column.
        /// </summary>
        Int32 Column { get; }

        /// <summary>
        /// Current position.
        /// </summary>
        Int32 Position { get; }

        /// <summary>
        /// The full location of the reader.
        /// </summary>
        SourceLocation Location { get; }

        #endregion Position Management

        /// <summary>
        /// The size of the stream of code being read.
        /// </summary>
        Int32 Length { get; }

        #region Non-mutable operations

        #region FindOffset

        /// <summary>
        /// Returns the offset of a given character from the current <see cref="Position" /> or -1 if not
        /// found.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        Int32 FindOffset ( Char ch );

        /// <summary>
        /// Finds the offset of a given character from the current <see cref="Position" /> that passes the
        /// provided <paramref name="predicate"/> or -1 if not found.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Int32 FindOffset ( Predicate<Char> predicate );

        /// <summary>
        /// Finds the offset of a given <paramref name="str" /> from the current <see cref="Position" />
        /// or -1 if not found.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        Int32 FindOffset ( String str );

        #endregion FindOffset

        #region IsNext

        /// <summary>
        /// Returns whether the character <paramref name="ch"/> is at the <see cref="Position"/> the reader is at.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        Boolean IsNext ( Char ch );

        /// <summary>
        /// Returns whether the string <paramref name="str"/> is at the <see cref="Position"/> the reader is at.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        Boolean IsNext ( String str );

        /// <summary>
        /// Returns whether the string <paramref name="span"/> is at the <see cref="Position"/> the reader is at.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        Boolean IsNext ( ReadOnlySpan<Char> span );

        #endregion IsNext

        #region Peek

        /// <summary>
        /// Returns the next character without advancing in the stream or null if the reader is at the end
        /// of the stream.
        /// </summary>
        /// <returns></returns>
        Char? Peek ( );

        /// <summary>
        /// Returns the character at the given offset without advancing in the stream or null if the
        /// reader is at the end of the stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        Char? Peek ( Int32 offset );

        #endregion Peek

        #region PeekRegex

        /// <summary>
        /// Attempts to match a regex without advancing the stream.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Match PeekRegex ( String expression );

        /// <summary>
        /// Attempts to match a regex without advancing the stream.
        /// </summary>
        /// <param name="regex">
        /// <para>
        /// A <see cref="Regex" /> instance that contains an expression starting with the \G modifier.
        /// </para>
        /// <para>
        /// An exception will be thrown if the match does not start at the same position the reader is
        /// located at.
        /// </para>
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method is offered purely for the performance benefits of regular expressions generated
        /// with Regex.CompileToAssembly
        /// (https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.compiletoassembly).
        /// It is not meant to be used with anything else, since all regexes passed in the form of strings
        /// are stored in an internal cache and the instances are initialized with
        /// <see cref="RegexOptions.Compiled" />.
        /// </remarks>
        Match PeekRegex ( Regex regex );

        #endregion PeekRegex

        #region PeekString

        /// <summary>
        /// Reads a string of the provided length without advancing the stream.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        String? PeekString ( Int32 length );

        #endregion PeekString

        #region PeekSpan

        /// <summary>
        /// Reads a span of the provided length without advancing the stream.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        ReadOnlySpan<Char> PeekSpan ( Int32 length );

        #endregion PeekString

        #endregion Non-mutable operations
    }
}
