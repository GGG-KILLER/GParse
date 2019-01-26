using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GParse.Common
{
    /// <summary>
    /// Represents a diagnostic emmited by the compiler, such as an error, warning, suggestion, etc.
    /// </summary>
    public readonly struct Diagnostic
    {
        /// <summary>
        /// The ID of the emitted diagnostic
        /// </summary>
        public readonly String Id;

        /// <summary>
        /// The location that the diagnostic is reffering to in the code
        /// </summary>
        public readonly SourceRange Range;

        /// <summary>
        /// The severity of the diagnostic
        /// </summary>
        public readonly DiagnosticSeverity Severity;

        /// <summary>
        /// Any locations that are related to the diagnostic
        /// </summary>
        public readonly ImmutableArray<SourceRange> RelatedLocations;

        /// <summary>
        /// Initializes a new diagnostic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="range"></param>
        /// <param name="severity"></param>
        public Diagnostic ( String id, SourceRange range, DiagnosticSeverity severity )
        {
            this.Id = id;
            this.Range = range;
            this.Severity = severity;
            this.RelatedLocations = ImmutableArray<SourceRange>.Empty;
        }

        /// <summary>
        /// Initializes a new diagnostic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        /// <param name="severity"></param>
        public Diagnostic ( String id, SourceLocation location, DiagnosticSeverity severity ) : this ( id, location.To ( location ), severity )
        {
        }

        /// <summary>
        /// Initializes a new diagnostic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="range"></param>
        /// <param name="severity"></param>
        /// <param name="relatedLocations"></param>
        public Diagnostic ( String id, SourceRange range, DiagnosticSeverity severity, IEnumerable<SourceRange> relatedLocations )
        {
            this.Id = id;
            this.Range = range;
            this.Severity = severity;
            this.RelatedLocations = relatedLocations.ToImmutableArray ( );
        }

        /// <summary>
        /// Initializes a new diagnostic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        /// <param name="severity"></param>
        /// <param name="relatedLocations"></param>
        public Diagnostic ( String id, SourceLocation location, DiagnosticSeverity severity, IEnumerable<SourceRange> relatedLocations ) : this ( id, location.To ( location ), severity, relatedLocations )
        {
        }
    }
}
