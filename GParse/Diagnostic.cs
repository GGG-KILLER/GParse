using GParse.Math;

namespace GParse
{
    /// <summary>
    /// Represents a diagnostic emmited by the compiler, such as an error, warning, suggestion, etc.
    /// </summary>
    public class Diagnostic
    {
        /// <summary>
        /// The severity of the diagnostic
        /// </summary>
        public DiagnosticSeverity Severity { get; }

        /// <summary>
        /// The ID of the emitted diagnostic
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The description of this diagnostic
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The location that the diagnostic is reffering to in the code.
        /// </summary>
        public Range<int> Range { get; }

        /// <summary>
        /// Initializes a new diagnostic
        /// </summary>
        /// <param name="severity"><inheritdoc cref="Severity" path="/summary"/></param>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="description"><inheritdoc cref="Description" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        public Diagnostic(DiagnosticSeverity severity, string id, string description, Range<int> range)
        {
            Id = id;
            Range = range;
            Severity = severity;
            Description = description;
        }
    }
}