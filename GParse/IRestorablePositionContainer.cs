using System;

namespace GParse
{
    /// <summary>
    /// The interface of classes that act over code and can have their positions restored.
    /// </summary>
    public interface IRestorablePositionContainer : IPositionContainer
    {
        /// <summary>
        /// Restores the position to a provided <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The location the position should be restored to.</param>
        /// <remarks>
        /// No validation is done to check that the provided line and column numbers are correct.
        /// </remarks>
        void Restore(SourceLocation location);

        /// <summary>
        /// Restores the position to a given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The position to be restored to.</param>
        /// <remarks>
        /// When restoring a position instead of a <see cref="SourceLocation"/>, if the last
        /// known location is after the position being restored to, we will have to recalculate
        /// the position from the start of the code until the provided position to obtain the
        /// <see cref="SourceLocation"/> when /// <see cref="IPositionContainer.GetLocation()"/>
        /// is called.
        /// </remarks>
        void Restore(Int32 position);
    }
}