﻿using System;

namespace GParse.Common.Errors
{
    public class ParseException : Exception
    {
        public SourceLocation Location;

        public ParseException ( SourceLocation sourceLocation, String message ) : base ( $"{sourceLocation.Line}:{sourceLocation.Column}  {message}" )
        {
            this.Location = sourceLocation;
        }
    }
}