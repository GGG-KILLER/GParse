using System;
using System.Text;

namespace GParse.Common.IO
{
    /// <summary>
    /// Defines a code writer
    /// </summary>
    public class CodeWriter
    {
        private readonly StringBuilder Builder;
        private String IndentationCached;

        /// <summary>
        /// The indentation level of the writer
        /// </summary>
        public Int32 Indentation { get; private set; }

        /// <summary>
        /// Initializes this class
        /// </summary>
        public CodeWriter ( )
        {
            this.Builder = new StringBuilder ( );
            this.Indentation = 0;
        }

        /// <summary>
        /// Resets the writer
        /// </summary>
        public void Reset ( )
        {
            this.Indentation = 0;
            this.IndentationCached = "";
            this.Builder.Clear ( );
        }

        /// <summary>
        /// Increases the indentation level
        /// </summary>
        public void Indent ( )
        {
            this.Indentation++;
            this.IndentationCached += '\t';
        }

        /// <summary>
        /// Decreases the indentation level
        /// </summary>
        public void Outdent ( )
        {
            this.Indentation--;
            this.IndentationCached = this.IndentationCached.Substring ( 0, this.Indentation );
        }

        /// <summary>
        /// Decreases the indentation level
        /// </summary>
        public void Unindent ( ) => this.Outdent ( );

        #region Write(Indented)

        /// <summary>
        /// Writes a value
        /// </summary>
        /// <param name="value"></param>
        public void Write ( Object value ) => this.Builder.Append ( value );

        /// <summary>
        /// Writes a value
        /// </summary>
        /// <param name="value"></param>
        public void Write ( String value ) => this.Builder.Append ( value );

        /// <summary>
        /// Writes a formatted value
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Write ( String format, params Object[] args ) => this.Builder.AppendFormat ( format, args );

        /// <summary>
        /// Writes prefixed by indentation
        /// </summary>
        /// <param name="value"></param>
        public void WriteIndented ( Object value ) => this.Builder.Append ( this.IndentationCached ).Append ( value );

        /// <summary>
        /// Writes prefixed by indentation
        /// </summary>
        /// <param name="value"></param>
        public void WriteIndented ( String value ) => this.Builder.Append ( this.IndentationCached ).Append ( value );

        /// <summary>
        /// WRites formatted prefixed by indentation
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteIndented ( String format, params Object[] args ) => this.Builder.Append ( this.IndentationCached ).AppendFormat ( format, args );

        #endregion Write(Indented)

        #region WriteLine(Indented)

        /// <summary>
        /// Writes a value followed by the line terminator
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine ( Object value ) => this.Builder.AppendLine ( value.ToString ( ) );

        /// <summary>
        /// Writes a value followed by the line terminator
        /// </summary>
        /// <param name="value"></param>
        public void WriteLine ( String value ) => this.Builder.AppendLine ( value.ToString ( ) );

        /// <summary>
        /// Writes a formatted value followed by the line terminator
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLine ( String format, params Object[] args ) => this.Builder.AppendFormat ( format, args ).AppendLine ( );

        /// <summary>
        /// Writes a value followed by the line terminator and
        /// prefixed by the indetation
        /// </summary>
        /// <param name="value"></param>
        public void WriteLineIndented ( Object value ) => this.Builder.Append ( this.IndentationCached ).AppendLine ( value.ToString ( ) );

        /// <summary>
        /// Writes a value followed by the line terminator and
        /// prefixed by the indetation
        /// </summary>
        /// <param name="value"></param>
        public void WriteLineIndented ( String value ) => this.Builder.Append ( this.IndentationCached ).AppendLine ( value.ToString ( ) );

        /// <summary>
        /// Writes a formatted value followed by the line
        /// terminator and prefixed by the indetation
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLineIndented ( String format, params Object[] args ) => this.Builder.Append ( this.IndentationCached ).AppendFormat ( format, args ).AppendLine ( );

        #endregion WriteLine(Indented)

        /// <summary>
        /// Increases the indentation before the callback and decreases it after
        /// </summary>
        /// <param name="cb"></param>
        public void WithIndentation ( Action cb )
        {
            if ( cb == null )
                throw new ArgumentNullException ( nameof ( cb ) );

            this.Indent ( );
            cb ( );
            this.Outdent ( );
        }

        /// <summary>
        /// Gets the entire code as a string
        /// </summary>
        /// <returns></returns>
        public String GetCode ( ) => this.Builder.ToString ( );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) => this.GetCode ( );
    }
}
