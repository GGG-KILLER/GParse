using System;
using System.Collections;
using System.Collections.Generic;

namespace GParse
{
    /// <summary>
    /// A class that implements both <see cref="IProgress{T}" /> and <see cref="IReadOnlyList{T}" /> for
    /// <see cref="Diagnostic" /> for use with components that require an <see cref="IProgress{T}" />
    /// </summary>
    public class DiagnosticList : IReadOnlyList<Diagnostic>, IProgress<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics;

        /// <inheritdoc />
        public Int32 Count =>
            this._diagnostics.Count;

        /// <inheritdoc />
        public Diagnostic this[Int32 index] =>
            this._diagnostics[index];

        /// <summary>
        /// Initializes this <see cref="DiagnosticList" />
        /// </summary>
        public DiagnosticList ( )
        {
            this._diagnostics = new List<Diagnostic> ( );
        }

        /// <summary>
        /// Reports a diagnostic
        /// </summary>
        /// <param name="item"></param>
        public void Report ( Diagnostic item ) =>
            this._diagnostics.Add ( item );

        /// <summary>
        /// Adds a diagnostic to this list.
        /// </summary>
        /// <param name="diagnostic">The diagnostic to be added.</param>
        public void Add ( Diagnostic diagnostic ) =>
            this._diagnostics.Add ( diagnostic );

        /// <summary>
        /// Adds multiple diagnostics to this list.
        /// </summary>
        /// <param name="diagnostics">The diagnostics to be added.</param>
        public void AddRange ( IEnumerable<Diagnostic> diagnostics ) =>
            this._diagnostics.AddRange ( diagnostics );

        /// <summary>
        /// Adds multiple diagnostics to this list.
        /// </summary>
        /// <param name="diagnostics">The diagnostics to be added.</param>
        public void AddRange ( params Diagnostic[] diagnostics ) =>
            this._diagnostics.AddRange ( diagnostics );

        /// <summary>
        /// Adds an error to this list.
        /// </summary>
        /// <param name="id">The error ID.</param>
        /// <param name="description">The error's description.</param>
        /// <param name="range">The location range the error refers to.</param>
        public void ReportError ( String id, String description, SourceRange range ) =>
            this.Add ( new Diagnostic ( DiagnosticSeverity.Error, id, description, range ) );

        /// <summary>
        /// Adds a warning to this list.
        /// </summary>
        /// <param name="id">The warning ID.</param>
        /// <param name="description">The warning's description.</param>
        /// <param name="range">The location range the error refers to.</param>
        public void ReportWarning ( String id, String description, SourceRange range ) =>
            this.Add ( new Diagnostic ( DiagnosticSeverity.Warning, id, description, range ) );

        /// <summary>
        /// Adds an info to this list.
        /// </summary>
        /// <param name="id">The info's ID.</param>
        /// <param name="description">The info's description.</param>
        /// <param name="range">The location range the info refers to.</param>
        public void ReportInfo ( String id, String description, SourceRange range ) =>
            this.Add ( new Diagnostic ( DiagnosticSeverity.Info, id, description, range ) );

        /// <inheritdoc/>
        public IEnumerator<Diagnostic> GetEnumerator ( ) =>
            this._diagnostics.GetEnumerator ( );

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator ( ) =>
            this._diagnostics.GetEnumerator ( );
    }
}
