using System;
using System.Collections;
using System.Collections.Generic;

namespace GParse.Common.IO
{
    public struct SourceCodeReaderEnumerator : IEnumerator<Char>
    {
        private readonly SourceLocation StartingLocation;
        private SourceCodeReader Reader;
        private Boolean MovedForFirstTime;

        public Char Current
        {
            get
            {
                if ( !this.MovedForFirstTime )
                    throw new InvalidOperationException ( "The iteration has not started. Call MoveNext to start it" );
                if ( !( this.Reader?.IsAtEOF is true ) )
                    throw new InvalidOperationException ( "The iteration has already finished." );

                return ( Char ) this.Reader.Peek ( );
            }
        }

        Object IEnumerator.Current
        {
            get
            {
                if ( !this.MovedForFirstTime )
                    throw new InvalidOperationException ( "The iteration has not started. Call MoveNext to start it" );
                if ( !( this.Reader?.IsAtEOF is true ) )
                    throw new InvalidOperationException ( "The iteration has already finished." );

                return ( Char ) this.Reader.Peek ( );
            }
        }

        public SourceCodeReaderEnumerator ( SourceCodeReader reader )
        {
            this.Reader = reader;
            this.StartingLocation = reader.Location;
            this.MovedForFirstTime = false;
        }

        public void Dispose ( ) => this.Reader = null;

        public Boolean MoveNext ( )
        {
            if ( !this.MovedForFirstTime )
            {
                this.MovedForFirstTime = true;
                return true;
            }
            else if ( this.Reader.HasContent )
            {
                this.Reader.Advance ( 1 );
                return true;
            }

            return false;
        }

        public void Reset ( )
        {
            this.Reader.Rewind ( this.StartingLocation );
            this.MovedForFirstTime = false;
        }
    }
}
