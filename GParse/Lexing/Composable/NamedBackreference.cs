﻿using System;
using GParse.Composable;

namespace GParse.Lexing.Composable
{
    /// <summary>
    /// Represents a backreference to a captured group.
    /// </summary>
    public class NamedBackreference : GrammarNode<Char>
    {
        /// <summary>
        /// The name of the capture this reference references.
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// Initializes a new backreference.
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name" path="/summary" /></param>
        public NamedBackreference ( String name )
        {
            if ( String.IsNullOrWhiteSpace ( name ) )
                throw new ArgumentException ( $"'{nameof ( name )}' cannot be null or whitespace", nameof ( name ) );
            this.Name = name;
        }

        /// <summary>
        /// Converts this node back into a regex string.
        /// </summary>
        /// <returns></returns>
        public override String ToString ( ) =>
            $"\\k<{this.Name}>";
    }
}