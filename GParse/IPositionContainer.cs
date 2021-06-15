using System;
using GParse.Math;

namespace GParse
{
    /// <summary>
    /// The interface of classes that act over code.
    /// </summary>
    public interface IPositionContainer
    {
        /// <summary>
        /// The code's length.
        /// </summary>
        Int32 Length { get; }

        /// <summary>
        /// The current position.
        /// </summary>
        Int32 Position { get; }

        /// <summary>
        /// The <see cref="SourceLocation"/> of the current position.
        /// </summary>
        SourceLocation GetLocation();

        /// <summary>
        /// Obtains the <see cref="SourceLocation"/> of a provided <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The position to obtain the location of.</param>
        /// <returns>The obtained location.</returns>
        SourceLocation GetLocation(Int32 position);

        /// <summary>
        /// Translates a <see cref="Range{T}"/> into a <see cref="SourceRange"/>.
        /// </summary>
        /// <param name="range">The position range to translate into a source range.</param>
        /// <returns>The obtained range.</returns>
        SourceRange GetLocation(Range<Int32> range);
    }
}