using System;

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
        public String Id { get; }

        /// <summary>
        /// The description of this diagnostic
        /// </summary>
        public String Description { get; }

        /// <summary>
        /// The location that the diagnostic is reffering to in the code
        /// </summary>
        public SourceRange Range { get; }

        /// <summary>
        /// Initializes a new diagnostic
        /// </summary>
        /// <param name="severity"><inheritdoc cref="Severity" path="/summary"/></param>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <param name="description"><inheritdoc cref="Description" path="/summary"/></param>
        /// <param name="range"><inheritdoc cref="Range" path="/summary"/></param>
        public Diagnostic ( DiagnosticSeverity severity, String id, String description, SourceRange range )
        {
            this.Id = id;
            this.Range = range;
            this.Severity = severity;
            this.Description = description;
        }
    }
}
