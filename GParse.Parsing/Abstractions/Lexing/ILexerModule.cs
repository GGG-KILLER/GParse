using System;
using System.Collections.Generic;
using System.Text;
using GParse.Common.IO;
using GParse.Common.Lexing;

namespace GParse.Parsing.Abstractions.Lexing
{
    public interface ILexerModule
    {
        /// <summary>
        /// Parser module name
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The module prefix
        /// </summary>
        String Prefix { get; }

        /// <summary>
        /// Whether this module can consume what's left in the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        Boolean CanConsumeNext ( SourceCodeReader reader );

        /// <summary>
        /// Consume the next element in the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        Token ConsumeNext ( SourceCodeReader reader );
    }
}
