using System;
using GParse.Verbose;
using GParse.Verbose.Matchers;

namespace GParse.CLI
{
    internal class Program
    {
        private static void Main ( )
        {
            var lang = new Language ( "Calculator" );
            BaseMatcher @int = ( Match.ByFilter ( Char.IsDigit ) * -1 ).As ( "integer" );
            BaseMatcher num = ( @int
                + ~( Match.Char ( '.' ) + @int )
                + ~( Match.Chars ( 'E', 'e' ) + Match.Chars ( '+', '-' ).Optional ( ) + @int ) ).As ( "number" );
        }
    }
}
