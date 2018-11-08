using System;
using System.Text;

namespace GParse.Common.IO
{
    public class CodeWriter
    {
        private readonly StringBuilder Builder;
        private String IndentationCached;
        public Int32 Indentation { get; private set; }

        public CodeWriter ( )
        {
            this.Builder = new StringBuilder ( );
            this.Indentation = 0;
        }

        public void Reset ( )
        {
            this.Indentation = 0;
            this.IndentationCached = "";
            this.Builder.Clear ( );
        }

        public void Indent ( )
        {
            this.Indentation++;
            this.IndentationCached += '\t';
        }

        public void Outdent ( )
        {
            this.Indentation--;
            this.IndentationCached = this.IndentationCached.Substring ( 0, this.Indentation );
        }

        #region Write(Indented)

        public void Write ( Object value ) => this.Builder.Append ( value );

        public void Write ( String value ) => this.Builder.Append ( value );

        public void Write ( String format, params Object[] args ) => this.Builder.AppendFormat ( format, args );

        public void WriteIndented ( Object value )
        {
            this.Builder.Append ( this.IndentationCached );
            this.Builder.Append ( value );
        }

        public void WriteIndented ( String value )
        {
            this.Builder.Append ( this.IndentationCached );
            this.Builder.Append ( value );
        }

        public void WriteIndented ( String format, params Object[] args )
        {
            this.Builder.Append ( this.IndentationCached );
            this.Builder.AppendFormat ( format, args );
        }

        #endregion Write(Indented)

        #region WriteLine(Indented)

        public void WriteLine ( Object value ) => this.Builder.AppendLine ( value.ToString ( ) );

        public void WriteLine ( String value ) => this.Builder.AppendLine ( value.ToString ( ) );

        public void WriteLine ( String format, params Object[] args )
        {
            this.Builder.AppendFormat ( format, args );
            this.Builder.AppendLine ( );
        }

        public void WriteLineIndented ( Object value )
        {
            this.Builder.Append ( this.IndentationCached );
            this.Builder.AppendLine ( value.ToString ( ) );
        }

        public void WriteLineIndented ( String value )
        {
            this.Builder.Append ( this.IndentationCached );
            this.Builder.AppendLine ( value.ToString ( ) );
        }

        public void WriteLineIndented ( String format, params Object[] args )
        {
            this.Builder.Append ( this.IndentationCached );
            this.Builder.AppendFormat ( format, args );
            this.Builder.AppendLine ( );
        }

        #endregion WriteLine(Indented)

        public void WithIndentation ( Action cb )
        {
            if ( cb == null )
                throw new ArgumentNullException ( nameof ( cb ) );

            this.Indent ( );
            cb ( );
            this.Outdent ( );
        }

        public String GetCode ( ) => this.Builder.ToString ( );
    }
}
