using System;
using System.Collections;
using System.Collections.Generic;
using GParse.Math;

namespace GParse
{
    /// <summary>
    /// A class that implements both <see cref="IProgress{T}" /> and <see cref="IReadOnlyList{T}" /> for
    /// <see cref="Diagnostic" /> for use with components that require an <see cref="IProgress{T}" />
    /// </summary>
    public class DiagnosticList : IReadOnlyList<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics;

        /// <inheritdoc />
        public int Count =>
            _diagnostics.Count;

        /// <inheritdoc />
        public Diagnostic this[int index] =>
            _diagnostics[index];

        /// <summary>
        /// Initializes this <see cref="DiagnosticList" />
        /// </summary>
        public DiagnosticList()
        {
            _diagnostics = new List<Diagnostic>();
        }

        /// <summary>
        /// Reports a diagnostic
        /// </summary>
        /// <param name="diagnostic"></param>
        public void Report(Diagnostic diagnostic) =>
            _diagnostics.Add(diagnostic);

        /// <summary>
        /// Adds multiple diagnostics to this list.
        /// </summary>
        /// <param name="diagnostics">The diagnostics to be added.</param>
        public void AddRange(IEnumerable<Diagnostic> diagnostics) =>
            _diagnostics.AddRange(diagnostics);

        /// <summary>
        /// Adds multiple diagnostics to this list.
        /// </summary>
        /// <param name="diagnostics">The diagnostics to be added.</param>
        public void AddRange(params Diagnostic[] diagnostics) =>
            _diagnostics.AddRange(diagnostics);

        /// <summary>
        /// Adds an error to this list.
        /// </summary>
        /// <param name="id">The error ID.</param>
        /// <param name="description">The error's description.</param>
        /// <param name="range">The location range the error refers to.</param>
        public void ReportError(string id, string description, Range<int> range) =>
            Report(new Diagnostic(DiagnosticSeverity.Error, id, description, range));

        /// <summary>
        /// Adds a warning to this list.
        /// </summary>
        /// <param name="id">The warning ID.</param>
        /// <param name="description">The warning's description.</param>
        /// <param name="range">The location range the error refers to.</param>
        public void ReportWarning(string id, string description, Range<int> range) =>
            Report(new Diagnostic(DiagnosticSeverity.Warning, id, description, range));

        /// <summary>
        /// Adds an info to this list.
        /// </summary>
        /// <param name="id">The info's ID.</param>
        /// <param name="description">The info's description.</param>
        /// <param name="range">The location range the info refers to.</param>
        public void ReportInfo(string id, string description, Range<int> range) =>
            Report(new Diagnostic(DiagnosticSeverity.Info, id, description, range));

        /// <inheritdoc/>
        public IEnumerator<Diagnostic> GetEnumerator() =>
            _diagnostics.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() =>
            _diagnostics.GetEnumerator();
    }
}