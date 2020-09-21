using System;
using System.Collections.Generic;
using GParse.IO;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// The delegate that represents the implementation of a grammar node's match action.
    /// </summary>
    /// <param name="reader">The reader to use.</param>
    /// <param name="offset">The offset to start matching at.</param>
    /// <param name="captures">The dictionary containing all captures.</param>
    /// <returns></returns>
    public delegate SimpleMatch SimpleMatchDelegate ( IReadOnlyCodeReader reader,
                                                      Int32 offset,
                                                      IDictionary<String, Capture>? captures = null );
}